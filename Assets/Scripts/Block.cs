using Events;
using Game;
using UnityEngine;
public class Block : EventHandler.GameEvent
{
    private bool _isDone = false;
    

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        if (bFirstTime && !Combat.Instance._playerTurn)
        {
            EventHandler.Main.PushEvent(new Wait());
        }

        _isDone = false;

        if (GameManager.Instance._currentState == GameManager.GameState.GameOver)
        {
            _isDone = true;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // pause
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            EventHandler.Main.PushEvent(PauseMenu.Instance);
        }

        if (Combat.Instance._playerTurn)
        {
            Mario.Instance._isBlocking = true;
            Mario.Instance._blockChance = Random.Range(0.5f, 1.0f);
        }

        if (!Combat.Instance._playerTurn)
        {
            Combat.Instance._goomba._isBlocking = true;
            Combat.Instance._goomba._blockChance = Random.Range(0.5f, 1.0f);
        }

        _isDone = true;
    }

    public override bool IsDone()
    {
        if(_isDone)
        {
            Combat.Instance._playerTurn = !Combat.Instance._playerTurn;
            return true;
        }
        return false;
    }
}
