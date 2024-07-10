using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PlayerEntity.cs
 * Desc     : BaseGameEntity 상속
 *            Entity의 한 종류 Player
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class PlayerEntity : BaseGameEntity
{
    private readonly float _exhaustedTime = 3f;
    private readonly float _valueAfterExhaustion = 1f;

    private float _staminaTimer;
    private float _exhaustedTimer;

    public PlayerParts[] Weapons;
    public SphereCollider[] HandsCollider;
    public AnimationClip DieAnimation;

    private int _weaponIndex;

    [HideInInspector]
    public int AttackCount = 0;

    public static StateOfPlay<PlayerEntity>[] States { get; private set; }
    public static StateMachine<PlayerEntity> StateMachine { get; private set; }

    public bool IsHausted { get; private set; }

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        #region Stamina

        if (InputManager.Instance.IsRun && InputManager.Instance.MoveVector != Vector2.zero)
        {
            DataManager.Instance.PlayerStatus.CurrentStamina -= Time.deltaTime;
        }

        if (DataManager.Instance.PlayerStatus.CurrentStamina == DataManager.Instance.PlayerStatus.MaxStaminaValue)
        {
            UIManager.Instance.PlayerInfoPanel.StaminaSlider.gameObject.SetActive(false);
        }
        else
        {
            UIManager.Instance.PlayerInfoPanel.StaminaSlider.gameObject.SetActive(true);
        }

        if (DataManager.Instance.PlayerStatus.CurrentStamina <= 0f)
        {
            _exhaustedTimer += Time.deltaTime;
            IsHausted = true;
        }

        if (_exhaustedTimer >= _exhaustedTime)
        {
            DataManager.Instance.PlayerStatus.CurrentStamina += _valueAfterExhaustion;
            IsHausted = false;
            _exhaustedTimer = 0f;
        }

        if (!IsHausted && !InputManager.Instance.IsRun)
        {
            _staminaTimer += Time.deltaTime;

            if (_staminaTimer > DataManager.Instance.PlayerStatus.StaminaRecoveryDelayTime)
            {
                DataManager.Instance.PlayerStatus.CurrentStamina += DataManager.Instance.PlayerStatus.StaminaRecoveryValue;
                _staminaTimer = 0f;
            }
        }
        #endregion

        Updated();
    }

    public override void Setup()
    {
        base.Setup();

        States = new StateOfPlay<PlayerEntity>[6];
        StateMachine = new StateMachine<PlayerEntity>();

        States[(int)EnumTypes.PlayerState.Idle] = new PlayerOwnedStates.Idle();
        States[(int)EnumTypes.PlayerState.Talk] = new PlayerOwnedStates.Talk();
        States[(int)EnumTypes.PlayerState.Walk] = new PlayerOwnedStates.Walk();
        States[(int)EnumTypes.PlayerState.Run] = new PlayerOwnedStates.Run();
        States[(int)EnumTypes.PlayerState.Attack] = new PlayerOwnedStates.Attack();
        States[(int)EnumTypes.PlayerState.Die] = new PlayerOwnedStates.Die();

        StateMachine.Setup(this, States[(int)EnumTypes.PlayerState.Idle]);
    }

    public override void Updated()
    {
        StateMachine.Execute();
    }

    public static void ChangeState(EnumTypes.PlayerState newState)
    {
        StateMachine.ChangeState(States[(int)newState]);
    }

    // 공격 콜라이더 
    public void EnableAttackCollider()
    {
        if (DataManager.Instance.Equipment.WeaponSlot.IsEquipped) // 무기 장착
        {
            for (int i = 0; i < Weapons.Length; ++i)
            {
                if (DataManager.Instance.Equipment.WeaponSlot.EquipmentItem.Id == Weapons[i].PartsId)
                {
                    _weaponIndex = i;
                    Weapons[i].GetComponent<BoxCollider>().enabled = true;
                }
            }
        }
        else
        {
            if (AttackCount == 2)
            {
                HandsCollider[(int)EnumTypes.PlayerAttackDirection.Left].enabled = true;
            }
            else
            {
                HandsCollider[(int)EnumTypes.PlayerAttackDirection.Right].enabled = true;
            }
        }
    }

    
    public void DisableAttackCollider()
    {
        Weapons[_weaponIndex].GetComponent<BoxCollider>().enabled = false;

        if (AttackCount == 2)
        {
            HandsCollider[(int)EnumTypes.PlayerAttackDirection.Left].enabled = false;
        }
        else
        {
            HandsCollider[(int)EnumTypes.PlayerAttackDirection.Right].enabled = false;
        }
    }
}
