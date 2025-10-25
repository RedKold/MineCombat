using UnityEngine;
using MineCombat;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace MineCombat
{
    /// <summary>
    /// 卡牌拖拽系统，管理卡牌的拖拽和出牌逻辑
    /// </summary>
    public class CardDragSystem : Singleton<CardDragSystem>
    {
        [Header("拖拽设置")]
        [SerializeField] private float dragThreshold = 0.1f; // 拖拽阈值
        [SerializeField] private float dragScale = 1.2f; // 拖拽时的缩放
        [SerializeField] private LayerMask playAreaLayer = 1; // 出牌区域层级

        [Header("拖拽状态")]
        [SerializeField] private bool isDragging = false;
        [SerializeField] private CardView draggedCard = null;

        [SerializeField] private HandView handView = null;
        [SerializeField] private Vector3 originalPosition;
        [SerializeField] private Vector3 originalScale;

        [SerializeField] private Quaternion originalRotation;
        [SerializeField] private int originalSortingOrder;

        private Camera mainCamera;
        private List<IPlayArea> playAreas = new List<IPlayArea>();

        public bool IsDragging => isDragging;
        public CardView DraggedCard => draggedCard;

        protected override void Awake()
        {
            base.Awake();
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            if (isDragging && draggedCard != null)
            {
                UpdateDragPosition();
                CheckPlayArea();
            }
        }

        /// <summary>
        /// 开始拖拽卡牌
        /// </summary>
        public void StartDrag(CardView cardView, HandView fromHand = null)
        {
            if (!InteractionSystem.Instance.CanDrag())
                return;

            draggedCard = cardView;
            handView = fromHand;
            isDragging = true;

        Debug.Log("Removing card from hand view.");
            // 保存原始状态
            InteractionSystem.Instance.BeginDrag();

            originalPosition = cardView.transform.position;
            originalScale = cardView.transform.localScale;
            originalRotation = cardView.transform.rotation;


            // 恢复旋转状态到不旋转
            cardView.transform.rotation = Quaternion.identity;

            // 设置拖拽状态
            cardView.transform.localScale = originalScale * dragScale;
            // cardView.SetSelected(true);

            // 提高渲染层级
            var spriteRenderer = cardView.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalSortingOrder = spriteRenderer.sortingOrder;
                spriteRenderer.sortingOrder = 1000; // 确保在最上层
            }

            Debug.Log($"开始拖拽卡牌: {cardView.Card?.Name}");
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public void EndDrag()
        {
            if (!isDragging || draggedCard == null) return;


            bool played = false;

            // 检查是否在出牌区域内
            // foreach (var playArea in playAreas)
            // {
            //     if (playArea.CanPlayCard(draggedCard))
            //     {
            //         if (playArea.TryPlayCard(draggedCard))
            //         {
            //             played = true;
            //             Debug.Log($"成功出牌: {draggedCard.Card?.Name}");
            //             break;
            //         }
            //     }
            // }

            Debug.DrawRay(draggedCard.transform.position, Vector3.forward * 10, Color.red, 1f);

            // Try our way to check if the card is played. use Ray
            if(Physics.Raycast(draggedCard.transform.position, Vector3.forward, out RaycastHit hitInfo, 10f, playAreaLayer))
            {
                Debug.Log($"Raycast hit: {hitInfo.collider.name}");
                IPlayArea playArea = hitInfo.collider.GetComponent<IPlayArea>();
                Assert.IsNotNull(playArea, "PlayArea component not found on hit collider.");
                if(playArea != null && playArea.CanPlayCard(draggedCard))
                {
                    if(playArea.TryPlayCard(draggedCard))
                    {
                        played = true;
                        Debug.Log($"成功出牌: {draggedCard.Card?.Name}");

                        // Remove the card from hand view
                        // 广播
                        Assert.IsNotNull(handView, "HandView is null when trying to remove played card.");
                        handView.StartCoroutine(handView.RemoveCard(draggedCard));
                    }
                }
            }

            if (!played)
            {
                // 回到原始位置
                Debug.Log($"未能出牌，返回原位: {draggedCard.Card?.Name}");
                ReturnToOriginalPosition();
            }

            
            // 重置状态
            ResetDragState();


            // 通知交互系统
            InteractionSystem.Instance.EndDrag();

            Debug.Log("结束拖拽");
        }

        /// <summary>
        /// 取消拖拽
        /// </summary>
        public void CancelDrag()
        {
            
            if (!isDragging) return;

            ReturnToOriginalPosition();
            ResetDragState();
            // 通知交互系统
            InteractionSystem.Instance.EndDrag();
            Debug.Log("拖拽已取消");
        }

        /// <summary>
        /// 注册出牌区域
        /// </summary>
        public void RegisterPlayArea(IPlayArea playArea)
        {
            if (!playAreas.Contains(playArea))
            {
                playAreas.Add(playArea);
            }
        }

        /// <summary>
        /// 注销出牌区域
        /// </summary>
        public void UnregisterPlayArea(IPlayArea playArea)
        {
            playAreas.Remove(playArea);
        }

        private void UpdateDragPosition()
        {
            Assert.IsNotNull(mainCamera, "Main Camera is null in CardDragSystem");
            Assert.IsNotNull(draggedCard, "Dragged Card is null in CardDragSystem");
            if (mainCamera == null || draggedCard == null) return;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = mainCamera.WorldToScreenPoint(draggedCard.transform.position).z;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            draggedCard.transform.position = worldPosition;
        }

        private void CheckPlayArea()
        {
            if (draggedCard == null) return;

            // 高亮可出牌的区域
            foreach (var playArea in playAreas)
            {
                bool canPlay = playArea.CanPlayCard(draggedCard);
                playArea.SetHighlight(canPlay);
            }
        }

        private void ReturnToOriginalPosition()
        {
            if (draggedCard == null) return;

            draggedCard.transform.position = originalPosition;
            draggedCard.transform.localScale = originalScale;
            draggedCard.transform.rotation = originalRotation;

            var handView = draggedCard.GetComponentInParent<HandView>();
            handView?.RefreshLayout();

            // draggedCard.SetSelected(false);

            // 恢复渲染层级
            var spriteRenderer = draggedCard.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = originalSortingOrder;
            }
        }

        private void ResetDragState()
        {
            // 清除所有区域的高亮
            foreach (var playArea in playAreas)
            {
                playArea.SetHighlight(false);
            }

            isDragging = false;
            draggedCard = null;
        }
    }

    /// <summary>
    /// 出牌区域接口
    /// </summary>
    public interface IPlayArea
    {
        bool CanPlayCard(CardView cardView);
        bool TryPlayCard(CardView cardView);
        void SetHighlight(bool highlight);
    }

}