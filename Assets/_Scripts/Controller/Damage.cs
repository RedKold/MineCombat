using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MineCombat
{
    public static class DamageModifier
    {
        public static DamageModifierAdd CreateAdd(double value, uint priority, ITags tags)
        {
            return new DamageModifierAdd(value, priority, tags);
        }

        public static DamageModifierMul CreateMul(double value, uint priority, ITags tags)
        {
            return new DamageModifierMul(value, priority, tags);
        }

        public static DamageModifierMulTotal CreateMulTotal(double value, uint priority, ITags tags)
        {
            return new DamageModifierMulTotal(value, priority, tags);
        }

        public static DamageModifierCustom CreateCustom(Process<double> processer, uint priority, ITags tags)
        {
            return new DamageModifierCustom(processer, priority, tags);
        }
    }

    public class Damage
    {
        public readonly string type;
        public double value;
        private Dictionary<string, Modifier<double>> _modifiers;

        internal Damage(string type, float value, Dictionary<string, Modifier<double>>? modifiers = null)
        {
            this.type = type;
            this.value = value;
            _modifiers = modifiers?.Any() == true ? modifiers : new();
        }

        //添加、合并或替换
        internal void AddModifier(string mdfid, Modifier<double> mdf)
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
            List<Modifier<double>> modifiers = _modifiers.Values.ToList();
            modifiers.Sort((x, y) => x.CompareTo(y));
            double value = this.value;
            foreach (var mdf in modifiers)
            {
                if (!DamageTypes.Ignore(type, mdf.tags))
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
