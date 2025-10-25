using UnityEngine;
using MineCombat;
// we better set it as a singleton for easy access
public class CardViewHoverSystem : Singleton<CardViewHoverSystem> 
{
    [SerializeField] private CardView cardViewHover;

    
    public void Show(Card card, Vector3 position)
    {
        cardViewHover.gameObject.SetActive(true);
        
        cardViewHover.SetCard(card);
        cardViewHover.transform.position = position;
    }

    public void Hide()
    {
        cardViewHover.gameObject.SetActive(false);
    }

}

