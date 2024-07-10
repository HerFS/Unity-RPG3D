using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : BaseEntityStatus.cs
 * Desc     : abstract
 *            모든 Entity들의 기본 스탯들
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public abstract class BaseEntityStatus : MonoBehaviour
{
    #region Events
    protected delegate void NameChangedHandler(BaseEntityStatus baseStatus, string newName, string prevName);
    public delegate void HpChangedHandler(BaseEntityStatus baseStatus, float currentHp, float prevHp);
    public delegate void MaxHpChangedHandler(BaseEntityStatus baseStatus, float currentMaxHp, float prevMaxHp);
    protected delegate void AttackDamageHandler(BaseEntityStatus baseStatus, float currentDamage, float prevDamage);
    protected delegate void CriticalDamageChangedHandler(BaseEntityStatus baseStatus, float currentCritical, float prevCritical);
    protected delegate void CriticalChanceHandler(BaseEntityStatus baseStatus, float currentChance, float prevChance);
    protected delegate void DefenseChangedHandler(BaseEntityStatus baseStatus, float currentDefense, float prevDefense);
    protected delegate void LevelChangedHandler(BaseEntityStatus baseStatus, uint currentLevel, uint prevLevel);
    #endregion

    [Header("Base")]
    [SerializeField]
    private InitialStatusSetup _initialStatusSetup;
    [HideInInspector]
    public float InvincibilityTime;

    // 게임 업데이트마다 수정 가능
    private readonly float _minAttackDamageValue = 0f;
    private readonly float _maxAttackDamageValue = 5000000f;
    private readonly float _maxCriticalChance = 100f;
    private readonly float _minDefenseValue = 0f; 
    private readonly float _maxDefenseValue = 100f;
    private readonly int _minLevelValue = 1;
    protected readonly float CriticalDamageMultiplier = 1.5f;
    protected readonly float MaxHpMultiplier = 1.5f;
    protected readonly int MaxLevelValue = 30;

    private string _name;
    private float _walkSpeed;

    private float _currentHp;
    private float _maxHp;

    private float _attackDamage;
    private float _criticalDamage;
    private float _criticalChance;

    private float _defense;

    private uint _level;

    protected event NameChangedHandler onNameChanged;
    public event HpChangedHandler onHpChanged;
    public event MaxHpChangedHandler onMaxHpChanged;
    protected event AttackDamageHandler onAttackDamage;
    protected event CriticalDamageChangedHandler onCriticalDamageChanged;
    protected event CriticalChanceHandler onCriticalChanceChanged;
    protected event DefenseChangedHandler onDefenseChanged;
    protected event LevelChangedHandler onLevelChanged;

    [HideInInspector]
    public bool IsHit;

    public float WalkSpeed
    {
        get { return _walkSpeed; }
        set
        {
            _walkSpeed = Mathf.Clamp(value, 0f, 5f);
        }
    }

    public float MaxHp
    {
        get { return _maxHp; }
        set
        {
            float prevMaxHp = _maxHp;
            _maxHp = value;
            if (prevMaxHp != _maxHp)
            {
                onMaxHpChanged?.Invoke(this, _maxHp, prevMaxHp);
            }
        }
    }

    public string Name
    {
        get { return _name; }
        set
        {
            string prevName = _name;
            _name = value;
            if (_name != prevName)
            {
                onNameChanged?.Invoke(this, _name, prevName);
            }
        }
    }

    public float CurrentHp
    {
        get { return _currentHp; }
        set
        {
            float prevHp = _currentHp;
            _currentHp = Mathf.Clamp(value, 0f, MaxHp);
            if (_currentHp != prevHp)
            {
                onHpChanged?.Invoke(this, _currentHp, prevHp);
            }
        }
    }

    public float AttackDamage
    {
        get { return _attackDamage; }
        set
        {
            float prevDamage = _attackDamage;
            _attackDamage = Mathf.Clamp(value, _minAttackDamageValue, _maxAttackDamageValue);
            if (_attackDamage != prevDamage)
            {
                onAttackDamage?.Invoke(this, _attackDamage, prevDamage);
            }
        }
    }

    public float CriticalDamage
    {
        get { return _criticalDamage; }
        set
        {
            float prevCritical = _criticalDamage;
            _criticalDamage = AttackDamage * CriticalDamageMultiplier;
            if (_criticalDamage != prevCritical)
            {
                onCriticalDamageChanged?.Invoke(this, _criticalDamage, prevCritical);
            }
        }
    }

    public float CriticalChance
    {
        get { return _criticalChance; }
        set
        {
            float prevChance = _criticalChance;
            _criticalChance = Mathf.Clamp(value, 0f, _maxCriticalChance);
            if (_criticalChance != prevChance)
            {
                onCriticalChanceChanged?.Invoke(this, _criticalChance, prevChance);
            }
        }
    }

    public float Defense
    {
        get { return _defense; }
        set
        {
            float prevDefense = _defense;
            _defense = Mathf.Clamp(value, _minDefenseValue, _maxDefenseValue);
            if (_defense != prevDefense)
            {
                onDefenseChanged?.Invoke(this, _defense, prevDefense);
            }
        }
    }

    public uint Level
    {
        get { return _level; }
        set
        {
            uint prevLevel = _level;
            _level = (uint)Mathf.Clamp(value, _minLevelValue, MaxLevelValue);
            if (_level != prevLevel)
            {
                onLevelChanged?.Invoke(this, _level, prevLevel);
            }
        }
    }

    protected virtual void Awake()
    {
        #region StatusSetup
        AttackDamage   = _initialStatusSetup.AttackDamage;
        CriticalDamage = AttackDamage * CriticalDamageMultiplier;
        CriticalChance = _initialStatusSetup.CriticalChance;
        WalkSpeed      = _initialStatusSetup.WalkSpeed;
        Defense        = _initialStatusSetup.Defense;
        Level          = _initialStatusSetup.Level;
        Name           = _initialStatusSetup.Name;
        MaxHp          = CalculationMaxHp();
        CurrentHp      = MaxHp;
        IsHit           = false;
        #endregion
    }

    protected float CalculationMaxHp()
    {
        float levelValue = Level == 0 ? 1 : Level;
        return (_initialStatusSetup.InitialHp * levelValue * MaxHpMultiplier);
    }
}
