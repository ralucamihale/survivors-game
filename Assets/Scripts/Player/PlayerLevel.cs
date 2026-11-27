using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO baseStats;
    [SerializeField] private float xpGrowthMultiplier = 1.35f;

    public int CurrentLevel { get; private set; } = 1;
    public float CurrentXP { get; private set; } = 0f;
    public float XpToNextLevel { get; private set; } = 10f;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void Start()
    {
        ResetLevel();
    }

    public void ResetLevel()
    {
        CurrentLevel = 1;
        CurrentXP = 0f;
        XpToNextLevel = baseStats != null ? baseStats.baseXpToNextLevel : 10f;

        EventBus.Instance.Broadcast(new PlayerXPChangedEvent(CurrentXP, XpToNextLevel));
        EventBus.Instance.Broadcast(new PlayerLevelUpEvent(CurrentLevel));
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        AddXP(e.XpReward);
    }

    public void AddXP(float amount)
    {
        CurrentXP += amount;

        while (CurrentXP >= XpToNextLevel)
        {
            CurrentXP -= XpToNextLevel;
            CurrentLevel++;
            XpToNextLevel = Mathf.Round(XpToNextLevel * xpGrowthMultiplier);

            EventBus.Instance.Broadcast(new PlayerLevelUpEvent(CurrentLevel));
        }

        EventBus.Instance.Broadcast(new PlayerXPChangedEvent(CurrentXP, XpToNextLevel));
    }

    private void Update()
    {
        if (Time.timeScale > 0f && Input.GetKeyDown(KeyCode.L))
        {
            AddXP(XpToNextLevel - CurrentXP);
        }
    }
}