using UnityEngine;
using TMPro;
using MineCombat;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Numerics;
using Unity.VisualScripting;
namespace MineCombat
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private SpriteRenderer cost;
        [SerializeField] private SpriteRenderer image;

        [SerializeField] private GameObject wrapper;

        [SerializeField] private GameObject dragger_behaviour;
        [SerializeField] private LayerMask playAreaLayerMask;

        // All cards need a player
        [SerializeField] private Player _owner;
        public Player Owner => _owner;

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
            // Assert if load Description failed    
            Assert.IsNotNull(card.Description, $"Card description is null for card id: {card.id}");
            description.text = card.Description;
            image.sprite = CardDatabaseSystem.Instance.GetCardImage(card.id);
            cost.sprite = CardDatabaseSystem.Instance.GetCostImage(card.cost, card.Xcost);
        }

        // 高亮显示
        public void SetSelected(bool selected)
        {
            float alpha = selected ? 1f : 0.5f;
            UnityEngine.Vector3 scale = selected ? UnityEngine.Vector3.one * 1.2f : UnityEngine.Vector3.one;

            if (image != null) image.color = new Color(1f, 1f, 1f, alpha);
            if (cost != null) cost.color = new Color(1f, 1f, 1f, alpha);

            transform.localScale = scale;

            // make sure the card is rendered above others when selected
            // Z 坐标偏移实现置顶
            UnityEngine.Vector3 pos = transform.localPosition;
            pos.z = selected ? -1f : 0f;  // 负 Z 靠近相机，高于 0 的其他物体
            transform.localPosition = pos;
        }

        // 显示 wrapper（保持默认显示）
        public void ShowWrapper(bool show)
        {
            wrapper.SetActive(show);
        }


        // 处理悬停鼠标方法

        // 用 EventSystem 悬停接口替换 OnMouseEnter/Exit
        void OnMouseEnter()
        {
            if (!InteractionSystem.Instance.CanHover()) return;

            InteractionSystem.Instance.BeginHover();

            // 关闭原来卡牌视图的显示，避免重叠
            ShowWrapper(false);
            UnityEngine.Vector3 hoverPos = transform.position + UnityEngine.Vector3.up * 1.5f;
            CardViewHoverSystem.Instance.Show(Card, hoverPos);
        }

        void OnMouseExit()
        {
            InteractionSystem.Instance.EndHover();
            
            CardViewHoverSystem.Instance.Hide();
            ShowWrapper(true);
        }

        void OnMouseDown()
        {
            if(!InteractionSystem.Instance.CanDrag())
            {
                Debug.Log("Cannot drag card due to interaction lock.");
                return;
            }
            ShowWrapper(true);
            CardDragSystem.Instance.StartDrag(this,GetComponentInParent<HandView>());
        }

        void OnMouseUp()
        {
            Debug.Log("Mouse up on card.");
            CardDragSystem.Instance.EndDrag();
        }
    }
}