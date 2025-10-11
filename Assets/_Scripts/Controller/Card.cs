using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineCombat
{
    public sealed class Card : Properties
    {
        public enum Rarity 
        { 
            COMMON = (ushort)0,
            UNCOMMON = (ushort)1,
            RARE = (ushort)2,
            EPIC = (ushort)3,
            LEGEND = (ushort)4,
            UNIQUE = (ushort)5
        }

        public readonly short cost;
        public readonly Rarity rarity;
        public readonly Action action;

        internal Card(short cost, Rarity rarity, Action action)
        {
            this.cost = cost;
            this.rarity = rarity;
            this.action = action;
        }
    }
}
