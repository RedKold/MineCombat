using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MineCombat
{
    public class DamageModifier
    {
        internal enum Type { ADD, MUL }

        internal readonly Type type;
        internal readonly uint priority;
        internal ITags tags;
        private double _value;

        internal DamageModifier(Type type, double value, uint priority, ITags tags)
        {
            this.type = type;
            _value = value;
            this.priority = priority;
            this.tags = tags;
        }

        public static DamageModifier CreateAdd(double value, uint priority, ITags tags)
        {
            return new DamageModifier(Type.ADD, value, priority, tags);
        }

        public static DamageModifier CreateMul(double value, uint priority, ITags tags)
        {
            return new DamageModifier(Type.MUL, value, priority, tags);
        }

        //此修改器是否忽略该类型的伤害
        internal bool Ignore(string dmgid)
        {
            return DamageTypes.Ignore(dmgid, tags);
        }

        internal bool TryMerge(DamageModifier mdf)
        {
            if (mdf.type == type && mdf.priority == priority)
            {
                _value += mdf._value;
                return true;
            }
            else 
                return false;
        }
        internal void Process(ref double damage)
        {
            switch (type)
            {
                case Type.ADD: damage += _value; break;
                case Type.MUL: damage *= Math.Max(0, (1 + _value)); break;
            }
        }
    }

    public class Damage
    {
        public readonly string type;
        public double value;
        private Dictionary<string, DamageModifier> _modifiers;

        internal Damage(string type, float value, Dictionary<string, DamageModifier>? modifiers = null)
        {
            this.type = type;
            this.value = value;
            _modifiers = modifiers?.Any() == true ? modifiers : new();
        }

        //添加、合并或替换
        internal void AddModifier(string mdfid, DamageModifier mdf)
        {
            if (!(_modifiers.ContainsKey(mdfid) && _modifiers[mdfid].TryMerge(mdf)))
                _modifiers[mdfid] = mdf;
        }

        internal bool RemoveModifier(string mdfid)
        {
            return _modifiers.Remove(mdfid);
        }

        internal double Get()
        {
            EventManager.Trigger("DamageProcess", this);
            List<DamageModifier> modifiers = _modifiers.Values.ToList();
            modifiers.Sort((x, y) => x.priority.CompareTo(y.priority));
            double value = this.value;
            foreach (var mdf in modifiers)
            {
                if (!mdf.Ignore(type))
                    mdf.Process(ref value);
            }
            return value;
        }
    }


    public static class DamageTypes 
    {
        private static TagsManager types_tags_table = new();

        internal static bool Ignore(string type, ITags tags)
        {
            return types_tags_table.Match(type, tags);
        }

        public static bool Ignore(string type, string tag)
        {
            return types_tags_table.Match(type, tag);
        }

        static DamageTypes()
        {
            types_tags_table.Add("mc_magic", (StaticTags)"{mc_bypass_armor, mc_bypass_test}");
        }
    }
}
