using UnityEngine;
using Events;
using Game;

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

        GameManager.Instance._currentState = GameManager.GameState.Combat;

        InitializeCombat();

        Mario.Instance._currentAction = Mario.CombatAction.Heal;
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

        if (!_playerTurn)
        {
            // player attacks
            if (Mario.Instance._currentAction == Mario.CombatAction.Attack)
            {
                // block
                _goomba._currentAction = Goomba.CombatAction.Block;
                OnBlock();
            }
            // player blocks
            else if (Mario.Instance._currentAction == Mario.CombatAction.Block)
            {
                if (_goomba.HP == _goomba.MaxHP)
                {
                    // attack
                    _goomba._currentAction = Goomba.CombatAction.Attack;
                    OnAttack();
                }
                else if (_goomba.HP < _goomba.MaxHP && _goomba.HP > 0)
                {
                    // heal
                    _goomba._currentAction = Goomba.CombatAction.Heal;
                    OnHeal();
                }
            }
            // player heals
            else if (Mario.Instance._currentAction == Mario.CombatAction.Heal)
            {
                // attack
                Debug.Log("Goomba Attack!");
                _goomba._currentAction = Goomba.CombatAction.Attack;
                OnAttack();
            }
        }
        
    }

    public void InitializeCombat()
    {
        Mario.Instance.ClearVelocity();
        _goomba.ClearVelocity();

        Mario.Instance.transform.position = _marioSpawnPos.position;
        _goomba.transform.position = _goombaSpawnPos.position;
    }

    public void OnAttack()
    {
        if (!_playerTurn)
        {
            _goomba._attack = true;
            _playerTurn = true;
            return;
        }
    }

    public void OnBlock()
    {
        if (!_playerTurn)
        {
            _goomba._block = true;
            _playerTurn = true;
            return;
        }
    }

    public void OnHeal()
    {
        if (!_playerTurn)
        {
            _goomba._heal = true;
            _playerTurn = true;
            return;
        }
    }

}
