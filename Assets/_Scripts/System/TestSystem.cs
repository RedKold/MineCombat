using MineCombat;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardData cardData;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 现在可以安全创建 Card
            Card card = Card.Create(cardData);

            // 创建 CardView
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);

            StartCoroutine(handView.AddCard(cardView));
        }
    }
}

