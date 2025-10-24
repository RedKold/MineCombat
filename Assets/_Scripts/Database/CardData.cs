using UnityEngine;

// for we can create CardData assets in Unity Editor
[CreateAssetMenu(menuName = "Database/Card")]
public class CardData : ScriptableObject
{
    public uint id;
    public string Name;
    public string rarity;
    public int cost;
    public bool Xcost;

    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
}
