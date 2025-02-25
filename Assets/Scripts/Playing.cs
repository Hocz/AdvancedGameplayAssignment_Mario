using UnityEngine;
using Events;
using System.Collections.Generic;
using Game;

public class Playing : EventHandler.GameEventBehaviour
{
    private static Playing _instance;

    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();


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
            PauseMenu pm = FindObjectOfType<PauseMenu>();
            EventHandler.Main.PushEvent(pm);
        }

        foreach (Enemy enemy in _enemies)
        {
            enemy.Move();
        }
    }
}
