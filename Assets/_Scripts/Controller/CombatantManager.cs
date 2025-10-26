using UnityEngine;
using MineCombat;

public class CombatManager : MonoBehaviour
{
    [Header("Prefab & Parent")]
    [SerializeField] private CombatantView combatantViewPrefab; // 战斗者视图预制体
    [SerializeField] private Transform viewParent;              // 父物体，用于组织层级

    [Header("战斗者配置")]
    [SerializeField] private int playerCount = 1;            // 玩家数量
    [SerializeField] private int enemyCount = 1;             // 敌人数量
    [SerializeField] private float spacing = 100f;           // 战斗者间距
    [SerializeField] private Sprite[] combatantSprites;      // 可选角色贴图



    private Player[] players;


    // CombatantManager. Init 2 enemy
    void Start()
    {
        if (combatantViewPrefab == null || viewParent == null)
        {
            Debug.LogError("请在 CombatManager 中拖入 CombatantView 预制体和父物体！");
            return;
        }

        int combatantCount = playerCount + enemyCount;
        players = new Player[combatantCount];

        // 初始化玩家和敌人
        players[0] = new Player("Hero (Player)", 100.0);
        players[1] = new Player("Creeper (Enemy)", 75.0);

        // 居中排列计算
        float totalWidth = (combatantCount - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < combatantCount; i++)
        {
            Vector3 localPos = new Vector3(startX + i * spacing, 0f, 0f);

            CombatantView view = Instantiate(combatantViewPrefab, viewParent);
            view.transform.localPosition = localPos;
            view.transform.localRotation = Quaternion.identity;

            // 绑定 Player 数据
            view.BindPlayer(players[i]);


            // 绑定 PlayerArea 数据


            // 可选设置贴图
            if (combatantSprites != null && i < combatantSprites.Length)
            {
                SpriteRenderer sr = view.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.sprite = combatantSprites[i];
            }
        }

        // 测试受伤
        Invoke(nameof(SimulateDamage), 2f);
    }

    private void SimulateDamage()
    {
        if (players.Length > 0)
        {
            Damage fireDamage = new Damage("mc_fire", 120.0);
            players[0].ApplyDamage(fireDamage.Get());
            Debug.Log($"🔥 {players[0].Name} 受到火焰伤害，当前 HP: {players[0].GetHealth()}/{players[0].GetMaxHealth()}");
        }
    }
}
