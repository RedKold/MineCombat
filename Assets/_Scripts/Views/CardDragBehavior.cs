using UnityEngine;
using UnityEngine.EventSystems;
using MineCombat;

namespace MineCombat
{
    /// <summary>
    /// 卡牌拖拽行为组件，处理卡牌的拖拽输入
    /// </summary>
    [RequireComponent(typeof(CardView))]
    public class CardDragBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("拖拽设置")]
        [SerializeField] private bool enableDrag = true;
        [SerializeField] private float dragThreshold = 0.1f;
        
        private CardView cardView;
        private Vector3 startPosition;
        private bool isPointerDown = false;
        private bool hasStartedDrag = false;
        
        private void Awake()
        {
            cardView = GetComponent<CardView>();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enableDrag || cardView?.Card == null) return;
            
            isPointerDown = true;
            hasStartedDrag = false;
            startPosition = transform.position;
            
            Debug.Log($"点击卡牌: {cardView.Card.Name}");
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isPointerDown) return;
            
            isPointerDown = false;
            
            if (!hasStartedDrag)
            {
                // 如果没有开始拖拽，则视为点击
                OnCardClicked();
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!enableDrag || !isPointerDown || cardView?.Card == null) return;
            
            hasStartedDrag = true;
            
            // 开始拖拽
            CardDragSystem.Instance.StartDrag(cardView);
            
            Debug.Log($"开始拖拽卡牌: {cardView.Card.Name}");
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!hasStartedDrag || !CardDragSystem.Instance.IsDragging) return;
            
            // 拖拽位置由CardDragSystem处理
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!hasStartedDrag) return;
            
            hasStartedDrag = false;
            
            // 结束拖拽
            CardDragSystem.Instance.EndDrag();
            
            Debug.Log($"结束拖拽卡牌: {cardView.Card.Name}");
        }
        
        /// <summary>
        /// 卡牌点击事件
        /// </summary>
        private void OnCardClicked()
        {
            Debug.Log($"点击卡牌: {cardView.Card.Name}");
            
            // 可以在这里添加点击卡牌的逻辑，比如显示详细信息
            // 或者切换选中状态
            cardView.SetSelected(!(cardView.transform.localScale.x > 1.0f));
        }
        
        /// <summary>
        /// 设置是否启用拖拽
        /// </summary>
        public void SetDragEnabled(bool enabled)
        {
            enableDrag = enabled;
        }
        
        /// <summary>
        /// 获取是否启用拖拽
        /// </summary>
        public bool IsDragEnabled()
        {
            return enableDrag;
        }
        
        /// <summary>
        /// 强制取消拖拽
        /// </summary>
        public void CancelDrag()
        {
            if (hasStartedDrag)
            {
                CardDragSystem.Instance.CancelDrag();
                hasStartedDrag = false;
            }
        }
        
        private void OnDisable()
        {
            // 如果组件被禁用，取消拖拽
            CancelDrag();
        }
    }
}
