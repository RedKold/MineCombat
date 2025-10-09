using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MineCombat
{
    public delegate void Process<T>(ref T t);

    public interface IModifier<T> : IComparable<IModifier<T>>
    {
        public int Class();
        public void Process(ref T t);
        public void Process(T t);
        public bool TryMerge(IModifier<T> mdf);
    }

    abstract public class Modifier<T> : IModifier<T>
    {
        protected uint priority;
        protected ITags tags;

        protected Modifier(uint priority, ITags tags)
        {
            this.priority = priority;
            this.tags = tags;
        }

        public int Class() { return 65793; }

        public int CompareTo(IModifier<T>? other)
        {
            if (other is null)
                return 1;
            if (other is Modifier<T> co)
                return (int)priority - (int)co.priority;
            return 65793 - other.Class();
        }

        virtual public void Process(ref T t) { }
        virtual public void Process(T t) { }
        virtual public bool Ignore(T t) { return false; }
        virtual public bool TryMerge(IModifier<T> mdf) { return false; }
    }

    public class DamageModifier : Modifier<Damage>
    {
        internal DamageModifier(uint priority, ITags tags) : base(priority, tags) { }

        public override bool Ignore(Damage t)
        {
            return DamageTypes.Ignore(t.type, tags);
        }
    }

    public sealed class DamageModifierAdd : Modifier<Damage>
    {
        private double _value;

        internal DamageModifierAdd(double value, uint priority, ITags tags) : base(priority, tags)
        {
            _value = value;
        }

        public override void Process(ref Damage t)
        {
            if (!Ignore(t))
                t.value += _value;
        }

        public override void Process(Damage t)
        {
            if (!Ignore(t))
                t.value += _value;
        }

        public override bool TryMerge(IModifier<Damage> rmdf)
        {
            if (rmdf is DamageModifierAdd mdf && mdf.priority == priority)
            {
                _value += mdf._value;
                return true;
            }
            else
                return false;
        }
    }

    public sealed class DamageModifierMul : Modifier<Damage>
    {
        private double _value;

        internal DamageModifierMul(double value, uint priority, ITags tags) : base(priority, tags)
        {
            _value = Math.Max(-1, value);
        }

        public override void Process(ref Damage t)
        {
            if (!Ignore(t))
                t.value *= (1 + _value);
        }

        public override void Process(Damage t)
        {
            if (!Ignore(t))
                t.value *= (1 + _value);
        }

        public override bool TryMerge(IModifier<Damage> rmdf)
        {
            if (rmdf is DamageModifierMul mdf && mdf.priority == priority)
            {
                _value = Math.Max(-1, _value + mdf._value);
                return true;
            }
            else
                return false;
        }
    }

    public sealed class DamageModifierMulTotal : Modifier<Damage>
    {
        private double _value;

        internal DamageModifierMulTotal(double value, uint priority, ITags tags) : base(priority, tags)
        {
            _value = value;
        }

        public override void Process(ref Damage t)
        {
            if (!Ignore(t))
                t.value *= (1 + _value);
        }

        public override void Process(Damage t)
        {
            if (!Ignore(t))
                t.value *= (1 + _value);
        }

        public override bool TryMerge(IModifier<Damage> rmdf)
        {
            if (rmdf is DamageModifierMulTotal mdf && mdf.priority == priority)
            {
                _value = Math.Max(-1, (1 + _value) * (1 + mdf._value) - 1);
                return true;
            }
            else
                return false;
        }
    }

    public sealed class DamageModifierCustom : Modifier<Damage>
    {
        private Process<double> _processer;

        internal DamageModifierCustom(Process<double> processer, uint priority, ITags tags) : base(priority, tags)
        {
            _processer = processer;
        }

        public override void Process(ref Damage t)
        {
            if (!Ignore(t))
                _processer(ref t.value);
        }

        public override void Process(Damage t)
        {
            if (!Ignore(t))
                _processer(ref t.value);
        }
    }
}
