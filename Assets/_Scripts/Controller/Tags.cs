using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MineCombat
{
    public interface ITag : IEquatable<ITag>
    {
        public bool Contains(string tag);
        public bool ContainedBy(ITags tags);
        public HashSet<string>? ToHashSet();
    }

    public class Tag : ITag 
    {
        public readonly string _tag;

        public Tag(string tag)
        {
            _tag = tag;
        }

        public bool Equals(ITag? other)
        {
            if (other is Tag otherTag)
                return _tag.Equals(otherTag._tag);
            return false;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as ITag);
        }

        public bool Contains(string tag)
        {
            return _tag.Equals(tag);
        }

        public bool ContainedBy(ITags tags)
        {
            return tags.Contains(_tag);
        }

        public HashSet<string>? ToHashSet()
        {
            return new HashSet<string> { _tag };
        }

        public override int GetHashCode()
        {
            return _tag.GetHashCode();
        }
        public override string ToString()
        {
            return _tag;
        }

        public static implicit operator Tag(string tag) => new Tag(tag);
    }

    public class TagSet : ITag
    {
        protected readonly HashSet<string>? _tags;

        public TagSet(IEnumerable<string>? tags = null)
        {
            _tags = tags?.Any() == true ? new HashSet<string>(tags) : null;
        }

        public TagSet(params string[] tags)
        {
            _tags = new HashSet<string>(tags);
        }

        public bool Equals(ITag? other)
        {
            if (other is TagSet otherTag && otherTag._tags is not null)
                return _tags?.SetEquals(otherTag._tags) == true;
            return false;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as ITag);
        }

        public bool Contains(string tag)
        {
            return _tags?.Contains(tag) == true;
        }

        public bool ContainedBy(ITags tags)
        {
            if (_tags is not null)
            {
                foreach (var tag in _tags)
                {
                    if (!tags.Contains(tag))
                        return false;
                }
                return true;
            }
            return false;
        }

        public HashSet<string>? ToHashSet()
        {
            return _tags;
        }

        public override int GetHashCode()
        {
            if (_tags is null) return 0;

            var hash = new HashCode();
            foreach (var tag in _tags.OrderBy(x => x))
            {
                hash.Add(tag);
            }
            return hash.ToHashCode();
        }
        public override string ToString()
        {
            if (_tags?.Any() == true)
                return '{' + string.Join(',', _tags) + '}';
            else return "{}";
        }

        public static implicit operator TagSet(string[] tags) => new TagSet(tags);
    }

    public interface ITags
    {
        public bool Contains(string tag);
        public bool Contains(ITag tag)
        {
            return tag.ContainedBy(this);
        }
        public bool Match(ITags target);
        internal HashSet<ITag> ToHashSet();
    }

    //静态标签集合只作为匹配目标，不需要复杂内部结构，也不能通过ITag初始化
    public class StaticTags : ITags
    {
#nullable enable
        protected readonly HashSet<string>? _tags;

        public StaticTags(params string[] tags)
        {
            _tags = new HashSet<string>(tags);
        }

        public StaticTags(IEnumerable<string>? tags = null)
        {
            _tags = tags?.Any() == true ? new HashSet<string>(tags) : null;
        }


        public bool Contains(string tag)
        {
            return _tags?.Contains(tag) == true;
        }

        public bool Match(ITags target)
        {
            return _tags?.Any(tag => target.Contains(tag)) == true;
        }

        HashSet<ITag> ITags.ToHashSet()
        {
            HashSet<ITag> tags = new();
            if (_tags?.Any() == true)
                foreach (var tag in _tags)
                {
                    tags.Add(new Tag(tag));
                }
            return tags;
        }

        public override string ToString()
        {
            if (_tags?.Any() == true)
                return '{' + string.Join(',', _tags) + '}';
            else return "{}";
        }

        public static implicit operator StaticTags(string[] tags) => new StaticTags(tags);
        public static implicit operator StaticTags(string tags)
        {
            IEnumerable<object>? collection = Parser.ToCollection(tags, 1);
            if (collection?.Any() == true)
            {
                List<string> strings = new();
                foreach (var item in collection)
                {
                    if (item is string str)
                        strings.Add(str);
                    else return new StaticTags(tags);
                }
                return new StaticTags(strings);
            }
            return new StaticTags(tags);
        }
#nullable disable
    }

    //普通标签集合既可以发起匹配，也可以作为匹配目标
    public class Tags : ITags
    {
#nullable enable
        protected List<ITag> _tags;
        protected HashSet<string> _flatView;

        protected void BuildFlatView()
        {
            _flatView.Clear();
            foreach (var tag in _tags)
            {
                _flatView.UnionWith(tag.ToHashSet() ?? Enumerable.Empty<string>());
            }
        }

        protected List<ITag> TryGetListFromUnknown(IEnumerable<object> items)
        {
            HashSet<ITag> result = new();
            foreach (var item in items)
            {
                switch (item)
                {
                    case string str:
                        result.Add((Tag)str);
                        break;
                    case ITag tag:
                        result.Add(tag);
                        break;
                    case string[] array:
                        result.Add((TagSet)array);
                        break;
                    case IEnumerable<string> collection:
                        result.Add((TagSet)collection);
                        break;
                    case IEnumerable<object> objects:
                        List<string> strings = new();
                        foreach (var obj in objects)
                        {
                            if (obj is string ss)
                                strings.Add(ss);
                            else throw new ArgumentException($"内部含有不支持的类型: {obj.GetType()}");
                        }
                        result.Add(new TagSet(strings));
                        break;

                    default:
                        throw new ArgumentException($"不支持的类型: {item.GetType()}");
                }
            }
            return result.ToList();
        }

        public Tags(params ITag[] tags)
        {
            _tags = new List<ITag>(tags);
            _flatView = new();
            BuildFlatView();
        }

        public Tags(IEnumerable<ITag> tags)
        {
            _tags = new List<ITag>(tags);
            _flatView = new();
            BuildFlatView();
        }

        public Tags(params object[] items)
        {
            _tags = TryGetListFromUnknown(items);
            _flatView = new();
            BuildFlatView();
        }

        public Tags(IEnumerable<object>? items = null)
        {
            _tags = items?.Any() == true ? TryGetListFromUnknown(items) : new List<ITag>();
            _flatView = new();
            BuildFlatView();
        }

        public bool Contains(string tag)
        {
            return _flatView.Contains(tag);
        }

        public bool Match(ITags target)
        {
            foreach (var tag in _tags)
            {
                if (target.Contains(tag))
                    return true;
            }
            return false;
        }

        HashSet<ITag> ITags.ToHashSet()
        {
            return new HashSet<ITag>(_tags);
        }

        public bool Add(ITag tag)
        {
            bool result = !_tags.Contains(tag);
            if (result)
            {
                _tags.Add(tag);
                _flatView.UnionWith(tag.ToHashSet() ?? Enumerable.Empty<string>());
            }
            return result;
        }

        public bool Remove(ITag tag)
        {
            bool result = _tags.Remove(tag);
            if (result) BuildFlatView();
            return result;
        }

        public Tags Merge(ITags other)
        {
            _tags.AddRange(other.ToHashSet().ToList());
            return this;
        }
        public Tags Exclude(ITags other)
        {
            _tags.RemoveAll(it => other.ToHashSet().Contains(it));
            return this;
        }

        public StaticTags GetStatic()
        {
            return new StaticTags(_flatView);
        }

        public override string ToString()
        {
            if (_tags.Any())
                return '{' + string.Join(',', _tags) + '}';
            else return "{}";
        }

        public static implicit operator Tags(string[] tags) => new Tags(tags);
        public static implicit operator Tags(object[] tags) => new Tags(tags);
        public static implicit operator Tags(string tags)
        {
            IEnumerable<object>? collection = Parser.ToCollection(tags, 2);
            if (collection?.Any() == true)
                return new Tags(collection);
            return new Tags(tags);
        }
#nullable disable
    }

    public class TagsManager
    {
        protected Dictionary<string, ITags> _tagsTable;

        public TagsManager() { _tagsTable = new(); }

        //静态标签集合拒绝任何修改，但允许外部用户操作普通标签集合
        public bool Add(string id, ITags ntags)
        {
            if (_tagsTable.TryGetValue(id, out var otags))
            {
                if (otags is Tags tags)
                { 
                    tags.Merge(ntags);
                    return true;
                }
                return false;
            }
            else
            {
                _tagsTable[id] = ntags;
                return true;
            }
        }

        public bool Remove(string id, ITags ntags)
        {
            if (_tagsTable.TryGetValue(id, out var otags) && otags is Tags tags)
            {
                tags.Exclude(ntags);
                return true;
            }
            return false;
        }

        public bool Delete(string id)
        {
            if (_tagsTable.TryGetValue(id, out var otags))
            {
                _tagsTable.Remove(id); 
                return true;
            }
            return false;
        }

        //目标标签集合的扁平化集合中至少包含src的一个完整子集
        public bool Match(string id, ITags src)
        {
            if (_tagsTable.TryGetValue(id, out var tags))
                return src.Match(tags);
            return false;
        }

        public bool Match(string id, string src)
        {
            if (_tagsTable.TryGetValue(id, out var tags))
                return tags.Contains(src);
            return false;
        }
    }
}
