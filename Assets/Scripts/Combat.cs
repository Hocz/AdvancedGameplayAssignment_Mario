using Events;
using Game;
using System.Collections;
using TMPro;
using UnityEngine;
using static Game.GameBody;

public class Combat : EventHandler.GameEventBehaviour
{
    private static Combat _instance;

    public Goomba _goomba;

    public bool _playerTurn;

    [SerializeField] private Transform _cameraPos;
    [SerializeField] private Transform _marioSpawnPos;
    [SerializeField] private Transform _goombaSpawnPos;
    [SerializeField] private Transform _attackPoint;

    [SerializeField] private GameObject _combatActions;

    [Header("Combat Info")]

    [SerializeField] private GameObject _combatInfo;

    [SerializeField] private TextMeshProUGUI _marioHealthText;
    [SerializeField] private TextMeshProUGUI _marioActionText;

    [SerializeField] private TextMeshProUGUI _goombaHealthText;
    [SerializeField] private TextMeshProUGUI _goombaActionText;


    [SerializeField] private TextMeshProUGUI _currentTurnText;
    [SerializeField] public TextMeshProUGUI _goombaActionChoiceText;

    #region Properties

    public static Combat Instance => _instance;


    public Transform CameraPos => _cameraPos;
    public Transform MarioSpawnPos => _marioSpawnPos;
    public Transform GoombaSpawnPos => _goombaSpawnPos;
    public Transform AttackPoint => _attackPoint;

    #endregion

    private void OnEnable()
    {
        _instance = this;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);

        if (GameManager.Instance._currentState == GameManager.GameState.GameOver 
            || GameManager.Instance._isGameOver)
        {
            EventHandler.Main.RemoveEvent(this);
        }

        InitializeCombat();

        _combatInfo.SetActive(true);
        _goombaActionChoiceText.gameObject.SetActive(false);

        UpdateCombatInfo();

        if (_playerTurn)
        {
            _combatActions.SetActive(true);
        }

        if (_goomba._currentAction != GameBody.CombatAction.Block)
        {
            _goomba._isBlocking = false;
        }
        if (Mario.Instance._currentAction != GameBody.CombatAction.Block)
        {
            Mario.Instance._isBlocking = false;
        }
    }

    public override bool IsDone()
    {
        if (_goomba.HP <= 0)
        {
            Mario.Instance.transform.position = Playing.Instance._marioLastPosition;
            return true;
        }
        else if (Mario.Instance.HP <= 0 
            || GameManager.Instance._currentState == GameManager.GameState.GameOver
            || GameManager.Instance._isGameOver)
        {
            return true;
        }
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


        if (!_playerTurn && GameManager.Instance._currentState == GameManager.GameState.Combat)
        {
            _goombaActionChoiceText.gameObject.SetActive(true);

            // no action selected
            if (Mario.Instance._currentAction == GameBody.CombatAction.None)
            {
                // attack
                _goomba._currentAction = GameBody.CombatAction.Attack;
                OnAttack();
            }

            // player attacks
            if (Mario.Instance._currentAction == GameBody.CombatAction.Attack)
            {
                // 75 / 25 chance to either attack or choose between heal, block or attack
                if (Random.value > 0.75f)
                {
                    // can heal? heal
                    if (_goomba.HP < _goomba.MaxHP && _goomba.HP > 0)
                    {
                        _goomba._currentAction = GameBody.CombatAction.Block;
                        OnBlock();
                    }
                    // 50 chance block
                    else if (Random.value > 0.5f)
                    {
                        // heal
                        _goomba._currentAction = GameBody.CombatAction.Heal;
                        OnHeal();
                    }
                    // else attack anyway
                    else
                    {
                        // attack
                        _goomba._currentAction = GameBody.CombatAction.Attack;
                        OnAttack();
                    }
                    
                }
                else
                {
                    // attack
                    _goomba._currentAction = GameBody.CombatAction.Attack;
                    OnAttack();
                }
                
            }
            // player blocks
            else if (Mario.Instance._currentAction == GameBody.CombatAction.Block)
            {
                // full hp -> attack
                if (_goomba.HP == _goomba.MaxHP)
                {
                    // attack
                    _goomba._currentAction = GameBody.CombatAction.Attack;
                    OnAttack();
                }
                // can heal? 50 / 50 heal or attack
                else if (_goomba.HP < _goomba.MaxHP && _goomba.HP > 0)
                {
                    if (Random.value > 0.5f)
                    {
                        // heal
                        _goomba._currentAction = GameBody.CombatAction.Heal;
                        OnHeal();
                    }
                    else
                    {
                        // attack
                        _goomba._currentAction = GameBody.CombatAction.Attack;
                        OnAttack();
                    }
                    
                }
            }
            // player heals
            else if (Mario.Instance._currentAction == GameBody.CombatAction.Heal)
            {
                // 50 / 50 chance block or attack
                if (Random.value > 0.5f)
                {
                    // attack
                    _goomba._currentAction = GameBody.CombatAction.Attack;
                    OnAttack();
                }
                else
                {
                    _goomba._currentAction = GameBody.CombatAction.Block;
                    OnBlock();
                }
                
            }
        }
        
    }

    public void InitializeCombat()
    {
        Mario.Instance.ClearVelocity();
        _goomba.ClearVelocity();

        Mario.Instance.transform.position = _marioSpawnPos.position;
        Mario.Instance.transform.rotation = Quaternion.identity;
        _goomba.transform.position = _goombaSpawnPos.position;
    }

    public void UpdateCombatInfo()
    {
        _marioHealthText.text = $"Health: {Mario.Instance.HP}";
        _goombaHealthText.text = $"Health: {_goomba.HP}";

        _marioActionText.text = $"Action: {Mario.Instance._currentAction}";
        _goombaActionText.text = $"Action: {_goomba._currentAction}";


        if (_playerTurn) _currentTurnText.text = "Player Turn";
        else _currentTurnText.text = "Goomba Turn";
    }

    public void OnAttack()
    {
        if (_playerTurn)
        {
            Mario.Instance._currentAction = GameBody.CombatAction.Attack;
        }
        _combatActions.SetActive(false);
        
        UpdateCombatInfo();

        EventHandler.Main.PushEvent(new Attack());
    }

    public void OnBlock()
    {
        if (_playerTurn)
        {
            Mario.Instance._currentAction = GameBody.CombatAction.Block;
        }
        _combatActions.SetActive(false);

        UpdateCombatInfo();

        EventHandler.Main.PushEvent(new Block());
    }

    public void OnHeal()
    {
        if (_playerTurn)
        {
            Mario.Instance._currentAction = GameBody.CombatAction.Heal;
        }
        _combatActions.SetActive(false);

        UpdateCombatInfo();

        EventHandler.Main.PushEvent(new Heal());
    }

}
