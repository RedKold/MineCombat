using MineCombat;
using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    private class Creeper : Entity
    {
        public Creeper(ITags tags) : base(20.0, tags) { }

        public override void Die()
        {
            Debug.Log("Creeper is dead!");
        }
    }

    public void Start()
    {
        Damage damage;
        Creeper creeper = new Creeper((Tags)"mc_resistance");
        creeper.Update("mc_resistance_level", 3);

        Card card = new Card(1, false, Rarity.Epic, StaticTags.Empty, card =>
        {
            double dmg = card.GetDouble("mc_attack_damage") ?? 0.0;
            if (dmg > 0)
            {
                card.Store("mc_output", new Damage(creeper, "mc_common", dmg));
                card.Store("mc_read_type", "mc_damage");
            }
        });
        card.Store("mc_attack_damage", 65.0);

        EventManager.Bind("DamageProcess", new Action<Damage>(dmg =>
        {
            int rst_lvl = dmg.target.GetInt("mc_resistance_level") ?? 0;
            dmg.AddModifier("modifier_mc_resistance", DamageModifiers.CreateCustom, (ref double d) =>
            {
                d *= (rst_lvl * 0.2);
            }, 5, "mc_bypass_resistance");
        }));

        card.action(card);
        string? type = card.GetString("mc_read_type");
        if (type is not null)
        {
            switch (type)
            {
                case "mc_damage":
                    Damage rldmg = card.Get<Damage>("mc_output");
                    if (rldmg is not null)
                    {
                        Debug.Log($"Creeper surfers {rldmg.Get()} damages.");
                    }
                    break;
            }
        }
    }
}