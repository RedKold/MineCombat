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

        private Dictionary<string, int> _ints = new();
        private Dictionary<string, double> _doubles = new();
        private Dictionary<string, bool> _bools = new();
        private Dictionary<string, string> _strings = new();
        private Dictionary<string, object> _objects = new();

        internal Properties() { }

        //若属性名已存在，返回假，不可用于更新属性的值
        public bool Store(string id, int i)
        {
            if (_ints.ContainsKey(id))
                return false;
            _ints[id] = i;
            return true;
        }
        public bool Store(string id, double d)
        {
            if (_doubles.ContainsKey(id))
                return false;
            _doubles[id] = d;
            return true;
        }
        public bool Store(string id, bool b)
        {
            if (_bools.ContainsKey(id))
                return false;
            _bools[id] = b;
            return true;
        }
        public bool Store(string id, string s)
        {
            if (_strings.ContainsKey(id))
                return false;
            _strings[id] = s;
            return true;
        }
        public bool Store<T>(string id, T t) where T : notnull
        {
            if (_objects.ContainsKey(id))
                return false;
            _objects[id] = t;
            return true;
        }

        //若属性名不存在，返回假，但仍会创建属性并赋值
        public bool Update(string id, int i)
        {
            bool result = _ints.ContainsKey(id);
            _ints[id] = i;
            return result;
        }
        public bool Update(string id, double d)
        {
            bool result = _doubles.ContainsKey(id);
            _doubles[id] = d;
            return result;
        }
        public bool Update(string id, bool b)
        {
            bool result = _bools.ContainsKey(id);
            _bools[id] = b;
            return result;
        }
        public bool Update(string id, string s)
        {
            bool result = _strings.ContainsKey(id);
            _strings[id] = s;
            return result;
        }
        public bool Update<T>(string id, T t) where T : notnull
        {
            bool result = _strings.ContainsKey(id);
            _objects[id] = t;
            return result;
        }

        //若属性名对应的属性不存在，默认尝试赋予并返回默认值（功能可关闭），如果没有默认值，返回null
        public int? GetInt(string id, bool checkDefault = true)
        {
            if (_ints.TryGetValue(id, out var o))
                return o;

            if (!checkDefault)
                return null;

            int? i = Default.GetInt(id, false);
            if (i.HasValue)
                _ints[id] = i.Value;
            return i;
        }
        public double? GetDouble(string id, bool checkDefault = true)
        {
            if (_doubles.TryGetValue(id, out var o))
                return o;

            if (!checkDefault)
                return null;

            double? d = Default.GetDouble(id, false);
            if (d.HasValue)
                _doubles[id] = d.Value;
            return d;
        }
        public bool? GetBool(string id, bool checkDefault = true)
        {
            if (_bools.TryGetValue(id, out var o))
                return o;

            if (!checkDefault)
                return null;

            bool? b = Default.GetBool(id, false);
            if (b.HasValue)
                _bools[id] = b.Value;
            return b;
        }
        public string? GetString(string id, bool checkDefault = true)
        {
            if (_strings.TryGetValue(id, out var o))
                return o;

            if (!checkDefault)
                return null;

            string? s = Default.GetString(id, false);
            if (s is not null)
                _strings[id] = s;
            return s;
        }
        /* 只能用于获取引用类型的值
         * 用于获取基础类型的值时，会返回c#的默认值，使用GetInt等函数获取基础类型的值 */
        public T? Get<T>(string id, bool checkDefault = true) where T : notnull
        {
            if (_objects.TryGetValue(id, out object? obj) && obj is T tobj)
                return tobj;

            if (!checkDefault)
                return default(T);

            T? t = Default.Get<T>(id, false);
            if (t != null)
                _objects[id] = t;
            return t;
        }

        //若属性名不存在，返回假
        public bool Change(string id, Process<int> processer)
        {
            if (!_ints.ContainsKey(id))
                return false;
            int i = _ints[id];
            processer(ref i);
            _ints[id] = i;
            return true;
        }
        public bool Change(string id, Process<double> processer)
        {
            if (!_doubles.ContainsKey(id))
                return false;
            double d = _doubles[id];
            processer(ref d);
            _doubles[id] = d;
            return true;
        }
        public bool Change(string id, Process<bool> processer)
        {
            if (!_bools.ContainsKey(id))
                return false;
            bool b = _bools[id];
            processer(ref b);
            _bools[id] = b;
            return true;
        }
        public bool Change(string id, Process<string> processer)
        {
            if (!_strings.ContainsKey(id))
                return false;
            string s = _strings[id];
            processer(ref s);
            _strings[id] = s;
            return true;
        }
        /* 只能用于修改引用类型的值，传递引用，可以重新赋值
         * 用于修改基础类型的值时，返回假 */
        public bool Change<T>(string id, Process<T> processer) where T : notnull
        {
            if (_objects.TryGetValue(id, out object? obj) && obj is T t)
            {
                processer(ref t);
                _objects[id] = t;
                return true;
            }
            return false;
        }

        static Properties()
        {
            //读文件初始化Default
        }
#nullable disable
    }
}
