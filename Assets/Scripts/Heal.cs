using Events;
using Game;
using UnityEngine;

public class Heal : EventHandler.GameEvent
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

        if (!Combat.Instance._playerTurn)
        {
            Combat.Instance._goomba.Heal();
            _isDone = true;
        }   

        if (Combat.Instance._playerTurn)
        {
            Mario.Instance.Heal();
            _isDone = true;
        }
    }

    public override bool IsDone()
    {
        if (_isDone)
        {
            Combat.Instance._playerTurn = !Combat.Instance._playerTurn;
            return true;
        }
        return false;
    }
}
