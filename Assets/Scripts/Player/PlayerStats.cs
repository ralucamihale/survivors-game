using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO baseStats;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public float MoveSpeed { get; private set; }
    public PlayerStatsSO BaseStats => baseStats;

    private void Awake()
    {
        MaxHealth = baseStats.maxHealth;
        CurrentHealth = baseStats.maxHealth;
        MoveSpeed = baseStats.moveSpeed;
    }

    private void Start()
    {
        // Trimite starea initiala catre UI dupa ce toti listenerii s-au abonat in OnEnable
        EventBus.Instance.Broadcast(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
    }

    public void ResetStats()
    {
        MaxHealth = baseStats.maxHealth;
        CurrentHealth = baseStats.maxHealth;
        MoveSpeed = baseStats.moveSpeed;
        EventBus.Instance.Broadcast(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        EventBus.Instance.Broadcast(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));

        if (CurrentHealth <= 0f)
        {
            EventBus.Instance.Broadcast(new PlayerDiedEvent());
        }
    }

    public void IncreaseSpeed(float multiplier) => MoveSpeed *= multiplier;

    public void IncreaseMaxHealth(float amount)
    {
        MaxHealth += amount;
        CurrentHealth += amount;
        EventBus.Instance.Broadcast(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnLevelUp(PlayerLevelUpEvent e)
    {
        if (e.NewLevel > 1)
        {
            CurrentHealth = MaxHealth;
            EventBus.Instance.Broadcast(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
        }
    }

    public void HealPercent(float percent)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + MaxHealth * percent);
        EventBus.Instance.Broadcast(new PlayerHealthChangedEvent(CurrentHealth, MaxHealth));
    }
}