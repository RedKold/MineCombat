using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineCombat
{
    //通用数据存储格式，仅支持int，double，bool三种基础值类型，对string类型有优化，也能存储引用对象
    public class Properties
    {
#nullable enable
        internal static Properties Default = new();

        private readonly Lazy<Dictionary<string, int>> _ints = new (() => new Dictionary<string, int>());
        private readonly Lazy<Dictionary<string, double>> _doubles = new(() => new Dictionary<string, double>());
        private readonly Lazy<Dictionary<string, bool>> _bools = new(() => new Dictionary<string, bool>());
        private readonly Lazy<Dictionary<string, string>> _strings = new(() => new Dictionary<string, string>());
        private readonly Lazy<Dictionary<string, object>> _objects = new(() => new Dictionary<string, object>());

        private object? _intLock;
        private object? _doubleLock;
        private object? _boolLock;
        private object? _stringLock;
        private object? _objectLock;

        private Dictionary<string, int> Ints => _ints.Value;
        private Dictionary<string, double> Doubles => _doubles.Value;
        private Dictionary<string, bool> Bools => _bools.Value;
        private Dictionary<string, string> Strings => _strings.Value;
        private Dictionary<string, object> Objects => _objects.Value;
        private object IntLock => _intLock ??= new object();
        private object DoubleLock => _doubleLock ??= new object();
        private object BoolLock => _boolLock ??= new object();
        private object StringLock => _stringLock ??= new object();
        private object ObjectLock => _objectLock ??= new object();

        internal Properties() { }

        //若属性名已存在，返回假，不可用于更新属性的值
        public bool Store(string id, int i)
        {
            lock (IntLock)
            {
                if (Ints.ContainsKey(id))
                    return false;
                Ints[id] = i;
                    return true;
            }
        }
        public bool Store(string id, double d)
        {
            lock (DoubleLock)
            {
                if (Doubles.ContainsKey(id))
                    return false;
                Doubles[id] = d;
                return true;
            }
        }
        public bool Store(string id, bool b)
        {
            lock (BoolLock)
            {
                if (Bools.ContainsKey(id))
                    return false;
                Bools[id] = b;
                return true;
            }
        }
        public bool Store(string id, string s)
        {
            lock (StringLock)
            {
                if (Strings.ContainsKey(id))
                    return false;
                Strings[id] = s;
                return true;
            }
        }
        public bool Store<T>(string id, T t) where T : notnull
        {
            lock (ObjectLock)
            {
                if (Objects.ContainsKey(id))
                    return false;
                Objects[id] = t;
                return true;
            }
        }

        //若属性名不存在，返回假，但仍会创建属性并赋值
        public bool Update(string id, int i)
        {
            lock (IntLock)
            {
                bool result = Ints.ContainsKey(id);
                Ints[id] = i;
                return result;
            }
        }
        public bool Update(string id, double d)
        {
            lock (DoubleLock)
            {
                bool result = Doubles.ContainsKey(id);
                Doubles[id] = d;
                return result;
            }
        }
        public bool Update(string id, bool b)
        {
            lock (BoolLock)
            {
                bool result = Bools.ContainsKey(id);
                Bools[id] = b;
                return result;
            }
        }
        public bool Update(string id, string s)
        {
            lock (StringLock)
            {
                bool result = Strings.ContainsKey(id);
                Strings[id] = s;
                return result;
            }
        }
        public bool Update<T>(string id, T t) where T : notnull
        {
            lock (ObjectLock)
            {
                bool result = Strings.ContainsKey(id);
                Objects[id] = t;
                return result;
            }
        }

        //若属性名对应的属性不存在，默认尝试赋予并返回默认值（功能可关闭），如果没有默认值，返回null
        public int? GetInt(string id, bool checkDefault = true)
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
        public double? GetDouble(string id, bool checkDefault = true)
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
        public bool? GetBool(string id, bool checkDefault = true)
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
        public string? GetString(string id, bool checkDefault = true)
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
        /* 只能用于获取引用类型的值
         * 用于获取基础类型的值时，会返回c#的默认值，使用GetInt等函数获取基础类型的值 */
        public T? Get<T>(string id, bool checkDefault = true) where T : notnull
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

        //若属性名不存在，返回假
        public bool Change(string id, Process<int> processer)
        {
            lock (IntLock)
            {
                if (!Ints.ContainsKey(id))
                    return false;
                int i = Ints[id];
                processer(ref i);
                Ints[id] = i;
                return true;
            }
        }
        public bool Change(string id, Process<double> processer)
        {
            lock (DoubleLock)
            {
                if (!Doubles.ContainsKey(id))
                    return false;
                double d = Doubles[id];
                processer(ref d);
                Doubles[id] = d;
                return true;
            }
        }
        public bool Change(string id, Process<bool> processer)
        {
            lock (BoolLock)
            {
                if (!Bools.ContainsKey(id))
                    return false;
                bool b = Bools[id];
                processer(ref b);
                Bools[id] = b;
                return true;
            }
        }
        public bool Change(string id, Process<string> processer)
        {
            lock (StringLock)
            {
                if (!Strings.ContainsKey(id))
                    return false;
                string s = Strings[id];
                processer(ref s);
                Strings[id] = s;
                return true;
            }
        }
        /* 只能用于修改引用类型的值，传递引用，可以重新赋值
         * 用于修改基础类型的值时，返回假 */
        public bool Change<T>(string id, Process<T> processer) where T : notnull
        {
            lock (ObjectLock)
            {
                if (Objects.TryGetValue(id, out object? obj) && obj is T t)
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
