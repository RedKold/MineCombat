using System;
using UnityEngine;
using MineCombat;
using UnityEngine.Assertions;
using TMPro;
/**
    This file provide a interface for IPlayArea
    Though it has the name "Area", but it not actually as the check if card hit any area. 
*/
public class PlayerPlayArea : MonoBehaviour, IPlayArea
{
    [SerializeField] private Player _owner; // Inspector assign
    public Player Owner => _owner;
    private void Start()
    {
        CardDragSystem.Instance.RegisterPlayArea(this);
        Player _p = this.GetComponentInParent<CombatantView>().player;
        SetPlayer(_p);

        if(_p == null)
        {
            Debug.LogWarning("Set the player for PlayerPlayArea failed. set default global player");
            SetPlayer(SinglePlayerSystem.Instance.getPlayer());
        }
        else
        {
            Debug.Log($"Set the player succeed, name is {_p.Name}");
        }
    }

    // 设置玩家
    public void SetPlayer(Player p)
    {
        if (p is null)
        {
            Debug.LogWarning("Player is null! Try get Default single player!");
            _owner = SinglePlayerSystem.Instance.getPlayer();
            return;
        }
        _owner = p;
        return;
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

    public bool TryPlayCard(CardView cardView, Box<Entity>? targets)
    {

        if (targets == null)
        {
            Debug.Log("target is null, or it's a card no target");
            // add your things.
        }
        // 在这里处理卡牌被打出的逻辑
        Debug.Log($"Card {cardView.Card.Name} played in PlayerPlayArea.");
        // 你可以在这里触发游戏行动，更新状态等

        Player player = cardView.Owner;
        
        if(player == null && SinglePlayerSystem.Instance.isSingleGame)
        {
            player = SinglePlayerSystem.Instance.getPlayer();
        }

        // Assert.IsNotNull(player, "The owner of the card is null!");

        player.Play(0, targets);

        return true;
    }

    public void SetHighlight(bool highlight)
    {
        Debug.Log($"PlayerPlayArea highlight set to: {highlight}");
        return;
        // 可选：实现高亮显示逻辑
    }
}