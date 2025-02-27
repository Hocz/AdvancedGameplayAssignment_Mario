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

        _isDone = false;
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
                _isDone = true;
            }
            
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
