using MineCombat;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardData cardData;

    private void Start()
    {
        // 1. 注册名字
        if (!ACard.HasTranslatorValue(cardData.id))
        {
            // 注册翻译器
            Debug.Log("Registering card translator for " + cardData.Name);
            ACard.RegisterTranslator(cardData.id, cardData.Name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 现在可以安全创建 Card
            Card card = new Card(cardData);

            // 创建 CardView
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);

            StartCoroutine(handView.AddCard(cardView));
        }
    }
}

