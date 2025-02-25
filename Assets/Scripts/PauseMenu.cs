using UnityEngine;
using Events;

public class PauseMenu : EventHandler.GameEventBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        _pauseMenu.SetActive(true);
    }

    public override bool IsDone()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseMenu.SetActive(false);
            return true;
        }
        return false;
    }
}
