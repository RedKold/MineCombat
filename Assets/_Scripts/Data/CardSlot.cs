using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MineCombat
{
    public class CardSlot : MonoBehaviour
    {
        [SerializeField] private Image cardImage;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Image highlight;

        public void SetCard(Sprite sprite, int count)
        {
            cardImage.sprite = sprite;
            countText.text = count > 1 ? count.ToString() : "";
        }

        public void SetHighlight(bool active)
        {
            highlight.enabled = active;
        }
    }
}