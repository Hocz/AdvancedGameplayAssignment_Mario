using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : EventHandler.GameEventBehaviour
{
    private void OnEnable()
    {
        EventHandler.Main.PushEvent(this);
    }

    public override bool IsDone()
    {
        return false;
    }

    public void OnStartGame()
    {
        EventHandler.Main.RemoveEvent(this);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void OnQuit()
    {
        EventHandler.Main.RemoveEvent(this);
        Application.Quit();
    }
}
