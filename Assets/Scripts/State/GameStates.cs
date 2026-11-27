using UnityEngine;

public class MenuState : IGameState
{
    private readonly GameStateMachine sm;
    public MenuState(GameStateMachine sm) => this.sm = sm;

    public void Enter()
    {
        Time.timeScale = 0f;
        sm.SetPanel(sm.MenuPanel, true);
    }
    public void Update() { }
    public void Exit() => sm.SetPanel(sm.MenuPanel, false);
}

public class PlayingState : IGameState
{
    private readonly GameStateMachine sm;
    public PlayingState(GameStateMachine sm) => this.sm = sm;

    public void Enter() => Time.timeScale = 1f;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            sm.ChangeState(sm.PausedState);
        }
    }

    public void Exit() { }
}

public class PausedState : IGameState
{
    private readonly GameStateMachine sm;
    public PausedState(GameStateMachine sm) => this.sm = sm;

    public void Enter()
    {
        Time.timeScale = 0f;
        sm.SetPanel(sm.PausePanel, true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            sm.ChangeState(sm.PlayingState);
        }
    }

    public void Exit() => sm.SetPanel(sm.PausePanel, false);
}

public class LevelUpState : IGameState
{
    private readonly GameStateMachine sm;
    private int pendingLevel;

    public LevelUpState(GameStateMachine sm) => this.sm = sm;

    public void SetLevel(int level) => pendingLevel = level;

    public void Enter()
    {
        Time.timeScale = 0f;
        sm.SetPanel(sm.LevelUpPanel, true);
        sm.UpgradeManager.ShowUpgrades(pendingLevel, () => sm.ChangeState(sm.PlayingState));
    }

    public void Update() { }
    public void Exit() => sm.SetPanel(sm.LevelUpPanel, false);
}

public class VictoryState : IGameState
{
    private readonly GameStateMachine sm;
    public VictoryState(GameStateMachine sm) => this.sm = sm;

    public void Enter()
    {
        Time.timeScale = 0f;
        sm.SetPanel(sm.VictoryPanel, true);
    }

    public void Update() { }
    public void Exit() => sm.SetPanel(sm.VictoryPanel, false);
}

public class GameOverState : IGameState
{
    private readonly GameStateMachine sm;
    public GameOverState(GameStateMachine sm) => this.sm = sm;

    public void Enter()
    {
        Time.timeScale = 0f;
        sm.UpdateGameOverMessage();
        sm.SetPanel(sm.GameOverPanel, true);
    }

    public void Update() { }
    public void Exit() => sm.SetPanel(sm.GameOverPanel, false);
}