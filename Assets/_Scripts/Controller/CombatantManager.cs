using UnityEngine;
using MineCombat;

public class CombatManager : MonoBehaviour
{
    [Header("Prefab & Parent")]
    [SerializeField] private CombatantView combatantViewPrefab; // 战斗者视图预制体
    [SerializeField] private Transform viewParent;              // 父物体，用于组织层级

    [Header("战斗者配置")]
    [SerializeField] private int combatantCount = 2;            // 战斗者数量
    [SerializeField] private float spacing = 100f;                // 战斗者间距
    [SerializeField] private Sprite[] combatantSprites;         // 可选角色贴图

    private Combatant[] combatants;

    void Start()
    {
        if (combatantViewPrefab == null || viewParent == null)
        {
            Debug.LogError("请在 CombatManager 中拖入 CombatantView 预制体和父物体！");
            return;
        }

        // 1️⃣ 初始化战斗者数据
        combatants = new Combatant[combatantCount];
        combatants[0] = new Combatant("Hero (Player)", 100.0);
        combatants[1] = new Combatant("Creeper (Enemy)", 75.0);

        // 2️⃣ 居中排列计算
        float totalWidth = (combatantCount - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < combatantCount; i++)
        {
            // 局部位置相对于父物体
            Vector3 localPos = new Vector3(startX + i * spacing, 0f, 0f);

            // 3️⃣ 实例化 CombatantView，挂载到父物体
            CombatantView view = Instantiate(combatantViewPrefab, viewParent);
            view.transform.localPosition = localPos; // 相对父物体定位
            view.transform.localRotation = Quaternion.identity;

            // 绑定数据
            view.BindCombatant(combatants[i]);

            // 可选：设置角色贴图
            if (combatantSprites != null && i < combatantSprites.Length)
            {
                SpriteRenderer sr = view.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.sprite = combatantSprites[i];
            }
        }

        // 4️⃣ 模拟受伤测试
        Invoke(nameof(SimulateDamage), 2f);
    }

    private void SimulateDamage()
    {
        if (combatants.Length > 0)
        {
            Damage fireDamage = new Damage("mc_fire", 30.0f);
            combatants[0].TakeDamage(fireDamage);
            Debug.Log($"🔥 {combatants[0].Name} 受到火焰伤害，当前 HP: {combatants[0].CurHP}/{combatants[0].MaxHP}");
        }
    }
}
