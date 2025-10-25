using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MineCombat;
using static MineCombat.EventManager;
using Unity.VisualScripting;

public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer healthBar;
    [SerializeField] private SpriteRenderer avatar;

    // 数据类改为 Player
    public Player player { get; private set; }

    public static List<CombatantView> AllViews = new List<CombatantView>();
    private float _displayedHealthRatio = 1f;  // 当前显示的血量比例（用于平滑动画）

    private void Awake()
    {
        if (!AllViews.Contains(this))
            AllViews.Add(this);
    }

    private void OnDestroy()
    {
        AllViews.Remove(this);
    }

    /// <summary>
    /// 绑定 Player 数据
    /// </summary>
    public void BindPlayer(Player p)
    {
        player = p;

        // 初始化显示
        SetPlayer(player.Name, player.GetHealth(), player.GetMaxHealth(), avatar != null ? avatar.sprite : null);


        // 更新血条
        UpdateHealthDisplay();
        Debug.Log("Binding player view for " + player.Name);
    }

    private void OnHealthChanged(Entity e)
    {
        if (e == player)
            UpdateHealthDisplay();
    }

    private void OnPlayerDied(Entity e)
    {
        if (e == player)
        {
            SetSelected(false);
            ShowWrapper(false);
        }
    }

    public void UpdateHealthDisplay()
    {
        if (player == null) return;

        double health = player.GetHealth();
        double maxHealth = player.GetMaxHealth();

        if (healthText != null)
            healthText.text = $"{health}/{maxHealth}";

        if (healthBar != null)
        {
            double ratio = maxHealth > 0 ? health / maxHealth : 0;
            StopAllCoroutines();
            StartCoroutine(SmoothHealthBar((float)ratio));
        }
    }

    private IEnumerator SmoothHealthBar(float targetRatio)
    {
        float start = _displayedHealthRatio;
        float elapsed = 0f;
        float duration = 0.3f;

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

    public void SetPlayer(string name, double health, double maxHealth, Sprite avatarSprite)
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
