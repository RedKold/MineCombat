using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public static class Helper
    {
        public static long NextInt64(this Random random, long min, long max)
        {
            if (max <= min)
                throw new ArgumentException("给出的取值上限不大于取值下限");

            return ((((long)random.Next() << 32) | (long)random.Next()) % (max - min)) + min;
        }

        private static object TryClone(object src, Type type)
        {
            var copier = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { type }, null);
            if (copier is not null)
                return copier.Invoke(new object[] { src });

            var cloneable = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICloneable<>));
            if (cloneable is not null)
#pragma warning disable CS8602, CS8603
                return cloneable.GetMethod("Clone").Invoke(src, null);
#pragma warning restore CS8602, CS8603

            if (src is ICloneable csrc)
                return csrc.Clone();

            return src;
        }
        public static T TryClone<T>(this T src) where T : class
        {
            Type type = typeof(T);
            return (T)TryClone(src, type);
        }

        internal static void Trigger(this Events<IPriorityEvent, AConstPriorityEvent> events, string name)
        {
            var evt = events[name];
            if (evt is ConstPriorityEvent cpevt)
                cpevt.Trigger();
            else
                throw new ArgumentException($"{events.Type}{name}参数错误");
        }
        internal static void Trigger<T>(this Events<IPriorityEvent, AConstPriorityEvent> events, string name, T paras)
        {
            var evt = events[name];
            if (evt is ConstPriorityEvent<T> cpevt)
                cpevt.Trigger(paras);
            else
                throw new ArgumentException($"{events.Type}{name}参数错误");
        }
        internal static void Trigger(this Events<ISlicedEvent, AConstSlicedEvent> events, string name, string branch)
        {
            var evt = events[name];
            if (evt is ConstSlicedEvent csevt)
                csevt.Trigger(branch);
            else
                throw new ArgumentException($"{events.Type}{name}参数错误");
        }
        internal static void Trigger<T>(this Events<ISlicedEvent, AConstSlicedEvent> events, string name, string branch, T paras)
        {
            var evt = events[name];
            if (evt is ConstSlicedEvent<T> csevt)
                csevt.Trigger(branch, paras);
            else
                throw new ArgumentException($"{events.Type}{name}参数错误");
        }
        internal static void Trigger(this Events<IRandomEvent, AConstRandomEvent> events, string name)
        {
            var evt = events[name];
            if (evt is ConstRandomEvent crevt)
                crevt.Trigger();
            else
                throw new ArgumentException($"{events.Type}{name}参数错误");
        }
        internal static void Trigger<T>(this Events<IRandomEvent, AConstRandomEvent> events, string name, T paras)
        {
            var evt = events[name];
            if (evt is ConstRandomEvent<T> crevt)
                crevt.Trigger(paras);
            else
                throw new ArgumentException($"{events.Type}{name}参数错误");
        }
    }
}
