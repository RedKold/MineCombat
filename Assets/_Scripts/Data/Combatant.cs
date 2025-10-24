using System;
using System.Diagnostics;
using static MineCombat.EventManager;
using UnityEngine;

namespace MineCombat
{
    /**
    * 战斗者数据类
    * 包含战斗者的属性、状态、技能等信息
    * 负责处理战斗者的逻辑，如受到伤害、使用技能等
    */
    // 1. 继承 Entity 类
    public class Combatant : Entity
    {
        public string Name { get; private set; }

        // 2. 移除冗余的 CurHP 和 MaxHP 属性，并提供兼容的只读属性
        public double CurHP => GetHealth();
        public double MaxHP => GetMaxHealth();

        // 3. 构造函数：必须调用基类 (Entity) 的构造函数
        public Combatant(string name, double maxHP) : base(maxHP)
        {
            Name = name;
        }

        // 受到伤害
        public void TakeDamage(Damage dmg)
        {
            // 获取最终伤害值 (double)
            double finalDamage = dmg.Get();

            // 4. 使用基类 Entity 提供的 ApplyDamage 方法
            // Entity 内部处理了生命值扣减、最小为0和死亡状态更新（_alive = false）的逻辑
            ApplyDamage(finalDamage);

            // Trigger Death Event
            // 5. 使用基类 Entity 提供的 IsAlive() 或 CurHP 属性来判断
            if (CurHP <=1e-9 || !IsAlive()) // 或 if (!IsAlive())
            {
                // 假设 EventManager 可用
                UnityEngine.Debug.Log("Combatant " + Name + " has died.");
                Events.Trigger("CombatantDied",this);
            }

            // Trigger Health Changed Event
            Events.Trigger("HealthChanged", this);
        } 

        // 6. isDead 属性现在可以基于基类的生命值状态来定义
        // 或者使用更精确的死亡判断：
        public bool isDead => !IsAlive(); 
    }
}