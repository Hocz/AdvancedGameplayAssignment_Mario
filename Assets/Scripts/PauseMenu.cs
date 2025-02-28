using UnityEngine;
using Events;
using Game;

public class PauseMenu : EventHandler.GameEventBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private static PauseMenu _instance;

    GameManager.GameState _lastGameState;

    #region Properties

    public static PauseMenu Instance => _instance;

    #endregion

    private void OnEnable()
    {
        _instance = this;
        _pauseMenu.SetActive(false);
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        if (GameManager.Instance._currentState == GameManager.GameState.GameOver)
        {
            EventHandler.Main.RemoveEvent(this);
        }

        _lastGameState = GameManager.Instance._currentState;
        GameManager.Instance._currentState = GameManager.GameState.Paused;

        _pauseMenu.SetActive(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override bool IsDone()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("UnPause!");

            GameManager.Instance._currentState = _lastGameState;

            _pauseMenu.SetActive(false);
            return true;
        }
        return false;
    }
}
