using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public interface IRandomizer<T>
    {
        T RandomlyGet();

        T[] GetItems();

        uint[] GetWeights();

        void GetContent(out T[] items, out uint[] weights);
    }

    public class DynamicRandomizer<T> : IRandomizer<T>, IConstable<ConstRandomizer<T>> where T : notnull
    {
        private static Random random = new();
        private static object _lock = new();

        private List<(T, uint)> _items;
        private long _total;

        public DynamicRandomizer()
        {
            _items = new();
            _total = 0;
        }

        public DynamicRandomizer(T[] items, uint[] weights)
        {
            int count = items.Length;

            if (count != weights.Length)
                throw new ArgumentException("项数组与权重数组长度不相等");

            _total = 0;
            _items = new(count);

            for (int i = 0; i < count; i++)
            {
                _total += weights[i];
                _items[i] = (items[i], weights[i]);
            }
        }

        protected void Next(out long l)
        {
            lock (_lock)
            {
                l = random.NextInt64(0, _total);
            }
        }

        public T RandomlyGet()
        {
            Next(out long l);
            long total = 0;
            foreach(var item in _items)
            {
                total += item.Item2;
                if (l < total)
                    return item.Item1;
            }
            throw new ArgumentException("尝试从动态随机器中获取值时发生异常");
        }

        public T[] GetItems()
        {
            int count = _items.Count;
            T[] result = new T[count];
            for (int i = 0; i< count; i++)
            {
                result[i] = _items[i].Item1;
            }
            return result;
        }

        public uint[] GetWeights()
        {
            int count = _items.Count;
            uint[] result = new uint[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = _items[i].Item2;
            }
            return result;
        }

        public void GetContent(out T[] items, out uint[] weights)
        {
            int count = _items.Count;
            items = new T[count];
            weights = new uint[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = _items[i].Item1;
                weights[i] = _items[i].Item2;
            }
        }

        public void Add(T item, uint weight)
        {
            _items.Add((item, weight));
            _total += weight;
        }

        public void Remove(T item)
        {
            _items.RemoveAll(x => 
            {
                if (!item.Equals(x.Item1))
                    return false;
                _total -= x.Item2;
                return true;
            });
        }

        public void RemoveAt(int index)
        {
            _total -= _items[index].Item2;
            _items.RemoveAt(index);
        }

        public ConstRandomizer<T> ConstCast()
        {
            GetContent(out T[] items, out uint[] weights);
            return new ConstRandomizer<T>(items, weights);
        }
    }

    public class ConstRandomizer<T> : IRandomizer<T>, IConstable<ConstRandomizer<T>> where T : notnull
    {
        private static Random random = new();
        private static object _lock = new();

        private T[] _items;
        private int[] _alias;
        private long[] _weights;
        private readonly long _avg;
        private readonly int _count;

        public ConstRandomizer(T[] items, uint[] weights)
        {
            _count = items.Length;

            if (_count != weights.Length)
                throw new InvalidDataException("项数组与权重数组长度不相等");

            if (_count == 0)
                throw new InvalidDataException("不能使用空数组初始化");

            _items = items;
            _alias = new int[_count];
            _weights = new long[_count];
            Stack<int> majors = new();
            Stack<int> minors = new();

            long[] lweights = Array.ConvertAll(weights, x => (long)x * _count);
            _avg = lweights.Sum(x => x) / _count;
            for (int i = 0; i < _count; i++)
            {
                if (lweights[i] > _avg)
                    majors.Push(i);
                else if (lweights[i] < _avg) minors.Push(i);
                else _weights[i] = 1;
            }

            while (majors.Count > 0 && minors.Count > 0)
            {
                int minor = minors.Pop();
                int major = majors.Pop();

                _alias[minor] = major;
                _weights[minor] = lweights[minor];
                lweights[major] = lweights[major] + lweights[minor] - _avg;

                if (lweights[major] > _avg)
                    majors.Push(major);
                else if (lweights[major] < _avg)
                    minors.Push(major);
                else _weights[major] = _avg;
            }
        }

        protected void Next(out int i, out long l)
        {
            lock (_lock)
            {
                i = random.Next(0, _count);
                l = random.NextInt64(0, _avg);
            }
        }

        public T RandomlyGet()
        {
            Next(out int i, out long l);
            return l < _weights[i] ? _items[i] : _items[_alias[i]];
        }

        public T[] GetItems()
        {
            T[] result = new T[_count];
            Array.Copy(_items, result, _count);
            return result;
        }

        public uint[] GetWeights()
        {
            return Array.ConvertAll(_weights, x => (uint)(x / _count));
        }

        public void GetContent(out T[] items, out uint[] weights)
        {
            items = new T[_count];
            Array.Copy(_items, items, _count);
            weights = Array.ConvertAll(_weights, x => (uint)(x / _count));
        }

        public ConstRandomizer<T> ConstCast()
        {
            return this;
        }
    }

    public static class Randomizer
    {
        //暂时无法使用此功能，未完成开发
        public static DynamicRandomizer<T> Cross<T>(IRandomizer<T> a, IRandomizer<T> b, bool uniWeight = true) where T : IEquatable<T>
        {
            a.GetContent(out T[] a_items, out uint[] a_weights);
            b.GetContent(out T[] b_items, out uint[] b_weights);
            int a_len = a_items.Length;
            int b_len = b_items.Length;
            if (a_len == 0 || b_len == 0)
                throw new ArgumentException("尝试交叉空随机器");
            return new DynamicRandomizer<T>(a_items, b_weights);
        }
    }
}
