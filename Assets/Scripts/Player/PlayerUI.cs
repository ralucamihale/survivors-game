using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("XP & Level UI")]
    [SerializeField] private Image xpFillImage;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PlayerHealthChangedEvent>(OnHealthChanged);
        EventBus.Instance.Subscribe<PlayerXPChangedEvent>(OnXPChanged);
        EventBus.Instance.Subscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PlayerHealthChangedEvent>(OnHealthChanged);
        EventBus.Instance.Unsubscribe<PlayerXPChangedEvent>(OnXPChanged);
        EventBus.Instance.Unsubscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnHealthChanged(PlayerHealthChangedEvent e)
    {
        if (hpFillImage != null)
            hpFillImage.fillAmount = e.MaxHealth > 0f ? e.CurrentHealth / e.MaxHealth : 0f;

        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(e.CurrentHealth)} / {Mathf.CeilToInt(e.MaxHealth)}";
    }

    private void OnXPChanged(PlayerXPChangedEvent e)
    {
        if (xpFillImage != null)
            xpFillImage.fillAmount = e.XpToNextLevel > 0f ? e.CurrentXP / e.XpToNextLevel : 0f;
    }

    private void OnLevelUp(PlayerLevelUpEvent e)
    {
        if (levelText != null)
            levelText.text = $"Level {e.NewLevel}";
    }
}