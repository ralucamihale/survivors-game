using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnEnable()
    {
        if (enemy != null)
            enemy.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (enemy != null)
            enemy.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}