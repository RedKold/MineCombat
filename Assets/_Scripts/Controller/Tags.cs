using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineCombat
{
    /* 注意：
     * 1.new Tags("...") != (Tags)"..."，后者会尝试解析字符串为一个集合，前者只会将字符串整体作为第一个ITag，对StaticTags同理
     * 2.(Tags)"" == new Tags("") != new Tags()，想要使用空的ITags，推荐使用StaticTags.Empty，可以有效节省开销 */
    public interface ITag : IEquatable<ITag>
    {
#nullable enable
        public bool Contains(string tag);
        public bool ContainedBy(ITags tags);
        public HashSet<string>? ToHashSet();
#nullable disable
    }

    public class Tag : ITag 
    {
#nullable enable
        public readonly string tag;

        public Tag(string tag)
        {
            this.tag = tag;
        }

        public bool Equals(ITag? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is Tag otherTag)
                return tag.Equals(otherTag.tag);
            return false;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null) 
                return false;
            
            return Equals(obj as ITag);
        }

        public bool Contains(string tag)
        {
            return this.tag.Equals(tag);
        }

        public bool ContainedBy(ITags tags)
        {
            return tags.Contains(tag);
        }

        public HashSet<string>? ToHashSet()
        {
            return new HashSet<string> { tag };
        }

        public override int GetHashCode()
        {
            return tag.GetHashCode();
        }
        public override string ToString()
        {
            return tag;
        }

        public static implicit operator Tag(string tag) => new Tag(tag);
#nullable disable
    }

    public class TagSet : ITag
    {
#nullable enable
        private readonly HashSet<string>? _tags;

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
            if (ReferenceEquals(this, other))
                return true;

            if (other is TagSet otherTag && otherTag._tags is not null)
                return _tags?.SetEquals(otherTag._tags) == true;
            return false;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

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
#nullable disable
    }

    public interface ITags : IEquatable<ITags>
    {
        public bool Contains(string tag);
        public bool Contains(ITag tag)
        {
            return tag.ContainedBy(this);
        }
        public bool Match(ITags target);
        internal HashSet<ITag> ToHashSet();
    }

    //静态标签集合可空，内部结构简单，不能通过ITag初始化
    public class StaticTags : ITags
    {
        public static readonly StaticTags Empty = new StaticTags();

#nullable enable
        private readonly HashSet<string>? _tags;

        public StaticTags(params string[] tags)
        {
            _tags = new HashSet<string>(tags);
        }

        public StaticTags(IEnumerable<string>? tags = null)
        {
            _tags = tags?.Any() == true ? new HashSet<string>(tags) : null;
        }

        public bool Equals(ITags? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is StaticTags otherTags && otherTags._tags is not null)
                return _tags?.SetEquals(otherTags._tags) == true;
            return false;
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
        private List<ITag> _tags;
        private HashSet<string> _flatView;
        private bool _updated;
        protected object _lock = new();

        private void BuildFlatView()
        {
            if (_updated) 
                return;

            _flatView.Clear();
            foreach (var tag in _tags)
            {
                HashSet<string>? flatView = tag.ToHashSet();
                if (flatView is not null)
                    _flatView.UnionWith(flatView);
            }
            _updated = true;
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
            _updated = false;
        }

        public Tags(IEnumerable<ITag> tags)
        {
            _tags = new List<ITag>(tags);
            _flatView = new();
            _updated = false;
        }

        public Tags(params object[] items)
        {
            _tags = TryGetListFromUnknown(items);
            _flatView = new();
            _updated = false;
        }

        public Tags(IEnumerable<object>? items = null)
        {
            _tags = items?.Any() == true ? TryGetListFromUnknown(items) : new List<ITag>();
            _flatView = new();
            _updated = false;
        }

        public bool Equals(ITags? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is Tags otherTags)
                lock (_lock) { return new HashSet<ITag>(_tags).SetEquals(otherTags._tags); }
            return false;
        }

        public bool Contains(string tag)
        {
            lock (_lock)
            {
                BuildFlatView();
                return _flatView.Contains(tag);
            }
        }

        public bool Match(ITags target)
        {
            lock (_lock)
            {
                foreach (var tag in _tags)
                {
                    if (target.Contains(tag))
                        return true;
                }
                return false;
            }
        }

        HashSet<ITag> ITags.ToHashSet()
        {
            lock (_lock) { return new HashSet<ITag>(_tags); }
        }

        public bool Add(ITag tag)
        {
            lock (_lock)
            {
                bool result = !_tags.Contains(tag);
                if (result)
                {
                    _tags.Add(tag);
                    if (_updated)
                    {
                        HashSet<string>? flatView = tag.ToHashSet();
                        if (flatView is not null)
                            _flatView.UnionWith(flatView);
                    }
                }
                return result;
            }
        }

        public bool Remove(ITag tag)
        {
            bool result;
            lock (_lock) { result = _tags.Remove(tag); }
            if (result)
                _updated = false;
            return result;
        }

        public Tags Merge(ITags other)
        {
            lock (_lock)
            {
                HashSet<ITag> set = new(_tags);
                foreach (var tag in other.ToHashSet())
                {
                    if (!set.Contains(tag))
                        _tags.Add(tag);
                }
            }
            _updated = false;
            return this;
        }
        public Tags Exclude(ITags other)
        {
            HashSet<ITag> set = other.ToHashSet();
            lock (_lock) { _tags.RemoveAll(tag => set.Contains(tag)); }
            _updated = false;
            return this;
        }

        public StaticTags GetStatic()
        {
            lock (_lock)
            {
                BuildFlatView();
                return new StaticTags(_flatView);
            }
        }

        public override string ToString()
        {
            if (_tags.Any())
                lock (_lock) { return '{' + string.Join(',', _tags) + '}'; }
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
#nullable enable
        protected Dictionary<string, ITags> _tagsTable = new();
        protected object _lock = new();

        public TagsManager() { }

        public void Set(string id, ITags tags)
        {
            lock(_lock) { _tagsTable[id] = tags; }
        }

        public bool Add(string id, ITags tags)
        {
            lock (_lock) { return _tagsTable.TryAdd(id, tags); }
        }

        //静态标签集合拒绝任何修改，但允许外部用户操作普通标签集合
        public bool AddorMerge(string id, ITags ntags)
        {
            lock (_lock)
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
        }

        public ITags? Get(string id)
        {
            lock (_lock)
            {
                if (_tagsTable.TryGetValue(id, out var tags))
                    return tags;
            }
            return null;
        }

        public bool Remove(string id, ITags ntags)
        {
            lock (_lock)
            {
                if (_tagsTable.TryGetValue(id, out var otags) && otags is Tags tags)
                {
                    tags.Exclude(ntags);
                    return true;
                }
            }
            return false;
        }

        public bool Delete(string id)
        {
            lock (_lock)
            {
                if (_tagsTable.TryGetValue(id, out var otags))
                {
                    _tagsTable.Remove(id);
                    return true;
                }
            }
            return false;
        }

        //目标标签集合的扁平化集合中至少包含src的一个完整子集
        public bool Match(string id, ITags src)
        {
            lock (_lock)
            {
                if (_tagsTable.TryGetValue(id, out var tags))
                    return src.Match(tags);
            }
            return false;
        }

        public bool Match(string id, string src)
        {
            lock (_lock)
            {
                if (_tagsTable.TryGetValue(id, out var tags))
                    return tags.Contains(src);
            }
            return false;
        }
    }
#nullable disable
}
