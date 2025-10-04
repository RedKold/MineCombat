using UnityEngine;

public class CardBar : MonoBehaviour
{
    [SerializeField] private CardView cardPrefab;          // 卡牌预制体
    [SerializeField] private int cardCount = 10;           // 卡牌数量
    [SerializeField] private SpriteRenderer background;    // 背景 SpriteRenderer

    private CardView[] cards;
    private int currentIndex = 0;

    void Start()
    {
        if (cardPrefab == null)
        {
            Debug.LogError("请在 CardBar 中拖入 cardPrefab！");
            return;
        }

        if (background == null)
        {
            Debug.LogError("请在 CardBar 中拖入背景 SpriteRenderer！");
            return;
        }

        cards = new CardView[cardCount];

        // 根据背景宽度计算 spacing
        float bgWidth = background.bounds.size.x;
        float spacing = bgWidth / cardCount;
        float startX = -bgWidth / 2f + spacing / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            CardView card = Instantiate(cardPrefab, transform);

            // 横向排列
            card.transform.localPosition = new Vector3(startX + i * spacing, 0, 0);

            // 显示 wrapper
            card.ShowWrapper(true);

            cards[i] = card;

            // 设置示例图片
            Sprite img = Resources.Load<Sprite>($"Sprites/card{i}");
            Sprite cost = Resources.Load<Sprite>("Sprites/cost");
            card.SetCard($"Card {i+1}", $"Description {i+1}", img, cost);
        }

        UpdateSelection();
    }

    void Update()
    {
        if (cards == null) return;

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            KeyCode key = (i < 9) ? KeyCode.Alpha1 + i : KeyCode.Alpha0; // 0 键对应第 10 张
            if (Input.GetKeyDown(key))
            {
                currentIndex = i;
                UpdateSelection();
            }
        }
    }

    private void UpdateSelection()
    {
        if (cards == null) return;

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;
            cards[i].SetSelected(i == currentIndex);
        }
    }
}
