using UnityEngine;
using MineCombat;

namespace MineCombat
{
    /// <summary>
    /// 出牌区域，检测卡牌是否可以在此区域出牌
    /// </summary>
    public class PlayArea : MonoBehaviour, IPlayArea
    {
        [Header("出牌区域设置")]
        [SerializeField] private bool requireSpecificCardType = false;
        [SerializeField] private string[] allowedCardTypes = new string[0];
        [SerializeField] private int maxCards = -1; // -1表示无限制
        [SerializeField] private bool checkCost = true; // 是否检查费用
        
        [Header("视觉效果")]
        [SerializeField] private SpriteRenderer highlightRenderer;
        [SerializeField] private Color highlightColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;
        
        [Header("状态")]
        [SerializeField] private int currentCardCount = 0;
        [SerializeField] private bool isHighlighted = false;
        
        private Color originalColor;
        
        private void Start()
        {
            // 注册到拖拽系统
            CardDragSystem.Instance.RegisterPlayArea(this);
            
            // 保存原始颜色
            if (highlightRenderer != null)
            {
                originalColor = highlightRenderer.color;
            }
        }
        
        private void OnDestroy()
        {
            // 从拖拽系统注销
            if (CardDragSystem.Instance != null)
            {
                CardDragSystem.Instance.UnregisterPlayArea(this);
            }
        }
        
        public bool CanPlayCard(CardView cardView)
        {
            if (cardView?.Card == null) return false;
            
            // 检查卡牌数量限制
            if (maxCards > 0 && currentCardCount >= maxCards)
            {
                return false;
            }
            
            // 检查卡牌类型限制
            if (requireSpecificCardType && allowedCardTypes.Length > 0)
            {
                bool typeAllowed = false;
                foreach (string cardType in allowedCardTypes)
                {
                    // 这里可以根据实际的卡牌类型系统来判断
                    // 暂时使用卡牌名称作为示例
                    if (cardView.Card.Name.Contains(cardType))
                    {
                        typeAllowed = true;
                        break;
                    }
                }
                if (!typeAllowed) return false;
            }
            
            // 检查费用（这里需要根据实际的费用系统来实现）
            if (checkCost)
            {
                // 示例：检查是否有足够的费用
                // 这里需要连接到实际的费用系统
                // return HasEnoughCost(cardView.Card.cost);
            }
            
            return true;
        }
        
        public bool TryPlayCard(CardView cardView)
        {
            if (!CanPlayCard(cardView)) return false;
            
            // 通过CardSystem执行出牌逻辑
            CardSystem cardSystem = FindObjectOfType<CardSystem>();
            if (cardSystem != null)
            {
                if (cardSystem.PlayCard(cardView, this))
                {
                    // 将卡牌移动到出牌区域
                    cardView.transform.SetParent(transform);
                    cardView.transform.localPosition = Vector3.zero;
                    
                    // 更新卡牌数量
                    currentCardCount++;
                    
                    // 触发出牌事件
                    OnCardPlayed(cardView);
                    
                    return true;
                }
            }
            else
            {
                // 如果没有CardSystem，直接执行出牌逻辑
                Debug.Log($"在 {gameObject.name} 出牌: {cardView.Card.Name}");
                
                // 将卡牌移动到出牌区域
                cardView.transform.SetParent(transform);
                cardView.transform.localPosition = Vector3.zero;
                
                // 更新卡牌数量
                currentCardCount++;
                
                // 触发出牌事件
                OnCardPlayed(cardView);
                
                return true;
            }
            
            return false;
        }
        
        public void SetHighlight(bool highlight)
        {
            if (isHighlighted == highlight) return;
            
            isHighlighted = highlight;
            
            if (highlightRenderer != null)
            {
                if (highlight)
                {
                    // 根据是否可以出牌显示不同颜色
                    bool canPlay = CardDragSystem.Instance.DraggedCard != null && 
                                 CanPlayCard(CardDragSystem.Instance.DraggedCard);
                    highlightRenderer.color = canPlay ? highlightColor : invalidColor;
                    highlightRenderer.enabled = true;
                }
                else
                {
                    highlightRenderer.enabled = false;
                    highlightRenderer.color = originalColor;
                }
            }
        }
        
        /// <summary>
        /// 卡牌出牌后的回调
        /// </summary>
        protected virtual void OnCardPlayed(CardView cardView)
        {
            // 子类可以重写此方法来处理特定的出牌逻辑
        }
        
        /// <summary>
        /// 移除卡牌
        /// </summary>
        public void RemoveCard(CardView cardView)
        {
            if (cardView.transform.parent == transform)
            {
                currentCardCount = Mathf.Max(0, currentCardCount - 1);
                Debug.Log($"从 {gameObject.name} 移除卡牌: {cardView.Card.Name}");
            }
        }
        
        /// <summary>
        /// 清空所有卡牌
        /// </summary>
        public void ClearCards()
        {
            currentCardCount = 0;
            // 销毁所有子卡牌
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<CardView>() != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
        
        /// <summary>
        /// 获取当前卡牌数量
        /// </summary>
        public int GetCardCount()
        {
            return currentCardCount;
        }
        
        /// <summary>
        /// 设置最大卡牌数量
        /// </summary>
        public void SetMaxCards(int max)
        {
            maxCards = max;
        }
        
        /// <summary>
        /// 设置允许的卡牌类型
        /// </summary>
        public void SetAllowedCardTypes(string[] types)
        {
            allowedCardTypes = types;
            requireSpecificCardType = types.Length > 0;
        }
    }
}
