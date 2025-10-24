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

    //标识目标选择方式
    public enum Target : byte
    {
        [Description("指定实体")] Selected = 0,
        [Description("敌方实体")] Enemy = 1,
        [Description("所有实体")] All = 2,
        [Description("随机实体")] Random = 3,
        [Description("自身")] Self = 4
    }

    abstract public class ACard : Properties, IEquatable<ACard>
    {
#nullable enable
        protected static KeyValueTranslator<string> _translator = new(97, true);

        public readonly uint id;
        public readonly Rarity rarity;
        public readonly ITags tags;
        public string Name => _translator.Translate(id);

        // add static method to access translator, for convenience. You can make it more grateful
        public static void RegisterTranslator(uint id, string name)
        {
            _translator.Translate(name, true); // 或 _translator.Register(id, name) 如果你有 Register 方法
        }

        public static bool HasTranslatorValue(uint id)
        {
            return _translator.IsValid(id);
        }


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
#nullable disable
    }

    public sealed class Card : ACard, ICloneable<Card>
    {
#nullable enable
        public readonly byte cost;
        public readonly bool Xcost;
        public readonly Target target;
        private readonly Box<string>?[] _commands;

        internal IReadOnlyList<Box<string>?> Commands => _commands.AsReadOnly();

        // 卡牌的描述信息
        public string Description => data != null ? data.Description : string.Empty;

        // Judge data is null or not;
        private readonly CardData? data;
    

        private Card(uint id, byte cost, bool Xcost, Rarity rarity, ITags tags, Action<Entity, Card, Box<Entity>, Box<string>>? action) : base(id, rarity, tags)
        {
            this.cost = cost;
            this.Xcost = Xcost;
            this.target = target;
            _commands = commands;
        }

        private Card(Card src) : base(src)
        {
            cost = src.cost;
            Xcost = src.Xcost;
            target = src.target;
            _commands = src._commands;
        }

         // 内部构造函数，只能在同程序集或类内部使用
        internal Card(CardData data)
            : base(
                data.id,
                Enum.TryParse<Rarity>(data.rarity, out var r) ? r : Rarity.Common,
                new Tags() // 如果 CardData 有 tags，可以用 data.tags
            )
        {
            this.data = data;
            this.cost = (byte)data.cost;
            this.Xcost = data.Xcost;
            this.action = null; // 根据需要可以修改
        }

        internal static Card Create(string name, byte cost, bool Xcost, Rarity rarity, ITags tags, Action<Entity, Card, Box<Entity>, Box<string>>? action = null)
        {
            return new Card(_translator.Translate(name), cost, Xcost, rarity, tags, target, Parser.ToBoxArray(commands) ?? new Box<string>?[3]);
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