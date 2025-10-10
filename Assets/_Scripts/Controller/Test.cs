using MineCombat;
using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void Start()
    {
        Damage damage1 = new("mc_common", 10);
        Damage damage2 = new("mc_magic", 10);

        EventManager.Bind("DamageProcess", new Action<Damage>(dmg =>
        {
            if (!DamageTags.Ignore(dmg.type, "mc_bypass_armor"))
            {
                dmg.value *= 0.5;
            }
            dmg.AddModifier("mc_test", DamageModifiers.CreateMul, 0.5, 10, "mc_bypass_test");
        }));

        Properties properties = new Properties();
        properties.Store("mc_fortune", 3);
        properties.Store("mc_attack_damage", 4.5);
        properties.Update("mc_fortune", 6);
        properties.Update("mc_name", "OftenOviour");
        properties.Update("mc_load", true);
        properties.Store("mc_damage1", damage1);
        properties.Store("mc_damage2", damage2);

        properties.Change("mc_attack_damage", (ref double d) => { d *= 2.5; });
        properties.Change("mc_damage1", (ref Damage dmg) => { 
            dmg.AddModifier("mc_test", DamageModifiers.CreateMul, 0.3, 10, "mc_bypass_test");
            dmg.AddModifier("mc_peek", DamageModifiers.CreateMulTotal, 0.5, 16, "mc_bypass_test");
            dmg.AddModifier("mc_peek", DamageModifiers.CreateMulTotal, 0.5, 16, "mc_bypass_test");
            dmg.AddModifier("mc_mid", DamageModifiers.CreateAdd, 11, 12);
            dmg.AddModifier("mc_custom", DamageModifiers.CreateCustom((ref double d) => { d *= 0.5; }, 14));
        });

        Debug.Log($"mc_fortune:{properties.GetInt("mc_fortune")}");
        Debug.Log($"mc_attack_damage:{properties.GetDouble("mc_attack_damage")}");
        Debug.Log($"mc_name:{properties.GetString("mc_name")}");
        Debug.Log($"mc_load:{properties.GetBool("mc_load")}");
        Debug.Log($"mc_damage1:{properties.Get<Damage>("mc_damage1")?.Get()}");
    }
}