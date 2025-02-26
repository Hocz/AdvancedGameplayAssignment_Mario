using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public enum GameState
    {
        Playing,
        Combat,
        Paused
    }

    public GameState _currentState;


    #region Properties

    public static GameManager Instance => _instance;

    #endregion

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

}
