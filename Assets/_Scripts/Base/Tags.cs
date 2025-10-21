using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineCombat
{
    /* 注意：
     * 1.new Tags("...") != (Tags)"..."，后者会尝试解析字符串为一个集合，前者只会将字符串整体作为第一个ITag，对StaticTags同理
     * 2.(Tags)"" == new Tags("") != new Tags()，想要使用空的ITags，推荐使用StaticTags.Empty，可以有效节省开销 */
    public interface IMatchable<T>
    {
        bool Match(T t);
    }

    public interface ITag : IEquatable<ITag>, IMatchable<ITags>
    {
        bool Contains(string tag);
        internal void JoinSet(HashSet<uint> set);
    }

    public interface ITags : IConstable<ConstTags>, IEquatable<ITags>, IMatchable<ITags>
    {
#nullable enable
        internal bool Contains(uint key);
        bool Contains(string tag);
        internal IReadOnlyList<ITag>? ToArray();
#nullable disable
    }

    abstract public class ATag
    {
#nullable enable
        protected static KeyTranslator<string> _translator = new(97, true);

        protected static uint[] Translate(string[] tags)
        {
            uint[] result = new uint[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                result[i] = _translator.Translate(tags[i]);
            }
            Array.Sort(result);
            return result;
        }

#pragma warning disable CS8602
        protected static bool SequenceEqual(uint[]? x, uint[]? y)
        {
            bool a = x == null;
            bool b = y == null;
            if (a || b)
                return a && b;
            if (x.Length != y.Length)
                return false;

            return x.SequenceEqual(y);
        }
#pragma warning restore CS8602
#nullable disable
    }

    public sealed class Tag : ATag, ITag
    {
#nullable enable
        private uint tag;

        internal Tag(uint tag, bool strict = true)
        {
            if (!strict || _translator.IsValid(tag))
                this.tag = tag;
            else
                throw new ArgumentException($"不合法的转译键：{tag}");
        }
        public Tag(string tag)
        {
            this.tag = _translator.Translate(tag);
        }

        public bool Equals(ITag? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is Tag otherTag)
                return tag == otherTag.tag;
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
            return this.tag == _translator.Translate(tag, false);
        }

        public bool Match(ITags tags)
        {
            return tags.Contains(tag);
        }

        void ITag.JoinSet(HashSet<uint> set)
        {
            set.Add(tag);
        }

        public override int GetHashCode()
        {
            return int.MaxValue - (int)tag;
        }

        public static implicit operator Tag(string tag) => new Tag(tag);
#nullable disable
    }

    public sealed class TagSet : ATag, ITag
    {
#nullable enable
        private uint[]? _tags;

        public TagSet(IEnumerable<string>? tags = null)
        {
            _tags = tags?.Any() == true ? Translate(tags.ToArray()) : null;
        }

        public TagSet(params string[] tags)
        {
            _tags = Translate(tags);
        }

        public bool Equals(ITag? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is TagSet otherTag)
                return SequenceEqual(_tags, otherTag._tags);
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
            if (_tags is null)
                return false;
            return Array.BinarySearch(_tags, _translator.Translate(tag, false)) >= 0;
        }

        public bool Match(ITags tags)
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

        void ITag.JoinSet(HashSet<uint> set)
        {
            if (_tags is not null)
            {
                foreach (var tag in _tags)
                {
                    set.Add(tag);
                }
            }
        }

        public override int GetHashCode()
        {
            if (_tags is null) return 0;

            unchecked
            {
                int hash = 17;
                foreach (int tag in _tags)
                {
                    hash = hash * 31 + (int)tag;
                }
                return hash;
            }
        }

        public static implicit operator TagSet(string[] tags) => new TagSet(tags);
#nullable disable
    }

    

    //静态标签集合可空，内部结构简单，不能通过ITag初始化，发起匹配时只能“或”
    public class ConstTags : ATag, ITags
    {
        public static readonly ConstTags Empty = new ConstTags();

#nullable enable
        private uint[]? _tags;

        internal ConstTags(uint[] tags, bool strict = true)
        {
            if (tags.Length == 0)
                _tags = null;
            else
            {
                if (strict)
                    foreach (var tag in tags)
                    {
                        if (!_translator.IsValid(tag))
                            throw new ArgumentException($"不合法的转译键：{tag}");
                    }
                _tags = new uint[tags.Length];
                Array.Copy(_tags, tags, tags.Length);
                Array.Sort(_tags);
            }
        }

        public ConstTags(params string[] tags)
        {
            _tags = Translate(tags);
        }

        public ConstTags(IEnumerable<string>? tags = null)
        {
            _tags = tags?.Any() == true ? Translate(tags.ToArray()) : null;
        }

        public bool Equals(ITags? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is ConstTags otherTags)
                return SequenceEqual(_tags, otherTags._tags);
            return false;
        }

        bool ITags.Contains(uint key)
        {
            if (_tags is null)
                return false;
            return Array.BinarySearch(_tags, key) >= 0;
        }

        public bool Contains(string tag)
        {
            if (_tags is null)
                return false;
            return Array.BinarySearch(_tags, _translator.Translate(tag, false)) >= 0;
        }

        public bool Match(ITags tags)
        {
            if (_tags is not null)
            {
                foreach (var tag in _tags)
                {
                    if (tags.Contains(tag))
                        return true;
                }
            }
            return false;
        }

        IReadOnlyList<ITag>? ITags.ToArray()
        {
            if (_tags is null)
                return null;

            int len = _tags.Length;
            ITag[] result = new ITag[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = new Tag(_tags[i], false);
            }
            return result;
        }

        public ConstTags ConstCast()
        {
            return this;
        }

        public static implicit operator ConstTags(string[] tags) => new ConstTags(tags);
        public static implicit operator ConstTags(string tags)
        {
            IEnumerable<object>? collection = Parser.ToCollection(tags, 1);
            if (collection?.Any() == true)
            {
                List<string> strings = new();
                foreach (var item in collection)
                {
                    if (item is string str)
                        strings.Add(str);
                    else return new ConstTags(tags);
                }
                return new ConstTags(strings);
            }
            return new ConstTags(tags);
        }
#nullable disable
    }

    //普通标签集合支持“与”、“或”匹配
    public class Tags : ATag, ITags
    {
#nullable enable
        private List<ITag> _tags;
        private HashSet<uint> _flatView;
        private bool _updated;
        protected object _lock = new();

        private void BuildFlatView()
        {
            if (_updated) 
                return;

            _flatView.Clear();
            foreach (var tag in _tags)
            {
                tag.JoinSet(_flatView);
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
                        result.Add(new TagSet(collection));
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

        bool ITags.Contains(uint tag)
        {
            lock (_lock)
            {
                BuildFlatView();
                return _flatView.Contains(tag);
            }
        }

        public bool Contains(string tag)
        {
            lock (_lock)
            {
                BuildFlatView();
                return _flatView.Contains(_translator.Translate(tag, false));
            }
        }

        public bool Match(ITags tags)
        {
            lock (_lock)
            {
                foreach (var tag in _tags)
                {
                    if (tag?.Match(tags) == true)
                        return true;
                }
                return false;
            }
        }

        IReadOnlyList<ITag> ITags.ToArray()
        {
            lock (_lock) { return _tags.ToArray(); }
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
                        tag.JoinSet(_flatView);
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
            var array = other.ToArray();
            if (array is not null)
            {
                lock (_lock)
                {
                    HashSet<ITag> set = new(_tags);
                    foreach (var tag in array)
                    {
                        if (!set.Contains(tag))
                            _tags.Add(tag);
                    }
                }
                _updated = false;
            }
            return this;
        }
        public Tags Exclude(ITags other)
        {
            var array = other.ToArray();
            if (array is not null)
            {
                lock (_lock)
                {
                    HashSet<ITag> set = new(array);
                    _tags.RemoveAll(tag => set.Contains(tag));
                }
                _updated = false;
            }
            return this;
        }

        public ConstTags ConstCast()
        {
            return new ConstTags(_flatView.ToArray(), false);
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
