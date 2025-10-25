using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public static class Parser
    {
#nullable enable
        private enum T { DIR, SET, LIST, END, NAME, VALUE }

        private struct Tstring
        {
            internal T type;
            internal string value;

            internal Tstring(T type, string value)
            {
                this.type = type;
                this.value = value;
            }
        };

        private class Worker
        {
            private Dictionary<char, char> validPairs;
            private HashSet<char> validPairEnds;
            private Dictionary<char, T> pairTypes;
            private HashSet<char> validQuoters;
            private HashSet<char> ignoreChars;
            private char divider;
            private char coloner;
            private char escaper;

            private Stack<char> pairStack;
            private char lastQuoter;

            private string src;
            private ushort limit;

            private readonly bool strict;

            public readonly List<Tstring>? result;
            public Worker(bool strict, Tstring[] pairs, char[] quoters, char[] ignores, string src, ushort limit, char divider = ',', char coloner = ':', char escaper = '\\')
            {
                this.strict = strict;
                validPairs = new();
                validPairEnds = new();
                pairTypes = new();
                foreach (var pair in pairs)
                {
                    if (pair.value.Length != 2)
                        throw new ArgumentException("构造时传入了长度不符合预期的字符串");
                    validPairs.Add(pair.value[0], pair.value[1]);
                    validPairEnds.Add(pair.value[1]);
                    pairTypes.Add(pair.value[0], pair.type);
                }
                validQuoters = new(quoters);
                ignoreChars = new(ignores);
                this.divider = divider;
                this.coloner = coloner;
                this.escaper = escaper;
                pairStack = new();
                lastQuoter = '\0';
                this.src = src;
                this.limit = limit;
                result = Process();
            }

            private List<Tstring>? Process()
            {
                if (src.Length > 65535 || src.Length < 3)
                {
                    if (strict)
                        throw new ArgumentException("源字符串的长度不合法");
                    else 
                        return null;
                } 
                    

                List<Tstring> result = new();
                if (validPairs.ContainsKey(src[0]))
                {
                    pairStack.Push(src[0]);
                    result.Add(new Tstring(pairTypes[src[0]], string.Empty));
                }
                else if (strict)
                    throw new ArgumentException($"字符串需要以下列合法字符之一开头：“{string.Join(',', validPairs.Keys)}”");
                else 
                    return null;

                    StringBuilder buffer = new();
                ushort i = 1;
                char c;
                bool opened = true, quoted = false, escaped = false;
                while (i < src.Length)
                {
                    if (pairStack.Count == 0)
                        break;
                    if (pairStack.Count > limit)
                    {
                        if (strict)
                            throw new ArgumentException($"字符串内部结构过深，最大深度限制为：{limit}");
                        else
                            return null;
                    }

                    c = src[i];

                    // 处理转义字符
                    if (escaped)
                    {
                        buffer.Append(c);
                        escaped = false;
                        i++;
                        continue;
                    }

                    // 检查转义
                    if (c == escaper)
                    {
                        escaped = true;
                        i++;
                        continue;
                    }

                    // 处理引号
                    if (validQuoters.Contains(c))
                    {
                        if (!quoted)
                        {
                            // 开始引号
                            quoted = true;
                            lastQuoter = c;
                        }
                        else if (c == lastQuoter)
                            // 结束引号
                            quoted = false;
                        else
                            // 引号内的其他引号字符，作为普通字符处理
                            buffer.Append(c);
                        i++;
                        continue;
                    }

                    // 如果在引号内，直接添加字符
                    if (quoted)
                    {
                        buffer.Append(c);
                        i++;
                        continue;
                    }

                    // 忽略字符
                    if (ignoreChars.Contains(c))
                    {
                        i++;
                        continue;
                    }

                    // 处理配对字符
                    if (validPairs.ContainsKey(c))
                    {
                        // 开始配对
                        if (buffer.Length > 0)
                        {
                            if (strict)
                                throw new ArgumentException($"错误使用的配对字符：“{c}”");
                            else
                                return null;
                        }
                        pairStack.Push(c);
                        result.Add(new Tstring(pairTypes[c], string.Empty));
                        i++;
                        continue;
                    }
                    else if (validPairEnds.Contains(c))
                    {
                        // 尝试结束配对
                        if (pairStack.Count > 0)
                        {
                            char d = pairStack.Pop();
                            if (c == validPairs[d])
                            {
                                if (buffer.Length > 0)
                                {
                                    result.Add(new Tstring(T.VALUE, buffer.ToString()));
                                    buffer.Clear();
                                }
                                opened = false;
                                result.Add(new Tstring(T.END, string.Empty));
                            }
                            else if (strict)
                                throw new ArgumentException($"配对字符不匹配：“{d}{c}”");
                            else
                                return null;
                        }
                        else if (strict)
                            throw new ArgumentException($"错误使用的配对字符：“{c}”");
                        else 
                            return null;
                            i++;
                        continue;
                    }

                    // 处理分隔符
                    if (c == divider)
                    {
                        if (opened)
                        {
                            if (buffer.Length > 0)
                            {
                                result.Add(new Tstring(T.VALUE, buffer.ToString()));
                                buffer.Clear();
                            }
                            else if (strict)
                                throw new ArgumentException($"错误使用的分隔符：“{c}”，此前应有值");
                            else 
                                return null;
                        }
                        else
                        {
                            opened = true;
                            if (buffer.Length > 0)
                            {
                                if (strict)
                                    throw new ArgumentException($"错误使用的分隔符：“{c}”，此前不应有值");
                                else
                                    return null;
                            }
                        }
                            i++;
                        continue;
                    }

                    // 处理键值分隔符
                    if (c == coloner)
                    {
                        if (buffer.Length > 0)
                        {
                            result.Add(new Tstring(T.NAME, buffer.ToString()));
                            buffer.Clear();
                        }
                        else if (strict)
                            throw new ArgumentException($"错误使用的键值分隔符：“{c}”");
                        else
                            return null;
                            i++;
                        continue;
                    }

                    // 普通字符
                    buffer.Append(c);
                    i++;
                }

                // 检查是否有未关闭的引号或括号
                if (quoted || pairStack.Count > 0)
                {
                    if (strict)
                        throw new ArgumentException("字符串含有未关闭的引号或括号");
                    else
                        return null;
                } 

                return result;
            }
        }

        private static char[] commonIgnores = {
            ' ',      // 空格
            '\t',     // 水平制表符
            '\n',     // 换行符
            '\r',     // 回车符
            '\v',     // 垂直制表符
            '\f',     // 换页符
            '\u00A0', // 不间断空格
            '\u1680', // 欧甘空格
            '\u2000', // 半身空距
            '\u2001', // 全身空距
            '\u2002', // 半身空距
            '\u2003', // 全身空距
            '\u2004', // 三分空距
            '\u2005', // 四分空距
            '\u2006', // 六分空距
            '\u2007', // 数字空格
            '\u2008', // 标点空格
            '\u2009', // 细空格
            '\u200A', // 毛发空格
            '\u2028', // 行分隔符
            '\u2029', // 段分隔符
            '\u202F', // 窄不间断空格
            '\u205F', // 中等数学空格
            '\u3000'  // 表意文字空格
        };

        public static Box<string>?[]? ToBoxArray(string src, bool strict = false)
        {
            Tstring[] pairs = {
                new Tstring(T.LIST, "{}"),
                new Tstring(T.LIST, "()"),
                new Tstring(T.LIST, "[]")
            };
            char[] quoters = { '"' };
            Worker worker = new Worker(strict, pairs, quoters, commonIgnores, src, 2);
            List<Tstring>? tokens = worker.result;
            if (tokens?.Any() == true)
            {
                List<Box<string>?> result = new();
                List<string> current = new();
                bool inList = false;

                foreach (var token in tokens)
                {
                    switch (token.type)
                    {
                        case T.NAME:
                            return null;

                        case T.LIST:
                            inList = true;
                            break;

                        case T.VALUE:
                            if (inList)
                                current.Add(token.value);
                            else
                                result.Add(token.value);
                            break;

                        case T.END:
                            if (inList)
                            {
                                inList = false;

                                if (current.Count > 0)
                                    result.Add(current.ToArray());
                                else
                                    result.Add(null);
                                current.Clear();
                            }
                            else
                                return result.ToArray();
                            break;

                        default:
                            break;
                    }
                }
            }
            return null;
        }

        public static Box<string>? ToBox(string src, bool strict = false)
        {
            Tstring[] pairs = {
                new Tstring(T.LIST, "{}"),
                new Tstring(T.LIST, "()"),
                new Tstring(T.LIST, "[]")
            };
            char[] quoters = { '"' };
            Worker worker = new Worker(strict, pairs, quoters, commonIgnores, src, 1);
            List<Tstring>? tokens = worker.result;
            if (tokens?.Any() == true)
            {
                List<string> list = new List<string>();

                foreach (var token in tokens)
                {
                    switch (token.type)
                    {
                        case T.NAME:
                            return null;

                        case T.VALUE:
                            list.Add(token.value);
                            break;

                        default:
                            break;
                    }
                }
                return new Box<string>(list.ToArray());
            }
            return null;
        }

        public static IEnumerable<object>? ToCollection(string src, byte limit = 255, bool strict = false)
        {
            Tstring[] pairs = {
                new Tstring(T.SET, "{}"),
                new Tstring(T.SET, "()"),
                new Tstring(T.LIST, "[]")
            };
            char[] quoters = { '"' };
            Worker worker = new Worker(strict, pairs, quoters, commonIgnores, src, limit);
            List<Tstring>? tokens = worker.result;
            if (tokens?.Any() == true)
            {
                Stack<IEnumerable<object>> workingStack = new ();

                foreach (var token in tokens)
                {
                    switch (token.type)
                    {
                        case T.NAME:
                            return null;

                        case T.SET:
                            var set = new HashSet<object>();
                            workingStack.Push(set);
                            break;

                        case T.LIST:
                            var list = new List<object>();
                            workingStack.Push(list);
                            break;

                        case T.VALUE:
                            if (workingStack.Count == 0)
                                return null;

                            var current = workingStack.Peek();
                            if (current is HashSet<object> cset)
                            {
                                cset.Add(token.value);
                            }
                            else if (current is List<object> clist)
                            {
                                clist.Add(token.value);
                            }
                            break;

                        case T.END:
                            if (workingStack.Count > 1)
                            {
                                // 嵌套集合结束
                                var finished = workingStack.Pop();

                                var parent = workingStack.Peek();
                                if (parent is HashSet<object> pset)
                                {
                                    pset.Add(finished);
                                }
                                else if (parent is List<object> plist)
                                {
                                    plist.Add(finished);
                                }
                            }
                            else if (workingStack.Count == 1)
                            {
                                // 根集合结束
                                var root = workingStack.Pop();
                                if (root is IEnumerable<object> enumerable)
                                {
                                    return enumerable;
                                }
                            }
                            break;

                        case T.DIR:
                            break;
                    }
                }
            }
            return null;
        }
#nullable disable
    }
}
