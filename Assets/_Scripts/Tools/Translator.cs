using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MineCombat
{
    public interface ITranslator<K, V>
    {
        V Translate(K key);
        bool IsValid(V value);
    }

    public interface IReversableTranslator<K, V> : ITranslator<K, V>
    {
        K Translate(V value);
        bool IsValid(K key);
    }

    public class KeyTranslator<T> : ITranslator<T, uint> where T : notnull
    {
        private Dictionary<T, uint> _keys;
        private uint _nextValue = 1;
        private object _lock = new();
        private bool _add;

        public KeyTranslator(uint capacity, bool add = false)
        {
            _keys = new((int)capacity);
            _add = add;
        }

        public uint Translate(T key)
        {
            return Translate(key, _add);
        }

        public uint Translate(T key, bool add)
        {
            if (_keys.TryGetValue(key, out var value))
                return value;

            if (!add)
                return 0;

            lock (_lock)
            {
                if (_keys.TryGetValue(key, out value))
                    return value;

                _keys.Add(key, _nextValue++);
                return _nextValue;
            }
        }

        public bool IsValid(uint value)
        {
            return value > 0 && value < _nextValue;
        }
    }

    public class KeyValueTranslator<T> : IReversableTranslator<T, uint> where T : notnull
    {
        private Dictionary<T, uint> _keys;
        private Dictionary<uint, T> _values;
        private uint _nextValue = 1;
        private object _lock = new();
        private bool _add;

        public KeyValueTranslator(uint capacity, bool add = false)
        {
            _keys = new((int)capacity);
            _values = new((int)capacity);
            _add = add;
        }

        public uint Translate(T key)
        {
            return Translate(key, _add);
        }

        public uint Translate(T key, bool add)
        {
            if (_keys.TryGetValue(key, out var value))
                return value;

            if (!add)
                return 0;

            lock (_lock)
            {
                if (_keys.TryGetValue(key, out value))
                    return value;

                _values.Add(_nextValue, key);
                _keys.Add(key, _nextValue++);
                return _nextValue;
            }
        }

        public bool IsValid(uint value)
        {
            return value > 0 && value < _nextValue;
        }

        public T Translate(uint value)
        {
            if (_values.TryGetValue(value, out var key))
                return key;
            throw new ArgumentException($"尝试转译不合法的值：{value}");
        }

        public bool IsValid(T key)
        {
            return _keys.ContainsKey(key);
        }
    }

    public static class TupleTranslators
    {
        internal class Worker<T>
        {
            private IOrderedEnumerable<FieldInfo> _fields;
            private readonly string _parasName;

            public Worker()
            {
                _fields = typeof(T).GetFields().Where(f => f.Name.StartsWith("Item")).OrderBy(f => f.Name);
                _parasName = string.Join(',', _fields.Select(f => f.FieldType.Name));
            }

            public Action<T> Translate(Delegate dlg)
            {
                if (_fields is not null)
                {
                    var paras = dlg.Method.GetParameters();
                    if (paras.Length == _fields.ToArray().Length && paras.Select(p => p.ParameterType).SequenceEqual(_fields.Select(f => f.FieldType)))
                        return tuple => { dlg.Method.Invoke(dlg.Target, _fields.Select(f => f.GetValue(tuple)).ToArray()); };
                }
                throw new ArgumentException($"需要参数为{_parasName}的无返回值函数");
            }
        }

        //用于转译多个参数为单个元组参数
        private class Translator : ITranslator<Type, object>
        {
            private ConcurrentDictionary<Type, object> _cache;
            private object _lock = new();

            public Translator()
            {
                _cache = new(Environment.ProcessorCount, 100);
            }

            public object Translate(Type type)
            {
                if (_cache.TryGetValue(type, out var worker))
                    return worker;
                return _cache.GetOrAdd(type, w =>
                {
                    lock (_lock) 
                    {
                        Type targetType = typeof(Worker<>).MakeGenericType(type);
                        return Activator.CreateInstance(targetType) ?? throw new ArgumentException($"无法创建目标类型为元组（{string.Join(',', type.GetFields().Where(f => f.Name.StartsWith("Item")).OrderBy(f => f.Name).Select(f => f.FieldType.Name))}）的类型转译器");
                    }
                });
            }

            public bool IsValid(object worker)
            {
                return false;
            }
        }

        private static Translator _translator = new();

#nullable enable
        internal static Worker<T>? GetTranslator<T>()
        {
            Type type = typeof(T);

            bool isTuple = false;
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                isTuple = genericType == typeof(Tuple<>) ||
                          genericType == typeof(Tuple<,>) ||
                          genericType == typeof(Tuple<,,>) ||
                          genericType == typeof(Tuple<,,,>) ||
                          genericType == typeof(Tuple<,,,,>) ||
                          genericType == typeof(Tuple<,,,,,>) ||
                          genericType == typeof(Tuple<,,,,,,>) ||
                          genericType == typeof(Tuple<,,,,,,,>) ||
                          genericType.FullName?.StartsWith("System.ValueTuple") == true;
            }

            return isTuple ? (Worker<T>)_translator.Translate(typeof(T)) : null;
        }

        public static Action<T>? TryTranslate<T>(Delegate dlg)
        {
            Worker<T>? worker = GetTranslator<T>();
            return worker?.Translate(dlg);
        }
#nullable disable
    }
}
