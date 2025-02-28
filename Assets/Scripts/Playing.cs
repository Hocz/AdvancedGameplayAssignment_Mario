using UnityEngine;
using Events;
using System.Collections.Generic;
using Game;
using UnityEngine.SceneManagement;
using System.Collections;

public class Playing : EventHandler.GameEventBehaviour
{
    private static Playing _instance;

    public Vector3 _marioLastPosition;
    public Vector3 _goombaLastPosition;
    public Vector3 _cameraLastPosition;

    #region Properties

    public static Playing Instance => _instance;

    #endregion

    private void OnEnable()
    {
        EventHandler.Main.PushEvent(this);

        _instance = this;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        if (GameManager.Instance._currentState == GameManager.GameState.GameOver)
        {
            StartCoroutine(GameOver());
        }

        GameManager.Instance._currentState = GameManager.GameState.Playing;
    }

    public override bool IsDone()
    {
        return false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // pause
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            EventHandler.Main.PushEvent(PauseMenu.Instance);
        }

        if (GameManager.Instance._currentState == GameManager.GameState.GameWon)
        {
            StartCoroutine(GameWon());
        }
    }

    IEnumerator GameOver()
    {
        GameManager.Instance._gameOverScreen.SetActive(true);
        EventHandler.Main.PushEvent(new Wait(2));

        yield return new WaitForSeconds(3);

        EventHandler.Main.RemoveEvent(this);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void SetGameWon()
    {
        StartCoroutine(GameWon());
    }

    IEnumerator GameWon()
    {
        GameManager.Instance._isGameOver = true;

        GameManager.Instance._gameWonScreen.SetActive(true);
        EventHandler.Main.PushEvent(new Wait(2));

        yield return new WaitForSeconds(3);

        EventHandler.Main.RemoveEvent(this);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
