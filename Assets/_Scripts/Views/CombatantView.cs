using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MineCombat;
using System;
public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private SpriteRenderer avatar;

    // Bind Combatant Data (if needed)
    private Combatant _combatant;

    public void BindCombatant(Combatant combatant)
    {
        _combatant = combatant;

        // Initial display
        SetCombatant(combatant.Name, combatant.CurHP, combatant.MaxHP, avatar != null ? avatar.sprite : null);

        // Subscribe to events
        EventManager.Bind("CombatantDied", new System.Action<Combatant>(c =>
        {
            if (c == _combatant)
            {
                SetSelected(false);
                ShowWrapper(false);
                Console.WriteLine($"{c.Name} has died.");
            }
        }));

        // Subscribe to damage event 
        EventManager.Bind("DamageProcess", new Action<Damage>(dmg =>
        {
            // Example: If this combatant is the target, update health display
            // This requires dmg to have a target reference, which is not implemented here
            // if (dmg.Target == _combatant) { UpdateHealthDisplay(); }
        }));
    } 

    // 设置战斗者信息
    public void SetCombatant(string name, int health, int maxHealth, Sprite avatarSprite)
    {
        if (nameText != null) nameText.text = name;
        if (healthText != null) healthText.text = $"{health}/{maxHealth}";
        if (avatar != null) avatar.sprite = avatarSprite;

        if (healthBar != null)
        {
            float healthRatio = maxHealth > 0 ? (float)health / maxHealth : 0f;
            healthBar.transform.localScale = new Vector3(healthRatio, 1f, 1f);
        }
    }

    // 高亮显示
    public void SetSelected(bool selected)
    {
        float alpha = selected ? 1f : 0.5f;
        Vector3 scale = selected ? Vector3.one * 1.2f : Vector3.one;

        if (avatar != null) avatar.color = new Color(1f, 1f, 1f, alpha);
        if (healthBar != null) healthBar.color = new Color(1f, 1f, 1f, alpha);

        transform.localScale = scale;

        // make sure the combatant is rendered above others when selected
        // Z 坐标偏移实现置顶
        Vector3 pos = transform.localPosition;
        pos.z = selected ? -1f : 0f;  // 负 Z 靠近相机，高于 0 的其他物体
        transform.localPosition = pos;
    }

    // 显示 wrapper（保持默认显示）
    public void ShowWrapper(bool show)
    {
        gameObject.SetActive(show);
    }
}
