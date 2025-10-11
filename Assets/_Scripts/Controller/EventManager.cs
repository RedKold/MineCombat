using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace MineCombat
{
    internal interface IEvent
    {
        void Bind(Delegate dlg);
    }

    internal class Event : IEvent
    {
#nullable enable
        private Action _actions;
        private Action? _finalize;

        internal Event(Action prepare, Action? finalize = null)
        {
            _actions = prepare;
            _finalize = finalize;
        }

        void IEvent.Bind(Delegate dlg)
        {
            if (dlg is Action act)
                _actions += act;
            else
                throw new ArgumentException("需要无参数的无返回值函数");
        }

        internal void Trigger()
        {
            _actions();
            _finalize?.Invoke();
        }

#nullable disable
    }

    internal class Event<T> : IEvent
    {
#nullable enable
        private Action<T> _actions;
        private Action<T>? _finalize;

        private IOrderedEnumerable<FieldInfo>? _fields;
        private string _parasName;

        internal Event(Action<T> prepare, Action<T>? finalize = null)
        {
            _actions = prepare;
            _finalize = finalize;
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

            if (isTuple)
            {
                _fields = typeof(T).GetFields().Where(f => f.Name.StartsWith("Item")).OrderBy(f => f.Name);
                _parasName = string.Join(',', _fields.Select(f => f.FieldType.Name));
            }
            else
            {
                _fields = null;
                _parasName = type.Name;
            }
        }

        //利用反射包装dlg使其能够接收元组作为参数
        private Action<T>? TryTransfer(Delegate dlg)
        {
            if (dlg is Action<T> act)
                return act;
            if (_fields is not null)
            {
                var paras = dlg.Method.GetParameters();
                if (paras.Length == _fields.ToArray().Length && paras.Select(p => p.ParameterType).SequenceEqual(_fields.Select(f => f.FieldType)))
                    return tuple => { dlg.Method.Invoke(dlg.Target, _fields.Select(f => f.GetValue(tuple)).ToArray()); };
            }
            return null;
        }

        void IEvent.Bind(Delegate dlg)
        {
            var act = TryTransfer(dlg);
            if (act is not null)
                _actions += act;
            else
                throw new ArgumentException($"需要参数为{_parasName}的无返回值函数");
        }

        internal void Trigger(T para)
        {
            _actions(para);
            _finalize?.Invoke(para);
        }

#nullable disable
    }
    public static class EventManager
    {
        private static Dictionary<string, IEvent> events = new();

        public static void Bind(string name, Delegate dlg)
        {
            if (events.TryGetValue(name, out var revt))
            {
                revt.Bind(dlg);
            }
            else
                throw new ArgumentException($"事件{name}不存在");
        }

        internal static void Trigger<T>(string name, T para)
        {
            if (events.TryGetValue(name, out var revt))
            {
                if (revt is Event<T> evt)
                    evt.Trigger(para);
                else
                    throw new ArgumentException($"事件{name}参数错误");
            }
            else
                throw new ArgumentException($"事件{name}不存在");
        }

        internal static void Trigger(string name)
        {
            if (events.TryGetValue(name, out var revt))
            {
                if (revt is Event evt)
                    evt.Trigger();
                else
                    throw new ArgumentException($"事件{name}参数错误");
            }
            else
                throw new ArgumentException($"事件{name}不存在");
        }

        static EventManager()
        {
            events.Add("DamageProcess", new Event<Damage>(x => { }));
            events.Add("CombatantDied", new Event<Combatant>(x => { }));
            events.Add("HealthChanged", new Event<Combatant>(x => { }));
        }
    }
}