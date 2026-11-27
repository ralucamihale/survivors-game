using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private int victoryLevel = 5;

    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverMessageText;

    public GameObject MenuPanel => menuPanel;
    public GameObject PausePanel => pausePanel;
    public GameObject LevelUpPanel => levelUpPanel;
    public GameObject VictoryPanel => victoryPanel;
    public GameObject GameOverPanel => gameOverPanel;
    public UpgradeManager UpgradeManager => upgradeManager;

    public MenuState MenuState { get; private set; }
    public PlayingState PlayingState { get; private set; }
    public PausedState PausedState { get; private set; }
    public LevelUpState LevelUpState { get; private set; }
    public VictoryState VictoryState { get; private set; }
    public GameOverState GameOverState { get; private set; }

    private IGameState currentState;
    private bool hasReachedVictory;

    private void Awake()
    {
        MenuState = new MenuState(this);
        PlayingState = new PlayingState(this);
        PausedState = new PausedState(this);
        LevelUpState = new LevelUpState(this);
        VictoryState = new VictoryState(this);
        GameOverState = new GameOverState(this);

        SetPanel(menuPanel, false);
        SetPanel(pausePanel, false);
        SetPanel(levelUpPanel, false);
        SetPanel(victoryPanel, false);
        SetPanel(gameOverPanel, false);
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PlayerLevelUpEvent>(OnPlayerLevelUp);
        EventBus.Instance.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PlayerLevelUpEvent>(OnPlayerLevelUp);
        EventBus.Instance.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void Start()
    {
        ChangeState(MenuState);
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    private void OnPlayerLevelUp(PlayerLevelUpEvent e)
    {
        if (e.NewLevel <= 1) return;

        LevelUpState.SetLevel(e.NewLevel);

        if (e.NewLevel == victoryLevel && !hasReachedVictory)
        {
            hasReachedVictory = true;
            ChangeState(VictoryState);
        }
        else
        {
            ChangeState(LevelUpState);
        }
    }

    private void OnPlayerDied(PlayerDiedEvent e)
    {
        ChangeState(GameOverState);
    }

    public void UpdateGameOverMessage()
    {
        if (gameOverMessageText == null || playerLevel == null) return;

        int lvl = playerLevel.CurrentLevel;
        if (lvl < victoryLevel)
            gameOverMessageText.text = $"Oof....you lost at level {lvl}!";
        else
            gameOverMessageText.text = $"Congrats! You lasted until level {lvl}!";
    }

    public void OnStartClicked()        => ChangeState(PlayingState);
    public void OnPauseClicked()        => ChangeState(PausedState);
    public void OnResumeClicked()       => ChangeState(PlayingState);
    public void OnKeepPlayingClicked()  => ChangeState(LevelUpState);
    public void OnRestartGameClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}