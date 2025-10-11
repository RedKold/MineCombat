using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MineCombat
{
    public enum Rarity : byte
    {
        [Description("普通")] Common = 0,
        [Description("不凡")] Uncommon = 1,
        [Description("稀有")] Rare = 2,
        [Description("史诗")] Epic = 3,
        [Description("传说")] Legend = 4,
        [Description("唯一")] Unique = 5
    }

    public sealed class Card : Properties
    {
#nullable enable
        public readonly byte cost;
        public readonly bool Xcost;
        public readonly Rarity rarity;
        public readonly ITags tags;
        public readonly Action<Card>? action;

        internal Card(byte cost, bool Xcost, Rarity rarity, ITags tags, Action<Card>? action = null)
        {
            this.cost = cost;
            this.Xcost = Xcost;
            this.rarity = rarity;
            this.tags = tags;
            this.action = action;
        }

        internal Card(byte cost, bool Xcost, Rarity rarity) : this(cost, Xcost, rarity, StaticTags.Empty) { }
#nullable disable
    }
}
