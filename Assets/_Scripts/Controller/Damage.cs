using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MineCombat
{
    public static class DamageModifiers
    {
        public readonly static TagsManager Tags = new();

        static DamageModifiers()
        {

        }

        public static DamageModifierAdd CreateAdd(double value, uint priority, ITags tags)
        {
            return new DamageModifierAdd(value, priority, tags);
        }
        public static DamageModifierAdd CreateAdd(double value, uint priority)
        {
            return new DamageModifierAdd(value, priority, StaticTags.Empty);
        }

        public static DamageModifierMul CreateMul(double value, uint priority, ITags tags)
        {
            return new DamageModifierMul(value, priority, tags);
        }
        public static DamageModifierMul CreateMul(double value, uint priority)
        {
            return new DamageModifierMul(value, priority, StaticTags.Empty);
        }

        public static DamageModifierMulTotal CreateMulTotal(double value, uint priority, ITags tags)
        {
            return new DamageModifierMulTotal(value, priority, tags);
        }
        public static DamageModifierMulTotal CreateMulTotal(double value, uint priority)
        {
            return new DamageModifierMulTotal(value, priority, StaticTags.Empty);
        }

        public static DamageModifierCustom CreateCustom(Process<double> processer, uint priority, ITags tags)
        {
            return new DamageModifierCustom(processer, priority, tags);
        }
        public static DamageModifierCustom CreateCustom(Process<double> processer, uint priority)
        {
            return new DamageModifierCustom(processer, priority, StaticTags.Empty);
        }
    }

    public class Damage
    {
#nullable enable
        internal readonly string type;
        internal double value;
        private Dictionary<string, IModifier<Damage>> _modifiers;

        internal Damage(string type, float value, Dictionary<string, IModifier<Damage>>? modifiers = null)
        {
            this.type = type;
            this.value = value;
            _modifiers = modifiers?.Any() == true ? modifiers : new();
        }

        //添加、合并或替换
        internal void AddModifier(string mdfid, Modifier<Damage> mdf, bool replaceTags = true, bool mergeTags = false)
        {
            if (!(_modifiers.ContainsKey(mdfid) && _modifiers[mdfid].TryMerge(mdf, replaceTags, mergeTags)))
                _modifiers[mdfid] = mdf;
        }
        //该方法只能沿用最初的tags；它是性能优化版，视情况选择是否将字符串转为Tags，减少开销，适合只少量创建的可合并modifiers
        internal void AddModifier(string mdfid, Func<double, uint, ITags, DamageModifier> creator, double value, uint priority, string? tags = null)
        {
            if (!(_modifiers.ContainsKey(mdfid) && _modifiers[mdfid].TryMerge(creator(value, priority, StaticTags.Empty), false, false)))
                _modifiers[mdfid] = creator(value, priority, tags is not null ? (Tags)tags : StaticTags.Empty);
        }
        internal void AddModifier(string mdfid, Func<Process<double>, uint, ITags, DamageModifier> creator, Process<double> value, uint priority, string? tags = null)
        {
            if (!(_modifiers.ContainsKey(mdfid) && _modifiers[mdfid].TryMerge(creator(value, priority, StaticTags.Empty), false, false)))
                _modifiers[mdfid] = creator(value, priority, tags is not null ? (Tags)tags : StaticTags.Empty);
        }

        //添加或替换
        internal void UpdateModifier(string mdfid, Modifier<Damage> mdf)
        {
            _modifiers[mdfid] = mdf;
        }

        internal bool RemoveModifier(string mdfid)
        {
            return _modifiers.Remove(mdfid);
        }

        internal double Get()
        {
            EventManager.Trigger("DamageProcess", this);
            List<IModifier<Damage>> modifiers = _modifiers.Values.ToList();
            modifiers.Sort((x, y) => x.CompareTo(y));
            double value = this.value;
            foreach (var mdf in modifiers)
            {
                mdf.Process(this);
            }
            double result = this.value;
            this.value = value;
            return result;
        }
#nullable disable
    }

    public static class DamageTags 
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

        static DamageTags()
        {
            types_tags_table.AddorMerge("mc_magic", (StaticTags)"{mc_bypass_armor, mc_bypass_test}");
        }
    }
}
