using UnityEngine;
using MineCombat;

namespace MineCombat
{
    /// <summary>
    /// 拖拽系统设置工具，帮助快速配置拖拽出牌系统
    /// </summary>
    public class DragSystemSetup : MonoBehaviour
    {
        [Header("自动设置")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool addDragBehaviorToCards = true;
        [SerializeField] private bool createPlayAreas = true;
        
        [Header("出牌区域设置")]
        [SerializeField] private GameObject playAreaPrefab;
        [SerializeField] private Vector3[] playAreaPositions = new Vector3[]
        {
            new Vector3(0, -3, 0),
            new Vector3(-2, -3, 0),
            new Vector3(2, -3, 0)
        };
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupDragSystem();
            }
        }
        
        [ContextMenu("设置拖拽系统")]
        public void SetupDragSystem()
        {
            Debug.Log("开始设置拖拽系统...");
            
            // 1. 确保CardDragSystem存在
            SetupCardDragSystem();
            
            // 2. 为现有卡牌添加拖拽行为
            if (addDragBehaviorToCards)
            {
                SetupCardDragBehaviors();
            }
            
            // 3. 创建出牌区域
            if (createPlayAreas)
            {
                SetupPlayAreas();
            }
            
            Debug.Log("拖拽系统设置完成！");
        }
        
        private void SetupCardDragSystem()
        {
            // 查找或创建CardDragSystem
            CardDragSystem dragSystem = FindObjectOfType<CardDragSystem>();
            if (dragSystem == null)
            {
                GameObject dragSystemObj = new GameObject("CardDragSystem");
                dragSystem = dragSystemObj.AddComponent<CardDragSystem>();
                Debug.Log("创建了CardDragSystem");
            }
        }
        
        private void SetupCardDragBehaviors()
        {
            // 为所有CardView添加拖拽行为
            CardView[] cardViews = FindObjectsOfType<CardView>();
            int addedCount = 0;
            
            foreach (CardView cardView in cardViews)
            {
                if (cardView.GetComponent<CardDragBehavior>() == null)
                {
                    cardView.gameObject.AddComponent<CardDragBehavior>();
                    addedCount++;
                }
            }
            
            Debug.Log($"为 {addedCount} 个卡牌添加了拖拽行为");
        }
        
        private void SetupPlayAreas()
        {
            // 创建出牌区域
            GameObject playAreaParent = GameObject.Find("PlayAreas");
            if (playAreaParent == null)
            {
                playAreaParent = new GameObject("PlayAreas");
            }
            
            for (int i = 0; i < playAreaPositions.Length; i++)
            {
                GameObject playAreaObj;
                
                if (playAreaPrefab != null)
                {
                    playAreaObj = Instantiate(playAreaPrefab, playAreaParent.transform);
                }
                else
                {
                    // 创建默认的出牌区域
                    playAreaObj = CreateDefaultPlayArea();
                    playAreaObj.transform.SetParent(playAreaParent.transform);
                }
                
                playAreaObj.name = $"PlayArea_{i + 1}";
                playAreaObj.transform.position = playAreaPositions[i];
                
                // 确保有PlayArea组件
                if (playAreaObj.GetComponent<PlayArea>() == null)
                {
                    playAreaObj.AddComponent<PlayArea>();
                }
            }
            
            Debug.Log($"创建了 {playAreaPositions.Length} 个出牌区域");
        }
        
        private GameObject CreateDefaultPlayArea()
        {
            GameObject playArea = new GameObject("PlayArea");
            
            // 添加SpriteRenderer作为视觉表示
            SpriteRenderer spriteRenderer = playArea.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateDefaultSprite();
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            spriteRenderer.sortingOrder = -1;
            
            // 添加Collider2D用于检测
            BoxCollider2D collider = playArea.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(2f, 1f);
            collider.isTrigger = true;
            
            // 添加PlayArea组件
            PlayArea playAreaComponent = playArea.AddComponent<PlayArea>();
            
            return playArea;
        }
        
        private Sprite CreateDefaultSprite()
        {
            // 创建一个简单的白色方块作为默认出牌区域
            Texture2D texture = new Texture2D(64, 32);
            Color[] pixels = new Color[64 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 64, 32), new Vector2(0.5f, 0.5f));
        }
        
        [ContextMenu("清理拖拽系统")]
        public void CleanupDragSystem()
        {
            // 移除所有CardDragBehavior
            CardDragBehavior[] dragBehaviors = FindObjectsOfType<CardDragBehavior>();
            foreach (var behavior in dragBehaviors)
            {
                DestroyImmediate(behavior);
            }
            
            // 移除所有PlayArea
            PlayArea[] playAreas = FindObjectsOfType<PlayArea>();
            foreach (var playArea in playAreas)
            {
                DestroyImmediate(playArea.gameObject);
            }
            
            // 移除CardDragSystem
            CardDragSystem dragSystem = FindObjectOfType<CardDragSystem>();
            if (dragSystem != null)
            {
                DestroyImmediate(dragSystem.gameObject);
            }
            
            Debug.Log("拖拽系统已清理");
        }
    }
}
