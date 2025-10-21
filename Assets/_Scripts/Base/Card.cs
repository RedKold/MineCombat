using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MineCombat
{
    public interface ICloneable<T>
    {
        public T Clone();
    }

    public enum Rarity : byte
    {
        [Description("普通")] Common = 0,
        [Description("不凡")] Uncommon = 1,
        [Description("稀有")] Rare = 2,
        [Description("史诗")] Epic = 3,
        [Description("传说")] Legend = 4,
        [Description("唯一")] Unique = 5
    }

    abstract public class ACard : Properties, IEquatable<ACard>
    {
        protected static KeyValueTranslator<string> _translator = new(97, true);

        public readonly uint id;
        public readonly Rarity rarity;
        public readonly ITags tags;
        public string Name => _translator.Translate(id);

        protected ACard(uint id, Rarity rarity, ITags tags)
        {
            this.id = id;
            this.rarity = rarity;
            this.tags = tags;
        }

        protected ACard(ACard src) : base(src)
        {
            id = src.id;
            rarity = src.rarity;
            tags = src.tags;
        }

        public bool Equals(ACard? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return id == other?.id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            return Equals(obj as ACard);
        }

        public override int GetHashCode()
        {
            return (int)id;
        }
    }

    public sealed class Card : ACard, ICloneable<Card>
    {
#nullable enable
        public readonly byte cost;
        public readonly bool Xcost;
        public readonly Action<Entity, Card, Box<Entity>, Box<string>>? action;

        private Card(uint id, byte cost, bool Xcost, Rarity rarity, ITags tags, Action<Entity, Card, Box<Entity>, Box<string>>? action) : base(id, rarity, tags)
        {
            this.cost = cost;
            this.Xcost = Xcost;
            this.action = action;
        }

        private Card(Card src) : base(src)
        {
            cost = src.cost;
            Xcost = src.Xcost;
            action = src.action;
        }

        internal static Card Create(string name, byte cost, bool Xcost, Rarity rarity, ITags tags, Action<Entity, Card, Box<Entity>, Box<string>>? action = null)
        {
            uint id = _translator.Translate(name);
            return new Card(id, cost, Xcost, rarity, tags, action);
        }

        public new Card Clone()
        {
            return new Card(this);
        }
#nullable disable
    }

    public sealed class Material : ACard, ICloneable<Material>
    {
        private Material(uint id, Rarity rarity, ITags tags) : base(id, rarity, tags) { }

        private Material(Material src) : base(src) { }

        public new Material Clone()
        {
            return new Material(this);
        }
    }
}