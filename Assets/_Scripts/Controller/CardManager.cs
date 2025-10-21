using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public static class CardManager
    {
        private static Dictionary<string, Card> _cards = new(97);
        private static ConstRandomizer<Card>[] rarity_rdms = new ConstRandomizer<Card>[6];
        private static Dictionary<string, ConstRandomizer<Card>> tag_rdms = new(17);
        private static bool _const = false;

        public static void Add(string id, Card card)
        {
            if (_const)
                throw new ArgumentException("不能在游戏中添加新的卡牌模板");
            if (_cards.ContainsKey(id))
                throw new ArgumentException($"重复的卡牌ID：{id}");
            _cards.Add(id, card);
        }

        public static Card Get(string id)
        {
            if (_cards.TryGetValue(id, out Card? card))
            {
                if (_const)
                    return card.Clone();
                return card;
            }
            throw new ArgumentException($"不存在的卡牌ID：{id}");
        }

        public static void CostCast()
        {
            _const = true;
        }

        static CardManager()
        {

        }
    }
}
