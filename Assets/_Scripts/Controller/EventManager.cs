using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MineCombat
{
    public class Actions
    {
#nullable enable
        private Dictionary<int, Action> _actions = new();

        public Actions() { }

        public void Add(Action action, int priority = 0)
        {
            if (_actions.ContainsKey(priority))
                _actions[priority] += action;
            else
                _actions[priority] = action;
        }

        public Action? Get()
        {
            if (_actions.Count == 0)
                return null;

            var pairs = _actions.ToArray();
            Array.Sort(pairs, (x, y) => x.Key.CompareTo(y.Key));

            Action? result = null;
            foreach (var pair in pairs)
            {
                result += pair.Value;
            }

            return result;
        }
#nullable disable
    }

    public class Actions<T>
    {
#nullable enable
        private Dictionary<int, Action<T>> _actions = new();
        private TupleTranslators.Worker<T>? worker;

        public Actions() 
        {
            worker = TupleTranslators.GetTranslator<T>();
        }

        protected Action<T>? TryTranslate(Delegate dlg)
        {
            return worker?.Translate(dlg);
        }

        public void Add(Action<T> action, int priority = 0)
        {
            if (_actions.ContainsKey(priority))
                _actions[priority] += action;
            else
                _actions[priority] = action;
        }

        protected void Add(Delegate dlg, int priority = 0)
        {
            Add(TryTranslate(dlg) ?? throw new ArgumentException($"需要参数为{typeof(T)}的无返回值函数"), priority);
        }

        public Action<T>? Get()
        {
            if (_actions.Count == 0)
                return null;

            var pairs = _actions.ToArray();
            Array.Sort(pairs, (x, y) => x.Key.CompareTo(y.Key));

            Action<T>? result = null;
            foreach (var pair in pairs)
            {
                result += pair.Value;
            }

            return result;
        }
#nullable disable
    }

    public interface IEvent { }

    public interface IConstable<T> where T : IConstable<T>
    {
        public T ConstCast();
    }

    public interface IPriorityEvent : IEvent, IConstable<AConstPriorityEvent>
    {
        public void Bind(Action action, int priority = 0);
        public void Bind<T1>(Action<T1> action, int priority = 0);
        public void Bind<T1, T2>(Action<T1, T2> action, int priority = 0);
        public void Bind<T1, T2, T3>(Action<T1, T2, T3> action, int priority = 0);
        public void Bind<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0);
    }

    public interface ISlicedEvent : IEvent, IConstable<AConstSlicedEvent>
    {
#nullable enable
        public void Bind(string branch, Action action, int priority = 0);
        public void Bind<T1>(string branch, Action<T1> action, int priority = 0);
        public void Bind<T1, T2>(string branch, Action<T1, T2> action, int priority = 0);
        public void Bind<T1, T2, T3>(string branch, Action<T1, T2, T3> action, int priority = 0);
        public void Bind<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0);
        public void CreateBranch(string branch, Action? prepare = null, Action? finalize = null);
        public void CreateBranch<T1>(string branch, Action<T1> prepare, Action<T1> finalize);
        public void CreateBranch<T1, T2>(string branch, Action<T1, T2> prepare, Action<T1, T2> finalize);
        public void CreateBranch<T1, T2, T3>(string branch, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize);
        public void CreateBranch<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize);
        public void CreateBranch<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize);
        public void CreateBranch<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize);
        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize);
        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize);
#nullable disable
    }

    public interface IRandomEvent : IEvent, IConstable<AConstRandomEvent>
    {
#nullable enable
        public void Bind(string id, Action action, int priority = 0);
        public void Bind<T1>(string id, Action<T1> action, int priority = 0);
        public void Bind<T1, T2>(string id, Action<T1, T2> action, int priority = 0);
        public void Bind<T1, T2, T3>(string id, Action<T1, T2, T3> action, int priority = 0);
        public void Bind<T1, T2, T3, T4>(string id, Action<T1, T2, T3, T4> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5>(string id, Action<T1, T2, T3, T4, T5> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6>(string id, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string id, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0);
        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string id, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0);
        public void CreateItem(string id, uint weight = 1024, Action? prepare = null, Action? finalize = null);
        public void CreateItem<T1>(string id, uint weight, Action<T1> prepare, Action<T1> finalize);
        public void CreateItem<T1, T2>(string id, uint weight, Action<T1, T2> prepare, Action<T1, T2> finalize);
        public void CreateItem<T1, T2, T3>(string id, uint weight, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize);
        public void CreateItem<T1, T2, T3, T4>(string id, uint weight, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize);
        public void CreateItem<T1, T2, T3, T4, T5>(string id, uint weight, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize);
        public void CreateItem<T1, T2, T3, T4, T5, T6>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize);
        public void CreateItem<T1, T2, T3, T4, T5, T6, T7>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize);
        public void CreateItem<T1, T2, T3, T4, T5, T6, T7, T8>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize);
        public void SetWeight(string id, uint weight);
#nullable disable
    }

    abstract public class AConstPriorityEvent : IPriorityEvent, IConstable<AConstPriorityEvent>
    {
        public void Bind(MethodInfo method, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind(Action action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1>(Action<T1> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2>(Action<T1, T2> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3>(Action<T1, T2, T3> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            throw new ArgumentException($"构建完成的静态事件无法绑定新的函数");
        }

        public AConstPriorityEvent ConstCast()
        {
            throw new ArgumentException("不能重复构建一个静态事件");
        }
    }

    abstract public class AConstSlicedEvent : ISlicedEvent, IConstable<AConstSlicedEvent>
    {
#nullable enable
        public void Bind(string branch, Action action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1>(string branch, Action<T1> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2>(string branch, Action<T1, T2> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3>(string branch, Action<T1, T2, T3> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void CreateBranch(string branch, Action? prepare = null, Action? finalize = null)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1>(string branch, Action<T1> prepare, Action<T1> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2>(string branch, Action<T1, T2> prepare, Action<T1, T2> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2, T3>(string branch, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize)
        {
            throw new ArgumentException($"构建完成的静态事件无法创建新的分支");
        }

        public AConstSlicedEvent ConstCast()
        {
            throw new ArgumentException("不能重复构建一个静态事件");
        }
#nullable disable
    }

    abstract public class AConstRandomEvent : IRandomEvent, IConstable<AConstRandomEvent>
    {
#nullable enable
        public void Bind(string id, Action action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1>(string id, Action<T1> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2>(string id, Action<T1, T2> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3>(string id, Action<T1, T2, T3> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4>(string id, Action<T1, T2, T3, T4> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5>(string id, Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(string id, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string id, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string id, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            throw new ArgumentException("构建完成的静态事件无法绑定新的函数");
        }

        public void CreateItem(string id, uint weight = 1024, Action? prepare = null, Action? finalize = null)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1>(string id, uint weight, Action<T1> prepare, Action<T1> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2>(string id, uint weight, Action<T1, T2> prepare, Action<T1, T2> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2, T3>(string id, uint weight, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2, T3, T4>(string id, uint weight, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2, T3, T4, T5>(string id, uint weight, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6, T7>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6, T7, T8>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize)
        {
            throw new ArgumentException("构建完成的静态事件无法创建新的随机项");
        }

        public void SetWeight(string id, uint weight)
        {
            throw new ArgumentException("构建完成的静态事件无法修改随机项的权重");
        }

        public AConstRandomEvent ConstCast()
        {
            throw new ArgumentException("不能重复构建一个静态事件");
        }
#nullable disable
    }

    public sealed class PriorityEvent : Actions, IPriorityEvent
    {
#nullable enable
        private Action? _prepare;
        private Action? _finalize;

        internal PriorityEvent(Action? prepare = null, Action? finalize = null)
        {
            _prepare = prepare;
            _finalize = finalize;
        }

        public void Bind(Action action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1>(Action<T1> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2>(Action<T1, T2> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3>(Action<T1, T2, T3> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        internal void Trigger()
        {
            Action? action = _prepare;
            action += Get();
            action += _finalize;
            return new ConstPriorityEvent(action);
        }
#nullable disable
    }

    public sealed class PriorityEvent<T> : Actions<T>, IPriorityEvent
    {
#nullable enable
        private Action<T>? _prepare;
        private Action<T>? _finalize;

        internal PriorityEvent(Action<T>? prepare = null, Action<T>? finalize = null)
        {
            _prepare = prepare;
            _finalize = finalize;
        }

        internal PriorityEvent(Delegate prepare, Delegate finalize)
        {
            _prepare = TryTranslate(prepare);
            _finalize = TryTranslate(prepare);
        }

        public void Bind(Action action, int priority = 0)
        {
            throw new ArgumentException("至少需要有参数参数的无返回值函数");
        }

        public void Bind<T1>(Action<T1> action, int priority = 0)
        {
            if (action is Action<T> actionT)
                Add(actionT, priority);
            else
                Add(action, priority);
        }

        public void Bind<T1, T2>(Action<T1, T2> action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1, T2, T3>(Action<T1, T2, T3> action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            Add(action, priority);
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            Add(action, priority);
        }

        public AConstPriorityEvent ConstCast()
        {
            Action<T>? action = _prepare;
            action += Get();
            action += _finalize;
            return new ConstPriorityEvent<T>(action);
        }
#nullable disable
    }

    public sealed class SlicedEvent : ISlicedEvent
    {
#nullable enable
        private Dictionary<string, PriorityEvent> _branches = new();

        public void Bind(string branch, Action action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1>(string branch, Action<T1> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2>(string branch, Action<T1, T2> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3>(string branch, Action<T1, T2, T3> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch(string branch, Action? prepare = null, Action? finalize = null)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent(prepare, finalize));
        }

        public void CreateBranch<T1>(string branch, Action<T1> prepare, Action<T1> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2>(string branch, Action<T1, T2> prepare, Action<T1, T2> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2, T3>(string branch, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public AConstSlicedEvent ConstCast()
        {
            return new ConstSlicedEvent(_branches.ToDictionary(pair => pair.Key, pair => (ConstPriorityEvent)pair.Value.ConstCast()));
        }
#nullable disable
    }

    public sealed class SlicedEvent<T> : ISlicedEvent
    {
#nullable enable
        private Dictionary<string, PriorityEvent<T>> _branches = new();

        public void Bind(string branch, Action action, int priority = 0)
        {
            throw new ArgumentException($"至少需要有参数的无返回值函数");
        }

        public void Bind<T1>(string branch, Action<T1> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2>(string branch, Action<T1, T2> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else 
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2, T3>(string branch, Action<T1, T2, T3> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else 
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else 
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在事件分支{branch}");
        }

        public void CreateBranch(string branch, Action? prepare = null, Action? finalize = null)
        {
            if (prepare is not null || finalize is not null)
                throw new ArgumentException("至少需要有参数的无返回值函数");
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>());
        }

        public void CreateBranch<T1>(string branch, Action<T1> prepare, Action<T1> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2>(string branch, Action<T1, T2> prepare, Action<T1, T2> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2, T3>(string branch, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public void CreateBranch<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize)
        {
            if (_branches.ContainsKey(branch))
                throw new ArgumentException($"事件分支{branch}被重复创建");
            _branches.Add(branch, new PriorityEvent<T>(prepare, finalize));
        }

        public AConstSlicedEvent ConstCast()
        {
            return new ConstSlicedEvent<T>(_branches.ToDictionary(pair => pair.Key, pair => (ConstPriorityEvent<T>)pair.Value.ConstCast()));
        }
#nullable disable
    }

    public sealed class RandomEvent : IRandomEvent
    {
#nullable enable
        private Dictionary<string, (PriorityEvent evt, uint wgt)> _items = new();

        public void Bind(string id, Action action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1>(string id, Action<T1> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2>(string id, Action<T1, T2> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3>(string id, Action<T1, T2, T3> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4>(string id, Action<T1, T2, T3, T4> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5>(string id, Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(string id, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string id, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string id, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem(string id, uint weight = 1024, Action? prepare = null, Action? finalize = null)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent(prepare, finalize), weight));
        }

        public void CreateItem<T1>(string id, uint weight, Action<T1> prepare, Action<T1> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2>(string id, uint weight, Action<T1, T2> prepare, Action<T1, T2> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3>(string id, uint weight, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3, T4>(string id, uint weight, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3, T4, T5>(string id, uint weight, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6, T7>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6, T7, T8>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize)
        {
            throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void SetWeight(string id, uint weight)
        {
            if (_items.TryGetValue(id, out var item))
                item.wgt = weight;
            throw new ArgumentException($"不存在随机项{id}");
        }

        public AConstRandomEvent ConstCast()
        {
            int len = _items.Count;
            int i = 0;
            var items = new ConstPriorityEvent[len];
            var weights = new uint[len];

            foreach ( var item in _items)
            {
                weights[i] = item.Value.wgt;
                if ( weights[i] == 0 )
                    continue;
                items[i++] = (ConstPriorityEvent)item.Value.evt.ConstCast();
            }

            return new ConstRandomEvent(items, weights);
        }
#nullable disable
    }

    public sealed class RandomEvent<T> : IRandomEvent
    {
#nullable enable
        private Dictionary<string, (PriorityEvent<T> evt, uint wgt)> _items = new();

        public void Bind(string id, Action action, int priority = 0)
        {
            throw new ArgumentException("至少需要有参数的无返回值函数");
        }

        public void Bind<T1>(string id, Action<T1> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2>(string id, Action<T1, T2> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2, T3>(string id, Action<T1, T2, T3> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2, T3, T4>(string id, Action<T1, T2, T3, T4> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2, T3, T4, T5>(string id, Action<T1, T2, T3, T4, T5> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2, T3, T4, T5, T6>(string id, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7>(string id, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string id, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)
        {
            if (_items.TryGetValue(id, out var item))
                item.evt.Bind(action, priority);
            else
                throw new ArgumentException($"不存在随机项{id}");
        }

        public void CreateItem(string id, uint weight = 1024, Action? prepare = null, Action? finalize = null)
        {
            if (prepare is not null || finalize is not null)
                throw new ArgumentException("至少需要有参数的无返回值函数");
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(), weight));
        }

        public void CreateItem<T1>(string id, uint weight, Action<T1> prepare, Action<T1> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void CreateItem<T1, T2>(string id, uint weight, Action<T1, T2> prepare, Action<T1, T2> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void CreateItem<T1, T2, T3>(string id, uint weight, Action<T1, T2, T3> prepare, Action<T1, T2, T3> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void CreateItem<T1, T2, T3, T4>(string id, uint weight, Action<T1, T2, T3, T4> prepare, Action<T1, T2, T3, T4> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void CreateItem<T1, T2, T3, T4, T5>(string id, uint weight, Action<T1, T2, T3, T4, T5> prepare, Action<T1, T2, T3, T4, T5> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6> prepare, Action<T1, T2, T3, T4, T5, T6> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6, T7>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7> prepare, Action<T1, T2, T3, T4, T5, T6, T7> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight)); throw new ArgumentException("需要无参数的无返回值函数");
        }

        public void CreateItem<T1, T2, T3, T4, T5, T6, T7, T8>(string id, uint weight, Action<T1, T2, T3, T4, T5, T6, T7, T8> prepare, Action<T1, T2, T3, T4, T5, T6, T7, T8> finalize)
        {
            if (_items.ContainsKey(id))
                throw new ArgumentException($"事件分支{id}被重复创建");
            _items.Add(id, (new PriorityEvent<T>(prepare, finalize), weight));
        }

        public void SetWeight(string id, uint weight)
        {
            if (_items.TryGetValue(id, out var item))
                item.wgt = weight;
            throw new ArgumentException($"不存在随机项{id}");
        }

        public AConstRandomEvent ConstCast()
        {
            int len = _items.Count;
            int i = 0;
            var items = new ConstPriorityEvent<T>[len];
            var weights = new uint[len];

            foreach (var item in _items)
            {
                weights[i] = item.Value.wgt;
                if (weights[i] == 0)
                    continue;
                items[i++] = (ConstPriorityEvent<T>)item.Value.evt.ConstCast();
            }

            return new ConstRandomEvent<T>(items, weights);
        }
#nullable disable
    }

    public sealed class ConstPriorityEvent : AConstPriorityEvent
    {
#nullable enable
        private Action? _action;

        internal ConstPriorityEvent(Action? action)
        {
            _action = action;
        }

        public void Trigger()
        {
            _action?.Invoke();
        }
#nullable disable
    }

    public sealed class ConstPriorityEvent<T> : AConstPriorityEvent
    {
#nullable enable
        private Action<T>? _action;

        internal ConstPriorityEvent(Action<T>? action)
        {
            _action = action;
        }

        public void Trigger(T paras)
        {
            _action?.Invoke(paras);
        }
#nullable disable
    }

    public sealed class ConstSlicedEvent : AConstSlicedEvent
    {
        private Dictionary<string, ConstPriorityEvent> _branches;

        internal ConstSlicedEvent(Dictionary<string, ConstPriorityEvent> branches)
        {
            _branches = branches;
        }

        public void Trigger(string branch)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Trigger();
            else
                throw new ArgumentException($"尝试触发不存在的事件分支：{branch}");
        }
    }

    public sealed class ConstSlicedEvent<T> : AConstSlicedEvent
    {
        private Dictionary<string, ConstPriorityEvent<T>> _branches;

        internal ConstSlicedEvent(Dictionary<string, ConstPriorityEvent<T>> branches)
        {
            _branches = branches;
        }

        public void Trigger(string branch, T paras)
        {
            if (_branches.TryGetValue(branch, out var evt))
                evt.Trigger(paras);
            else
                throw new ArgumentException($"尝试触发不存在的事件分支：{branch}");
        }
    }

    public sealed class ConstRandomEvent : AConstRandomEvent
    {
        private ConstRandomizer<ConstPriorityEvent> _rdm;

        internal ConstRandomEvent(ConstPriorityEvent[] items, uint[] weights)
        {
            _rdm = new(items, weights);
        }

        public void Trigger()
        {
            _rdm.RandomlyGet().Trigger();
        }
    }

    public sealed class ConstRandomEvent<T> : AConstRandomEvent
    {
        private ConstRandomizer<ConstPriorityEvent<T>> _rdm;

        internal ConstRandomEvent(ConstPriorityEvent<T>[] items, uint[] weights)
        {
            _rdm = new(items, weights);
        }

        public void Trigger(T paras)
        {
            _rdm.RandomlyGet().Trigger(paras);
        }
    }

    public sealed class Events<I, A> where A : I where I : IEvent, IConstable<A>
    {
#nullable enable
        private Dictionary<string, I> _events = new();
        public readonly string Type;
        private bool _const = false;

        internal Events(string type)
        {
            Type = type;
        }

        public I this[string name]
        {
            get
            {
                if (_events.TryGetValue(name, out var t))
                    return t;
                else
                    throw new ArgumentException($"不存在名为{name}的{Type}");
            }
        }

        internal void Add(string name, I evt)
        {
            if (!_const)
                _events.Add(name, evt);
        }

        internal void BuildConst()
        {
            if (!_const)
            {
                _const = true;
                _events = _events.ToDictionary(pair => pair.Key, pair => (I)pair.Value.ConstCast());
            }
        }
#nullable disable
    }

    public static class EventManager
    {
        public readonly static Events<IPriorityEvent, AConstPriorityEvent> Events = new("事件");
        public readonly static Events<ISlicedEvent, AConstSlicedEvent> SlicedEvents = new("分支事件");
        public readonly static Events<IRandomEvent, AConstRandomEvent> RandomEvents = new("随机事件");

        internal static void BuildConst()
        {
            Events.BuildConst();
            SlicedEvents.BuildConst();
            RandomEvents.BuildConst();
        }

        static EventManager()
        {
            Events.Add("TestEvent1", new PriorityEvent());
            Events.Add("TestEvent2", new PriorityEvent<(string, string)>());
            Events.Add("TestEvent3", new PriorityEvent<(int, int)>(item => { var(i1, i2) = item; Console.WriteLine($"i1 = {i1}, i2 = {i2}"); }, item => { var (i1, i2) = item; Console.WriteLine($"i1 = {i1}, i2 = {i2}"); }));
            SlicedEvents.Add("TestSlicedEvent1", new SlicedEvent());
            RandomEvents.Add("TestRandomEvent1", new RandomEvent());
        }
    }
}