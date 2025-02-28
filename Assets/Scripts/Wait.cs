using UnityEngine;
using Events;

public class Wait : EventHandler.GameEvent
{
    private float _waitTime;
    private float _waitDuration = 4;

    private bool _isDone = false;

    public Wait(float duration)
    {
        _waitTime = duration;
    }

    public Wait()
    {

    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        _isDone = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // pause
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            EventHandler.Main.PushEvent(PauseMenu.Instance);
        }

        if (GameManager.Instance._currentState == GameManager.GameState.Combat)
        {
            if (!Combat.Instance._playerTurn)
            {
                if (_waitTime < _waitDuration - 1)
                {
                    _waitTime += Time.deltaTime;
                    Combat.Instance._goombaActionChoiceText.text =
                        "Thinking...";
                }
                else
                {
                    _waitTime += Time.deltaTime;

                    Combat.Instance._goombaActionChoiceText.text =
                        $"{Combat.Instance._goomba._currentAction}";

                    if (_waitTime >= _waitDuration)
                    {
                        _isDone = true;
                    }
                }
            }
        }
        else
        {
            if (_waitTime < _waitDuration)
            {
                _waitTime += Time.deltaTime;
            }
            else
            {
                _isDone = true;
            }
        }
    }
    public override bool IsDone()
    {
        if (_isDone)
        {
            return true;
        }
        return false;
    }
}
