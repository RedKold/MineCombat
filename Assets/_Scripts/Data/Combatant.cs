using System;
namespace MineCombat
{
    /**
    * 战斗者数据类
    * 包含战斗者的属性、状态、技能等信息
    * 负责处理战斗者的逻辑，如受到伤害、使用技能等
    */
    public class Combatant
    {
        public string Name { get; private set; }
        public int CurHP { get; private set; }
        public int MaxHP { get; private set; }

        public Combatant(string name, int maxHP)
        {
            Name = name;
            MaxHP = maxHP;
            CurHP = maxHP;
        }
        // 受到伤害
        public void TakeDamage(Damage dmg)
        {
            double finalDamage = dmg.Get();
            CurHP -= (int)finalDamage;
            if (CurHP < 0) CurHP = 0;

            // Trigger Death Event
            if (isDead)
            {
                EventManager.Trigger("CombatantDied", this);
            }
        } 

        public bool isDead => CurHP <= 1;
    }
}