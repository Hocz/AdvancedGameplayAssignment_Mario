using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameObject _gameOverScreen;
    public GameObject _gameWonScreen;

    public bool _isGameOver = false;

    public enum GameState
    {
        Playing,
        Combat,
        Paused,
        GameOver,
        GameWon
    }

    public GameState _currentState;


    #region Properties

    public static GameManager Instance => _instance;

    #endregion

    private void OnEnable()
    {
        _instance = this;

        _gameOverScreen.SetActive(false);
        _gameWonScreen.SetActive(false);

        _isGameOver = false;
    }

}
