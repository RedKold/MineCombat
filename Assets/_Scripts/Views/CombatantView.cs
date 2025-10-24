using System;
using System.Collections;
using UnityEngine;
using TMPro;
using MineCombat;
using static MineCombat.EventManager;
using System.Collections.Generic;


public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private SpriteRenderer avatar;

    public static List<CombatantView> AllViews = new List<CombatantView>();


    // Maintain the List
     private void Awake()
    {
        // 添加到全局列表
        if (!AllViews.Contains(this))
            AllViews.Add(this);
    }

    private void OnDestroy()
    {
        // 移除（防止引用已销毁对象）
        AllViews.Remove(this);
    }
    public Combatant _combatant{ get; private set; }
    private float _displayedHealthRatio = 1f;  // 当前显示的血量比例（用于平滑动画）

    public void BindCombatant(Combatant combatant)
    {
        _combatant = combatant;

        // 初始显示
        SetCombatant(combatant.Name, combatant.CurHP, combatant.MaxHP, avatar != null ? avatar.sprite : null);

        
        // Update initial health display
        Debug.Log("Binding combatant view for " + combatant.Name);
        UpdateHealthDisplay(); // 更新血条和文字
    }

    private void OnHealthChanged(Combatant c)
    {
        if (c == _combatant)
        {
            UpdateHealthDisplay(); // 刷新血条
        }
    }

    private void OnCombatantDied(Combatant c)
    {
        if (c == _combatant)
        {
            SetSelected(false);
            ShowWrapper(false);
        }
    }
    // 更新血条与血量文字
    public void UpdateHealthDisplay()
    {
        if (_combatant == null) return;

        double health = _combatant.CurHP;
        double maxHealth = _combatant.MaxHP;

        if (healthText != null)
            healthText.text = $"{health}/{maxHealth}";

        if (healthBar != null)
        {
            double ratio = maxHealth > 0 ? health / maxHealth : 0;
            StopAllCoroutines();
            StartCoroutine(SmoothHealthBar((float)ratio));
        }
    }

    // 平滑动画更新血条宽度
    private IEnumerator SmoothHealthBar(float targetRatio)
    {
        float start = _displayedHealthRatio;
        float elapsed = 0f;
        float duration = 0.3f; // 动画时间（秒）

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _displayedHealthRatio = Mathf.Lerp(start, targetRatio, t);
            healthBar.transform.localScale = new Vector3(_displayedHealthRatio, 1f, 1f);
            yield return null;
        }

        _displayedHealthRatio = targetRatio;
        healthBar.transform.localScale = new Vector3(_displayedHealthRatio, 1f, 1f);
    }

    // 初始化战斗者信息
    public void SetCombatant(string name, double health, double maxHealth, Sprite avatarSprite)
    {
        if (nameText != null) nameText.text = name;
        if (healthText != null) healthText.text = $"{health}/{maxHealth}";
        if (avatar != null) avatar.sprite = avatarSprite;

        if (healthBar != null)
        {
            double ratio = maxHealth > 0 ? health / maxHealth : 0;
            _displayedHealthRatio = (float)ratio;
            healthBar.transform.localScale = new Vector3(_displayedHealthRatio, 1f, 1f);
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

        Vector3 pos = transform.localPosition;
        pos.z = selected ? -1f : 0f;
        transform.localPosition = pos;
    }


    
    public void ShowWrapper(bool show)
    {
        gameObject.SetActive(show);
    }
}
