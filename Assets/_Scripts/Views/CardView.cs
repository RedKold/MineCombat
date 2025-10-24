using UnityEngine;
using TMPro;
using MineCombat;
using UnityEngine.Assertions;
namespace MineCombat
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private SpriteRenderer cost;
        [SerializeField] private SpriteRenderer image;

        [SerializeField] private GameObject wrapper;

        public Card Card { get; private set; }

        // 设置卡牌信息
        public void SetCard(string name, string desc, Sprite imageSprite, Sprite costSprite)
        {
            if (title != null) title.text = name;
            if (description != null) description.text = desc;
            if (image != null) image.sprite = imageSprite;
            if (cost != null) cost.sprite = costSprite;
        }

        public void SetCard(Card card)
        {
            
            Card = card;
            title.text = card.Name;
            // description.text = CardDatabaseSystem.Instance.GetCardDescription(card); 
            description.text = card.Description;
            image.sprite = CardDatabaseSystem.Instance.GetCardImage(card.id);
            cost.sprite = CardDatabaseSystem.Instance.GetCostImage(card.cost, card.Xcost);
        }

        // 高亮显示
        public void SetSelected(bool selected)
        {
            float alpha = selected ? 1f : 0.5f;
            Vector3 scale = selected ? Vector3.one * 1.2f : Vector3.one;

            if (image != null) image.color = new Color(1f, 1f, 1f, alpha);
            if (cost != null) cost.color = new Color(1f, 1f, 1f, alpha);

            transform.localScale = scale;

            // make sure the card is rendered above others when selected
            // Z 坐标偏移实现置顶
            Vector3 pos = transform.localPosition;
            pos.z = selected ? -1f : 0f;  // 负 Z 靠近相机，高于 0 的其他物体
            transform.localPosition = pos;
        }

        // 显示 wrapper（保持默认显示）
        public void ShowWrapper(bool show)
        {
            wrapper.SetActive(show);
        }

        
        // 处理悬停鼠标方法
        void OnMouseEnter()
        {
            wrapper.SetActive(false);
            Vector3 pos = new(transform.position.x, -2 , 0);
            CardViewHoverSystem.Instance.Show(Card, pos);
        }


        void OnMouseExit()
        {
            CardViewHoverSystem.Instance.Hide();
            wrapper.SetActive(true);
        }

        // 处理鼠标拖拽
        void OnMouseDown()
        {
            
        }
        
    }
}