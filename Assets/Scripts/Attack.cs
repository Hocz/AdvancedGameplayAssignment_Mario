using Events;
using Game;
using UnityEngine;

public class Attack : EventHandler.GameEvent
{
    private float _arcTime = 0;
    private float _arcDuration = 2;
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

        if (!Combat.Instance._playerTurn)
        {
            // jump in arc
            if (_arcTime < _arcDuration)
            {
                _arcTime += Time.deltaTime;

                float t = _arcTime / _arcDuration;

                Vector3 pos = Vector3.Lerp(
                    Combat.Instance.GoombaSpawnPos.position,
                    Mario.Instance.AttackTarget.position,
                    t);

                float arc = Mathf.Sin(t * Mathf.PI) * 2;

                Combat.Instance._goomba.transform.position = new Vector3(pos.x, pos.y + arc, pos.z);
            }
            else
            {
                if (Random.value <= Mario.Instance._blockChance && Mario.Instance._isBlocking)
                {
                    // successfully blocked attack
                    Debug.Log("Mario blocked attack!");
                }
                else
                {
                    Debug.Log("Mario Failed block!");

                    Mario.Instance.TakeDamage();
                }
                _isDone = true;
            }
        }

        if (Combat.Instance._playerTurn)
        {
            // jump in arc
            if (_arcTime < _arcDuration)
            {
                _arcTime += Time.deltaTime;

                float t = _arcTime / _arcDuration;

                Vector3 pos = Vector3.Lerp(
                    Combat.Instance.MarioSpawnPos.position,
                    Combat.Instance._goomba.AttackTarget.position,
                    t);

                float arc = Mathf.Sin(t * Mathf.PI) * 2;

                Mario.Instance.transform.position = new Vector3(pos.x, pos.y + arc, pos.z);
            }
            else
            {
                if (Random.value <= Combat.Instance._goomba._blockChance && Combat.Instance._goomba._isBlocking)
                {
                    // successfully blocked attack
                    Debug.Log("Goomba blocked attack!");
                }
                else
                {
                    Debug.Log("Goomba Failed block!");

                    Combat.Instance._goomba.TakeDamage();
                }
                _isDone = true;
            }
            
        }
    }


    public override bool IsDone()
    {
        if (_isDone)
        {
            if (Combat.Instance._goomba.HP > 0 && Mario.Instance.HP > 0)
            {
                Combat.Instance._playerTurn = !Combat.Instance._playerTurn;
                return true;
            }
            else
            {
                return true;
            }
            
        }
        return false;
    }

}
