using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PlayerStatus.cs
 * Desc     : BaseEntitiyStatus 상속
 *            플레이어의 데이터들
 * Date     : 2024-07-10
 * Writer   : 정지훈
 */

public class PlayerStatus : BaseEntityStatus
{
    #region Events
    private delegate void SceneChangedHandler(PlayerStatus playerStatus, string currentScene, string prevScene);
    private delegate void MoneyChangedHandler(PlayerStatus playerStatus, uint currentMoney, uint prevMoney);
    private delegate void MpChangedHandler(PlayerStatus playerStatus, float currentMp, float prevMp);
    private delegate void MaxMpChangedHandler(PlayerStatus playerStatus, float currentMaxMp, float prevMaxMp);
    private delegate void StaminaChangedHandler(PlayerStatus playerStatus, float currentStamina, float prevStamina);
    private delegate void ExpChangedHandler(PlayerStatus playerStatus, float currentExp, float prevExp);
    private delegate void RequiredExpChangedHandler(PlayerStatus playerStatus, float currentRequiredExp, float prevRequiredExp);
    #endregion

    private readonly int _maxMoney = 999999999;
    private readonly float _maxMpMultiplier = 2.0f;
    private readonly float _levelUpParticlePlayTime = 2f;

    public readonly float JumpHeight = 5f;
    public readonly float MaxStaminaValue = 10f;
    public readonly float HpRecoveryDelayTime = 0.5f;
    public readonly float MpRecoveryDelayTime = 0.3f;
    public readonly float StaminaRecoveryDelayTime = 0.2f;
    public readonly float StaminaRecoveryValue = 0.1f;

    [Header("Player")]
    [SerializeField]
    private CalculateRequiredExp _calculatersExp;
    [SerializeField, Range(0f, 10f)]
    private float _runSpeed;
    [SerializeField]
    private float _initialMp;

    [SerializeField]
    private ParticleSystem _levelupParticle;

    private string _currentScene;
    private uint _money;

    private float _maxMp;
    private float _currentStamina;
    private float _currentExp;
    private float _currentMp;
    private float _requiredExp;

    private event SceneChangedHandler onSceneChanged;
    private event MoneyChangedHandler onMoneyChanged;
    private event MpChangedHandler onMpChanged;
    private event MaxMpChangedHandler onMaxMpChanged;
    private event StaminaChangedHandler onStaminaChanged;
    private event ExpChangedHandler onExpChanged;
    private event RequiredExpChangedHandler onRequiredExpChanged;

    public float RunSpeed
    {
        get { return _runSpeed; }
        set
        {
            _runSpeed = Mathf.Clamp(value, 0f, 10f);
        }
    }

    public float MaxMp
    {
        get { return _maxMp; }
        set
        {
            float prevMaxMp = _maxMp;
            _maxMp = value;
            if (prevMaxMp != _maxMp)
            {
                onMaxMpChanged?.Invoke(this, _maxMp, prevMaxMp);
            }
        }
    }

    public float RequiredExp
    {
        get { return _requiredExp; }
        set
        {
            float prevExp = _requiredExp;
            _requiredExp = value;
            if (_requiredExp != prevExp)
            {
                onRequiredExpChanged?.Invoke(this, _requiredExp, prevExp);
            }
        }
    }

    public string CurrentScene
    {
        get { return _currentScene; }
        set
        {
            string prevScene = _currentScene;
            _currentScene = value;
            if (_currentScene != prevScene)
            {
                onSceneChanged?.Invoke(this, _currentScene, prevScene);
            }
        }
    }

    public uint Money
    {
        get { return _money; }
        set
        {
            uint prevMoney = _money;
            _money = (uint)Mathf.Clamp(value, 0, _maxMoney);
            if (_money != prevMoney)
            {
                onMoneyChanged?.Invoke(this, _money, prevMoney);
            }
        }
    }
    public float CurrentMp
    {
        get { return _currentMp; }
        set
        {
            float prevMp = _currentMp;
            _currentMp = Mathf.Clamp(value, 0f, MaxMp);
            if (_currentMp != prevMp)
            {
                onMpChanged?.Invoke(this, _currentMp, prevMp);
            }
        }
    }

    public float CurrentStamina
    {
        get { return _currentStamina; }
        set
        {
            float prevStamina = _currentStamina;
            _currentStamina = Mathf.Clamp(value, 0f, MaxStaminaValue);

            if (_currentStamina != prevStamina)
            {
                onStaminaChanged?.Invoke(this, _currentStamina, prevStamina);
            }
        }
    }

    public float CurrentExp
    {
        get { return _currentExp; }
        set
        {
            float prevExp = _currentExp;
            _currentExp = value;
            if (_currentExp != prevExp)
            {
                onExpChanged?.Invoke(this, _currentExp, prevExp);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        InvincibilityTime = 0.5f;
        _requiredExp = CalculationExp();
        _maxMp = CalculationMaxMp();
        _currentStamina = MaxStaminaValue;
        _currentMp = _maxMp;

        _levelupParticle.gameObject.SetActive(false);
        #region BaseEvents
        onNameChanged += (baseStatus, newName, prevName) =>
        {
            UIManager.Instance.PlayerStatusText.Name.text = newName;
            UIManager.Instance.PlayerInfoPanel.NameText.text = newName;

            DataManager.Instance.PlayerData.Name = newName;
            DataManager.Instance.SaveStausData();
        };
        onHpChanged += (baseStatus, currentHp, prevHp) =>
        {
            if (currentHp <= 0)
            {
                PlayerEntity.ChangeState(EnumTypes.PlayerState.Die);
            }

            UIManager.Instance.PlayerStatusText.Hp.text = string.Format("{0: #,###; -#,###;0}", currentHp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", MaxHp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.HpText.text = string.Format("{0: #,###; -#,###;0}", currentHp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", MaxHp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.HpSlider.value = currentHp;

            DataManager.Instance.PlayerData.CurrentHp = currentHp;
            DataManager.Instance.SaveStausData();
        };
        onMaxHpChanged += (baseStatus, currentMaxHp, prevMaxHp) =>
        {
            if (currentMaxHp <= CurrentHp)
            {
                CurrentHp = currentMaxHp;
            }

            UIManager.Instance.PlayerStatusText.Hp.text = string.Format("{0: #,###; -#,###;0}", CurrentHp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", currentMaxHp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.HpText.text = string.Format("{0: #,###; -#,###;0}", CurrentHp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", currentMaxHp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.HpSlider.value = CurrentHp;
            UIManager.Instance.PlayerInfoPanel.HpSlider.maxValue = currentMaxHp;

            DataManager.Instance.PlayerData.MaxHp = currentMaxHp;
            DataManager.Instance.SaveStausData();
        };
        onAttackDamage += (baseStatus, currentDamage, prevDamage) =>
        {
            UIManager.Instance.PlayerStatusText.Attack.text = currentDamage.ToString("F1");
            DataManager.Instance.PlayerData.AttackDamage = currentDamage;
            DataManager.Instance.SaveStausData();
        };
        onCriticalDamageChanged += (baseStatus, currentCritical, prevCritical) =>
        {
            DataManager.Instance.PlayerData.CriticalDamage = currentCritical;
            DataManager.Instance.SaveStausData();
        };
        onCriticalChanceChanged += (baseStatus, currentChance, prevChance) =>
        {
            UIManager.Instance.PlayerStatusText.CriticalChance.text = currentChance.ToString("F1");
            DataManager.Instance.PlayerData.CriticalChance = currentChance;
            DataManager.Instance.SaveStausData();
        };
        onDefenseChanged += (baseStatus, currentDefense, prevDefense) =>
        {
            UIManager.Instance.PlayerStatusText.Defense.text = currentDefense.ToString("F1");
            DataManager.Instance.PlayerData.Defense = currentDefense;
            DataManager.Instance.SaveStausData();
        };
        onLevelChanged += (baseStatus, currentLevel, prevLevel) =>
        {
            if (currentLevel >= MaxLevelValue)
            {
                UIManager.Instance.PlayerStatusText.Level.text = "Max";
            }
            else
            {
                UIManager.Instance.PlayerStatusText.Level.text = currentLevel.ToString();
            }

            UIManager.Instance.PlayerInfoPanel.LevelText.text = currentLevel.ToString();

            DataManager.Instance.PlayerData.Level = currentLevel;
            DataManager.Instance.SaveStausData();
        };
        #endregion

        #region PlayerEvents
        onSceneChanged += (baseStatus, currentScene, prevScene) =>
        {
            if (GameManager.Instance.IsGameStart)
            {
                DataManager.Instance.PlayerData.CurrentScene = currentScene;
                DataManager.Instance.SaveStausData();
            }
        };
        onMoneyChanged += (baseStatus, currentMoney, prevMoney) =>
        {
            UIManager.Instance.PlayerStatusText.Money.text = string.Format("{0: #,###; -#,###;0}", currentMoney);
            DataManager.Instance.PlayerData.Money = currentMoney;
            DataManager.Instance.SaveStausData();
        };
        onMpChanged += (baseStatus, currentMp, prevMp) =>
        {
            UIManager.Instance.PlayerStatusText.Mp.text = string.Format("{0: #,###; -#,###;0}", currentMp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", MaxMp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.MpText.text = string.Format("{0: #,###; -#,###;0}", currentMp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", MaxMp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.MpSlider.value = currentMp;

            DataManager.Instance.PlayerData.CurrentMp = currentMp;
            DataManager.Instance.SaveStausData();
        };
        onMaxMpChanged += (baseStatus, currentMaxMp, prevMaxMp) =>
        {
            if (currentMaxMp <= CurrentMp)
            {
                CurrentMp = currentMaxMp;
            }

            UIManager.Instance.PlayerStatusText.Mp.text = string.Format("{0: #,###; -#,###;0}", CurrentMp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", currentMaxMp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.MpText.text = string.Format("{0: #,###; -#,###;0}", CurrentMp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", currentMaxMp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.MpSlider.value = CurrentMp;
            UIManager.Instance.PlayerInfoPanel.MpSlider.maxValue = currentMaxMp;

            DataManager.Instance.PlayerData.MaxMp = currentMaxMp;
            DataManager.Instance.SaveStausData();
        };
        onExpChanged += (baseStatus, currentExp, prevExp) =>
        {
            if (currentExp >= RequiredExp && Level != MaxLevelValue)
            {
                if (!_levelupParticle.gameObject.activeSelf)
                {
                    _levelupParticle.gameObject.SetActive(true);
                }

                StartCoroutine(PlayLevelUpParticle());

                DataManager.Instance.PlayerData.CurrentExp = currentExp - RequiredExp;
                _currentExp = DataManager.Instance.PlayerData.CurrentExp;

                ++Level;

                MaxHp = CalculationMaxHp();
                MaxMp = CalculationMaxMp();
                RequiredExp = CalculationExp();
                CriticalChance += 3;

                DataManager.Instance.PlayerData.MaxHp = MaxHp;
                DataManager.Instance.PlayerData.MaxMp = MaxMp;

                UIManager.Instance.PlayerInfoPanel.HpSlider.maxValue = MaxHp;
                UIManager.Instance.PlayerInfoPanel.MpSlider.maxValue = MaxMp;
                UIManager.Instance.PlayerInfoPanel.ExpSlider.maxValue = RequiredExp;

                AttackDamage += 2.5f;
                CriticalDamage = AttackDamage;

                CurrentHp = MaxHp;
                CurrentMp = MaxMp;
            }
            else
            {
                DataManager.Instance.PlayerData.CurrentExp = currentExp;
            }

            UIManager.Instance.PlayerStatusText.Exp.text = string.Format("{0: #,###; -#,###;0}", CurrentExp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", RequiredExp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.ExpText.text = string.Format("{0: #,###; -#,###;0}", CurrentExp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", RequiredExp.ToString("F1")) + " (" + (100 * (CurrentExp / RequiredExp)).ToString("N1") + "%)";
            UIManager.Instance.PlayerInfoPanel.ExpSlider.value = CurrentExp;

            DataManager.Instance.SaveStausData();
        };

        onRequiredExpChanged += (playerStatus, currentRequiredExp, prevRequiredExp) =>
        {
            UIManager.Instance.PlayerStatusText.Exp.text = string.Format("{0: #,###; -#,###;0}", CurrentExp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", currentRequiredExp.ToString("F1"));
            UIManager.Instance.PlayerInfoPanel.ExpText.text = string.Format("{0: #,###; -#,###;0}", CurrentExp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", currentRequiredExp.ToString("F1")) + " (" + (100 * (CurrentExp / RequiredExp)).ToString("N1") + "%)";

            DataManager.Instance.PlayerData.RequiredExp = currentRequiredExp;
            DataManager.Instance.SaveStausData();
        };

        onStaminaChanged += (playerStatus, currentStamina, prevStamina) =>
        {
            UIManager.Instance.PlayerInfoPanel.StaminaSlider.value = currentStamina;
            DataManager.Instance.PlayerData.CurrentStamina = currentStamina;
            DataManager.Instance.SaveStausData();
        };
        #endregion
    }

    private float CalculationExp()
    {
        int solveForRequiredExp = 0;
        for (int levelCycle = 1; levelCycle <= Level; ++levelCycle)
        {
            solveForRequiredExp += (int)Mathf.Floor(levelCycle + _calculatersExp.AdditionMultiplier * Mathf.Pow(_calculatersExp.PowerMultiplier, levelCycle / _calculatersExp.DivisionMultiplier));
        }

        return (solveForRequiredExp / 4);
    }

    private float CalculationMaxMp()
    {
        float levelValue = Level == 0 ? 1 : Level;
        return (_initialMp * levelValue * _maxMpMultiplier);
    }

    private IEnumerator PlayLevelUpParticle()
    {
        _levelupParticle.Play();
        yield return new WaitForSeconds(_levelUpParticlePlayTime);
        _levelupParticle.Stop();
    }
}
