using UnityEngine;
using MineCombat;
using System.Collections.Generic;

public class CardDatabaseSystem : Singleton<CardDatabaseSystem>
{
    private Dictionary<uint, Sprite> _cardImages = new();
    private Dictionary<string, Sprite> _costImages = new();

    private void Awake()
    {
        LoadCardImages();
        LoadCostImages();
        Debug.Log($"Loaded {_cardImages.Count} card images and {_costImages.Count} cost images.");
    }

    private void LoadCardImages()
    {
        var sprites = Resources.LoadAll<Sprite>("Cards");
        foreach (var sprite in sprites)
        {
            // 假设文件名为 "001_diamond_sword" → 取 001 作为 id
            string name = sprite.name.Split('_')[0];
            if (uint.TryParse(name, out uint id))
                _cardImages[id] = sprite;
        }
    }

    private void LoadCostImages()
    {
        var sprites = Resources.LoadAll<Sprite>("Costs");
        foreach (var sprite in sprites)
        {
            _costImages[sprite.name] = sprite;
        }
    }

    public Sprite GetCardImage(uint cardId)
    {
        return _cardImages.TryGetValue(cardId, out var sprite) ? sprite : null;
    }

    public Sprite GetCostImage(int cost, bool Xcost)
    {
        if (Xcost && _costImages.TryGetValue("X", out var xSprite))
            return xSprite;

        Debug.Log($"Getting cost image for cost: {cost}, Xcost: {Xcost}");
        return _costImages.TryGetValue(cost.ToString(), out var sprite) ? sprite : null;
    }

    public string GetCardDescription(Card card)
    {
        return $"这是 {card.Name} 的卡牌，稀有度：{card.rarity}。";
    }
}


