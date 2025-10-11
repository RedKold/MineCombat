using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MineCombat
{
    //通用数据存储格式，仅支持int，double，bool三种基础值类型，对string类型有优化，也能存储引用对象
    public class Properties
    {
#nullable enable
        internal readonly static Properties Default = new();

        private readonly static Dictionary<string, uint> _keys = new();
        private static uint next = 1;
        private static object _keyLock = new();

        private readonly Lazy<Dictionary<uint, int>> _ints = new(() => new Dictionary<uint, int>(), LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Dictionary<uint, double>> _doubles = new(() => new Dictionary<uint, double>(), LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Dictionary<uint, bool>> _bools = new(() => new Dictionary<uint, bool>(), LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Dictionary<uint, string>> _strings = new(() => new Dictionary<uint, string>(), LazyThreadSafetyMode.ExecutionAndPublication);
        private readonly Lazy<Dictionary<uint, object>> _objects = new(() => new Dictionary<uint, object>(), LazyThreadSafetyMode.ExecutionAndPublication);

        private object? _intLock;
        private object? _doubleLock;
        private object? _boolLock;
        private object? _stringLock;
        private object? _objectLock;

        private Dictionary<uint, int> Ints => _ints.Value;
        private Dictionary<uint, double> Doubles => _doubles.Value;
        private Dictionary<uint, bool> Bools => _bools.Value;
        private Dictionary<uint, string> Strings => _strings.Value;
        private Dictionary<uint, object> Objects => _objects.Value;
        private object IntLock => _intLock ??= new object();
        private object DoubleLock => _doubleLock ??= new object();
        private object BoolLock => _boolLock ??= new object();
        private object StringLock => _stringLock ??= new object();
        private object ObjectLock => _objectLock ??= new object();

        internal Properties() { }

        private static uint Translate(string name, bool add = false)
        {
            lock (_keyLock)
            {
                uint result = _keys.TryGetValue(name, out var key) ? key : 0;
                if (add && result == 0)
                {
                    _keys.Add(name, next);
                    result = next++;
                }
                return result;
            }
        }

        //若属性名已存在，返回假，不可用于更新属性的值
        public bool Store(string name, int i)
        {
            lock (IntLock)
            {
                uint id = Translate(name, true);
                if (Ints.ContainsKey(id))
                    return false;
                Ints[id] = i;
                return true;
            }
        }
        public bool Store(string name, double d)
        {
            lock (DoubleLock)
            {
                uint id = Translate(name, true);
                if (Doubles.ContainsKey(id))
                    return false;
                Doubles[id] = d;
                return true;
            }
        }
        public bool Store(string name, bool b)
        {
            lock (BoolLock)
            {
                uint id = Translate(name, true);
                if (Bools.ContainsKey(id))
                    return false;
                Bools[id] = b;
                return true;
            }
        }
        public bool Store(string name, string s)
        {
            lock (StringLock)
            {
                uint id = Translate(name, true);
                if (Strings.ContainsKey(id))
                    return false;
                Strings[id] = s;
                return true;
            }
        }
        public bool Store<T>(string name, T t) where T : notnull
        {
            lock (ObjectLock)
            {
                uint id = Translate(name, true);
                if (Objects.ContainsKey(id))
                    return false;
                Objects[id] = t;
                return true;
            }
        }

        //若属性名不存在，返回假，但仍会创建属性并赋值
        public bool Update(string name, int i)
        {
            lock (IntLock)
            {
                uint id = Translate(name, true);
                bool result = Ints.ContainsKey(id);
                Ints[id] = i;
                return result;
            }
        }
        public bool Update(string name, double d)
        {
            lock (DoubleLock)
            {
                uint id = Translate(name, true);
                bool result = Doubles.ContainsKey(id);
                Doubles[id] = d;
                return result;
            }
        }
        public bool Update(string name, bool b)
        {
            lock (BoolLock)
            {
                uint id = Translate(name, true);
                bool result = Bools.ContainsKey(id);
                Bools[id] = b;
                return result;
            }
        }
        public bool Update(string name, string s)
        {
            lock (StringLock)
            {
                uint id = Translate(name, true);
                bool result = Strings.ContainsKey(id);
                Strings[id] = s;
                return result;
            }
        }
        public bool Update<T>(string name, T t) where T : notnull
        {
            lock (ObjectLock)
            {
                uint id = Translate(name, true);
                bool result = Strings.ContainsKey(id);
                Objects[id] = t;
                return result;
            }
        }

        //若属性名对应的属性不存在，默认尝试赋予并返回默认值（功能可关闭），如果没有默认值，返回null
        private int? GetInt(uint id, bool checkDefault = true)
        {
            lock (IntLock)
            {
                if (Ints.TryGetValue(id, out var o))
                    return o;

                if (!checkDefault)
                    return null;

                int? i = Default.GetInt(id, false);
                if (i.HasValue)
                    Ints[id] = i.Value;
                return i;
            }
        }
        public int? GetInt(string name, bool checkDefault = true)
        {
            uint id = Translate(name);
            return id == 0 ? null : GetInt(id, checkDefault);
        }
        private double? GetDouble(uint id, bool checkDefault = true)
        {
            lock (DoubleLock)
            {
                if (Doubles.TryGetValue(id, out var o))
                    return o;

                if (!checkDefault)
                    return null;

                double? d = Default.GetDouble(id, false);
                if (d.HasValue)
                    Doubles[id] = d.Value;
                return d;
            }
        }
        public double? GetDouble(string name, bool checkDefault = true)
        {
            uint id = Translate(name);
            return id == 0 ? null : GetDouble(id, checkDefault);
        }
        private bool? GetBool(uint id, bool checkDefault = true)
        {
            lock (BoolLock)
            {
                if (Bools.TryGetValue(id, out var o))
                    return o;

                if (!checkDefault)
                    return null;

                bool? b = Default.GetBool(id, false);
                if (b.HasValue)
                    Bools[id] = b.Value;
                return b;
            }
        }
        public bool? GetBool(string name, bool checkDefault = true)
        {
            uint id = Translate(name);
            return id == 0 ? null : GetBool(id, checkDefault);
        }
        private string? GetString(uint id, bool checkDefault = true)
        {
            lock (StringLock)
            {
                if (Strings.TryGetValue(id, out var o))
                    return o;

                if (!checkDefault)
                    return null;

                string? s = Default.GetString(id, false);
                if (s is not null)
                    Strings[id] = s;
                return s;
            }
        }
        public string? GetString(string name, bool checkDefault = true)
        {
            uint id = Translate(name);
            return id == 0 ? null : GetString(id, checkDefault);
        }
        /* 只能用于获取引用类型的值
         * 用于获取基础类型的值时，会返回c#的默认值，使用GetInt等函数获取基础类型的值 */
        private T? Get<T>(uint id, bool checkDefault = true) where T : notnull
        {
            lock (ObjectLock)
            {
                if (Objects.TryGetValue(id, out object? obj) && obj is T tobj)
                    return tobj;

                if (!checkDefault)
                    return default(T);

                T? t = Default.Get<T>(id, false);
                if (t != null)
                    Objects[id] = t;
                return t;
            }
        }
        public T? Get<T>(string name, bool checkDefault = true) where T : notnull
        {
            uint id = Translate(name);
            return id == 0 ? default : Get<T>(id, checkDefault);
        }

        //若属性名不存在，返回假
        public bool Change(string name, Process<int> processer)
        {
            lock (IntLock)
            {
                uint id = Translate(name);
                if (id == 0 || !Ints.ContainsKey(id))
                    return false;
                int i = Ints[id];
                processer(ref i);
                Ints[id] = i;
                return true;
            }
        }
        public bool Change(string name, Process<double> processer)
        {
            lock (DoubleLock)
            {
                uint id = Translate(name);
                if (id == 0 || !Doubles.ContainsKey(id))
                    return false;
                double d = Doubles[id];
                processer(ref d);
                Doubles[id] = d;
                return true;
            }
        }
        public bool Change(string name, Process<bool> processer)
        {
            lock (BoolLock)
            {
                uint id = Translate(name);
                if (id == 0 || !Bools.ContainsKey(id))
                    return false;
                bool b = Bools[id];
                processer(ref b);
                Bools[id] = b;
                return true;
            }
        }
        public bool Change(string name, Process<string> processer)
        {
            lock (StringLock)
            {
                uint id = Translate(name);
                if (id == 0 || !Strings.ContainsKey(id))
                    return false;
                string s = Strings[id];
                processer(ref s);
                Strings[id] = s;
                return true;
            }
        }
        /* 只能用于修改引用类型的值，传递引用，可以重新赋值
         * 用于修改基础类型的值时，返回假 */
        public bool Change<T>(string name, Process<T> processer) where T : notnull
        {
            lock (ObjectLock)
            {
                uint id = Translate(name);
                if (id != 0 && Objects.TryGetValue(id, out object? obj) && obj is T t)
                {
                    processer(ref t);
                    Objects[id] = t;
                    return true;
                }
                return false;
            }
        }

        static Properties()
        {
            //读文件初始化Default
        }
#nullable disable
    }
}
