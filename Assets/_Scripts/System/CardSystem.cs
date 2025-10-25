using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MineCombat;

// we don't need it
namespace MineCombat
{
    /// <summary>
    /// 卡牌系统，管理卡牌的游戏逻辑
    /// </summary>
    public class CardSystem : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private HandView handView;
        [SerializeField] private PlayArea[] playAreas;

        [Header("卡牌设置")]
        [SerializeField] private int maxHandSize = 10;
        [SerializeField] private int currentMana = 3;
        [SerializeField] private int maxMana = 10;

        [Header("事件")]
        [SerializeField] private UnityEngine.Events.UnityEvent<CardView> onCardPlayed;
        [SerializeField] private UnityEngine.Events.UnityEvent<CardView> onCardDrawn;

        private List<CardView> handCards = new List<CardView>();
        private List<CardView> playedCards = new List<CardView>();

        public int CurrentMana => currentMana;
        public int MaxMana => maxMana;
        public int HandSize => handCards.Count;
        public int MaxHandSize => maxHandSize;

        private void Start()
        {
            // 初始化出牌区域
            if (playAreas == null || playAreas.Length == 0)
            {
                playAreas = FindObjectsOfType<PlayArea>();
            }

            // 注册事件
            if (onCardPlayed == null)
                onCardPlayed = new UnityEngine.Events.UnityEvent<CardView>();
            if (onCardDrawn == null)
                onCardDrawn = new UnityEngine.Events.UnityEvent<CardView>();
        }

        /// <summary>
        /// 添加卡牌到手牌
        /// </summary>
        public void AddCardToHand(CardView cardView)
        {
            if (handCards.Count >= maxHandSize)
            {
                Debug.LogWarning("手牌已满，无法添加更多卡牌");
                return;
            }

            handCards.Add(cardView);
            StartCoroutine(handView.AddCard(cardView));

            onCardDrawn?.Invoke(cardView);
            Debug.Log($"添加卡牌到手牌: {cardView.Card?.Name}");
        }

        /// <summary>
        /// 从手牌移除卡牌
        /// </summary>
        public void RemoveCardFromHand(CardView cardView)
        {
            if (handCards.Remove(cardView))
            {
                Debug.Log($"从手牌移除卡牌: {cardView.Card?.Name}");
            }
        }

        /// <summary>
        /// 出牌
        /// </summary>
        public bool PlayCard(CardView cardView, PlayArea playArea)
        {
            if (cardView?.Card == null || playArea == null)
            {
                Debug.LogWarning("无效的卡牌或出牌区域");
                return false;
            }

            // 检查费用
            if (!CanAffordCard(cardView.Card))
            {
                Debug.LogWarning($"费用不足，无法出牌: {cardView.Card.Name}");
                return false;
            }

            // 检查出牌区域是否接受此卡牌
            if (!playArea.CanPlayCard(cardView))
            {
                Debug.LogWarning($"出牌区域不接受此卡牌: {cardView.Card.Name}");
                return false;
            }

            // 消耗费用
            ConsumeMana(cardView.Card.cost);

            // 从手牌移除
            RemoveCardFromHand(cardView);

            // 添加到已出卡牌列表
            playedCards.Add(cardView);

            // 触发出牌事件
            onCardPlayed?.Invoke(cardView);

            Debug.Log($"成功出牌: {cardView.Card.Name}");
            return true;
        }

        /// <summary>
        /// 检查是否有足够费用
        /// </summary>
        public bool CanAffordCard(Card card)
        {
            if (card == null) return false;

            // 这里可以添加更复杂的费用检查逻辑
            return currentMana >= card.cost;
        }

        /// <summary>
        /// 消耗费用
        /// </summary>
        private void ConsumeMana(int cost)
        {
            currentMana = Mathf.Max(0, currentMana - cost);
            Debug.Log($"消耗费用: {cost}, 剩余费用: {currentMana}");
        }

        /// <summary>
        /// 恢复费用
        /// </summary>
        public void RestoreMana(int amount)
        {
            currentMana = Mathf.Min(maxMana, currentMana + amount);
            Debug.Log($"恢复费用: {amount}, 当前费用: {currentMana}");
        }

        /// <summary>
        /// 设置最大费用
        /// </summary>
        public void SetMaxMana(int max)
        {
            maxMana = Mathf.Max(1, max);
            currentMana = Mathf.Min(currentMana, maxMana);
        }

        /// <summary>
        /// 清空手牌
        /// </summary>
        public void ClearHand()
        {
            foreach (var card in handCards)
            {
                if (card != null)
                {
                    DestroyImmediate(card.gameObject);
                }
            }
            handCards.Clear();
        }

        /// <summary>
        /// 清空已出卡牌
        /// </summary>
        public void ClearPlayedCards()
        {
            foreach (var card in playedCards)
            {
                if (card != null)
                {
                    DestroyImmediate(card.gameObject);
                }
            }
            playedCards.Clear();
        }

        /// <summary>
        /// 获取手牌列表
        /// </summary>
        public List<CardView> GetHandCards()
        {
            return new List<CardView>(handCards);
        }

        /// <summary>
        /// 获取已出卡牌列表
        /// </summary>
        public List<CardView> GetPlayedCards()
        {
            return new List<CardView>(playedCards);
        }
    }
}
