using UnityEngine;
using Events;
using System.Collections.Generic;
using Game;

public class Playing : EventHandler.GameEventBehaviour
{
    private static Playing _instance;

    [SerializeField] private List<Goomba> _goombas = new List<Goomba>();


    #region Properties

    public static Playing Instance => _instance;

    #endregion

    private void OnEnable()
    {
        EventHandler.Main.PushEvent(this);
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

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

        Mario.Instance.Move();

        foreach (Goomba goomba in _goombas)
        {
            
        }
    }
}
