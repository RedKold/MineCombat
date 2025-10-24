using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public class Box<T>
    {
#nullable enable
        private T? _value;
        private T[]? _values;

        public Box()
        {
            _value = default;
            _values = null;
        }

        public Box(T value)
        {
            _value = value;
            _values = null;
        }

        public Box(params T[] values)
        {
            if (values.Length == 0)
            {
                _value = default;
                _values = null;
            }
            else if (values.Length == 1)
            {
                _value = values[0];
                _values = null;
            }
            else
            {
                _value = default;
                _values = values;
            }
        }

        public void ClearContent()
        {
            _value = default;
            _values = null;
        }

        public void UpdateContent(T value)
        {
            _value = value;
            _values = null;
        }

        public void UpdateContent(params T[] values)
        {
            _value = default;
            _values = values;
        }

        public void ForEach(Action<T> action)
        {
            if (_value is not null)
                action(_value);
            else if (_values is not null)
                for (int i = 0; i < _values.Length; i++)
                {
                    action(_values[i]);
                }
        }

        public void GetContent(out T? value, out T[]? values)
        {
            value = _value;
            values = _values;
        }
#nullable disable
        public static implicit operator Box<T>(T value) => new Box<T>(value);
        public static implicit operator Box<T>(T[] values) => new Box<T>(values);
    }

    public class Slots<T> where T : class
    {
        private T[] _slots;
        private uint _count;
        private Queue<int> _empties;

        public int Count => (int)_count - _empties.Count;
        public int Capacity => _slots.Length;

        public Slots(int capacity)
        {
            _slots = new T[capacity];
            _count = 0;
            _empties = new ();
        }

        public Slots(uint capacity) : this ((int)capacity) { }

        public T this[int i] => _slots[i];
        public T this[uint u] => _slots[u];

        public int TryUpdateCapacity(int capacity)
        {
            if (Count > capacity)
                return Count - capacity;

            T[] slots = new T[capacity];

            if (_empties.Count == 0)
                Array.Copy(_slots, slots, _count);
            else
            {
                uint index = 0;
                for (int i = 0; i < _slots.Length; i++)
                {
                    if (_slots[i] is not null)
                        slots[index++] = _slots[i];
                }
                _count = index;
                _empties.Clear();
            }
            _slots = slots;
            return 0;
        }

        public bool Add(T item)
        {
            if (_empties.Count > 0)
            {
                _slots[_empties.Dequeue()] = item;
                return true;
            }

            if (_count >= _slots.Length)
                return false;

            _slots[_count++] = item;
            return true;
        }

        public bool Remove(int i)
        {
            if (i < 0 || i >= _count) 
                return false;

            if (i == _count - 1)
            {
                _slots[--_count] = null;
                return true;
            }

            _slots[i] = null;
            _empties.Enqueue(i);
            return true;
        }

        public void ForEach(Action<T, uint> action)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_slots[i] is not null)
                    action(_slots[i], (uint)i);
            }
        }

        public T[] GetContent(bool ignoreNull = true)
        {
            T[] result;
            if (!ignoreNull)
            {
                result = new T[_count];
                Array.Copy(_slots, result, _count);
                return result;
            }

            result = new T[Count];
            uint index = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] is not null)
                    result[index++] = _slots[i];
            }
            return result;
        }
    }
}
