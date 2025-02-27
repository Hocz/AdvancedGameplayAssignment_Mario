using Events;
using Game;
using TMPro;
using UnityEngine;

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
    
    [SerializeField] private GameObject _combatInfo;

    [SerializeField] private TextMeshProUGUI _marioHealthText;
    [SerializeField] private TextMeshProUGUI _marioActionText;

    [SerializeField] private TextMeshProUGUI _goombaHealthText;
    [SerializeField] private TextMeshProUGUI _goombaActionText;


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

        _combatInfo.SetActive(true);

        UpdateCombatInfo();

        if (_playerTurn)
        {
            _combatActions.SetActive(true);
        }
        

        //Mario.Instance._currentAction = Mario.CombatAction.Heal;
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
                // block
                _goomba._currentAction = GameBody.CombatAction.Block;
                OnBlock();
            }
            // player blocks
            else if (Mario.Instance._currentAction == GameBody.CombatAction.Block)
            {
                if (_goomba.HP == _goomba.MaxHP)
                {
                    // attack
                    _goomba._currentAction = GameBody.CombatAction.Attack;
                    OnAttack();
                }
                else if (_goomba.HP < _goomba.MaxHP && _goomba.HP > 0)
                {
                    // heal
                    _goomba._currentAction = GameBody.CombatAction.Heal;
                    OnHeal();
                }
            }
            // player heals
            else if (Mario.Instance._currentAction == GameBody.CombatAction.Heal)
            {
                // attack
                _goomba._currentAction = GameBody.CombatAction.Attack;
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

    public void UpdateCombatInfo()
    {
        _marioHealthText.text = $"Health: {Mario.Instance.HP}";
        _goombaHealthText.text = $"Health: {_goomba.HP}";

        _marioActionText.text = $"Action: {Mario.Instance._currentAction}";
        _goombaActionText.text = $"Action: {_goomba._currentAction}";
    }


    public void OnAttack()
    {
        if (_playerTurn) Mario.Instance._currentAction = GameBody.CombatAction.Attack;

        _combatActions.SetActive(false);
        
        UpdateCombatInfo();

        EventHandler.Main.PushEvent(new Attack());
    }

    public void OnBlock()
    {
        UpdateCombatInfo();
    }

    public void OnHeal()
    {
        UpdateCombatInfo();
    }

}
