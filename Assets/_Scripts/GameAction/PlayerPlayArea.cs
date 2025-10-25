using System;
using UnityEngine;
using MineCombat; 

public class PlayerPlayArea : MonoBehaviour, IPlayArea 
{
    private void Start()
    {
        CardDragSystem.Instance.RegisterPlayArea(this);
    }

    private void OnDestroy()
    {
        CardDragSystem.Instance.UnregisterPlayArea(this);
    }
    public bool CanPlayCard(CardView cardView)
    {
        // 在这里添加你对卡牌是否可以被打出的逻辑判断
        // 例如，检查资源、状态等
        return true; // 暂时允许所有卡牌被打出
    }

    public bool TryPlayCard(CardView cardView)
    {
        // 在这里处理卡牌被打出的逻辑
        Debug.Log($"Card {cardView.Card.Name} played in PlayerPlayArea.");
        // 你可以在这里触发游戏行动，更新状态等
        return true;
    }

    public void SetHighlight(bool highlight)
    {
        Debug.Log($"PlayerPlayArea highlight set to: {highlight}");
        return;
        // 可选：实现高亮显示逻辑
    }
}