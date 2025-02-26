using UnityEngine;
using Events;
using Game;

public class PauseMenu : EventHandler.GameEventBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private static PauseMenu _instance;


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
        GameManager.Instance._currentState = GameManager.GameState.Paused;

        _pauseMenu.SetActive(true);
    }

    public override bool IsDone()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance._currentState = GameManager.GameState.Playing;

            _pauseMenu.SetActive(false);
            return true;
        }
        return false;
    }
}
