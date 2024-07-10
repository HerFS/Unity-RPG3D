using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

/*
 * File     : MonsterEntity.cs
 * Desc     : BaseGameEntity 상속
 *            Entity의 한 종류 Monster
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

[System.Serializable]
public class MonsterInfoUI
{
    public TextMeshProUGUI StatusText;
    public Slider HpSlider;
    public TextMeshProUGUI HpText;
}

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(MonsterStatus))]
public class MonsterEntity : BaseGameEntity, IDamageable
{
    [SerializeField]
    private GameObject _damageTextPrefab;

    private CapsuleCollider _collider;
    private Collider[] _nearMonsters;
    private Vector3 _itemSpawnPosition;

    private float _timer;

    public Transform SpawnPoint;

    public UnityEngine.Events.UnityEvent onDead;

    public readonly string AttackSpeed = "AttackSpeed";
    private readonly float _hpResetTime = 10f;

    private Vector3 _spawnPos;

    [Header("UI")]
    [SerializeField]
    private MonsterInfoUI _monsterInfoUI;
    public GameObject MonsterQuestMarker;

    [Header("Animation")]
    public AnimationClip AttackAnimation;
    public AnimationClip DieAnimation;

    [HideInInspector]
    public NodeGrid NodeGrid;
    [HideInInspector]
    public PathFinding PathFinding;
    [HideInInspector]
    public Rigidbody Rigidbody;
    [HideInInspector]
    public MonsterSpawner MonsterSpawner;
    [HideInInspector]
    public MonsterStatus MonsterStatus;
    [HideInInspector]
    public StateOfPlay<MonsterEntity>[] States;
    [HideInInspector]
    public StateMachine<MonsterEntity> StateMachine;
    [HideInInspector]
    public float Distance;

    public Transform Target { get; private set; }
    public int MonsterMask { get; private set; }

    private void Awake()
    {
        Setup();

        Rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        MonsterStatus = GetComponent<MonsterStatus>();

        #region Events
        MonsterStatus.onHpChanged += (monsterStatus, currentHp, prevHp) =>
        {
            _monsterInfoUI.HpSlider.value = currentHp;
            _monsterInfoUI.HpText.text = string.Format("{0: #,###;}", currentHp.ToString("F1")) + " / " + string.Format("{0: #,###;}", MonsterStatus.MaxHp.ToString("F1"));

            if (currentHp <= prevHp)
            {
                StartCoroutine(Hit());

                if (currentHp <= 0)
                {
                    MonsterStatus.MonsterFieldOfView.DectectionRadius.enabled = false;
                    MonsterStatus.MonsterFieldOfView.IsPlayerDetection = false;
                    _collider.enabled = false;

                    ChangeState(EnumTypes.MonsterState.Die);
                }
                else
                {
                    if (StateMachine.CurrentState != States[(int)EnumTypes.MonsterState.Attack])
                    {
                        ChangeState(EnumTypes.MonsterState.Attack);
                    }
                }
            }
        };
        #endregion
    }

    private void Start()
    {
        #region Setting
        MonsterSpawner = GetComponentInParent<MonsterSpawner>();

        NodeGrid = GetComponentInParent<NodeGrid>();
        PathFinding = GetComponentInParent<PathFinding>();
        PathFinding.NodeGrid = NodeGrid;
        PathFinding.StartPos = this.transform;
        _nearMonsters = new Collider[30]; // 스폰 몬스터 수로 정함

        Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        MonsterMask = 1 << (int)EnumTypes.LayerIndex.Monster;
        _spawnPos = this.transform.position;
        Target = GameManager.Instance.Player;
        Physics.IgnoreLayerCollision(8, 8, false);
        #endregion

        #region UI Setting
        _monsterInfoUI.HpSlider.maxValue = MonsterStatus.MaxHp;
        _monsterInfoUI.HpSlider.value = MonsterStatus.CurrentHp;

        _monsterInfoUI.StatusText.text = "LV." + MonsterStatus.Level + " " + MonsterStatus.Name;
        _monsterInfoUI.HpText.text = string.Format("{0: #,###;}", MonsterStatus.CurrentHp.ToString("F1")) + " / " + string.Format("{0: #,###;}", MonsterStatus.MaxHp.ToString("F1"));

        HideInfoUI();
        #endregion
    }

    private void OnEnable()
    {
        MonsterStatus.CurrentHp = MonsterStatus.MaxHp;

        if (StateMachine.CurrentState == States[(int)EnumTypes.MonsterState.Die])
        {
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            _collider.enabled = true;
            this.transform.position = SpawnPoint.position;
            ChangeState(EnumTypes.MonsterState.Idle);
            MonsterStatus.MonsterFieldOfView.DectectionRadius.enabled = true;
            MonsterStatus.MonsterFieldOfView.IsPlayerDetection = false;
        }
    }

    private void Update()
    {

        if (StateMachine.CurrentState != States[(int)EnumTypes.MonsterState.Idle] &&
            StateMachine.CurrentState != States[(int)EnumTypes.MonsterState.Die] && !MonsterSpawner.IsPlayerDetected)
        {
            MonsterStatus.MonsterFieldOfView.IsPlayerDetection = false;
            ChangeState(EnumTypes.MonsterState.Walk);
        }

        Updated();

        if (StateMachine.CurrentState == States[(int)EnumTypes.MonsterState.Idle])
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0f;
        }

        if (_timer >= _hpResetTime)
        {
            MonsterStatus.CurrentHp = MonsterStatus.MaxHp;
        }

        SpawnPoint.position = _spawnPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Contains(Globals.TagName.Weapon) && !MonsterStatus.IsHit)
        {
            float criticalChance = Random.Range(0f, 101f);

            TextMeshPro damageTextMesh = null;

            if (_damageTextPrefab)
            {
                var damageText = Instantiate(_damageTextPrefab, transform.position, Quaternion.identity, transform);
                damageTextMesh = damageText.GetComponent<TextMeshPro>();
            }

            if (DataManager.Instance.PlayerStatus.CriticalChance >= criticalChance)
            {
                if (damageTextMesh)
                {
                    damageTextMesh.text = DataManager.Instance.PlayerStatus.CriticalDamage.ToString();
                    damageTextMesh.color = Color.red;
                }

                MonsterStatus.CurrentHp -= HitDamageCalculation(DataManager.Instance.PlayerStatus.CriticalDamage);
            }
            else
            {
                if (damageTextMesh)
                {
                    damageTextMesh.text = DataManager.Instance.PlayerStatus.AttackDamage.ToString();
                    damageTextMesh.color = Color.yellow;
                }

                MonsterStatus.CurrentHp -= HitDamageCalculation(DataManager.Instance.PlayerStatus.AttackDamage);
            }

            Physics.OverlapSphereNonAlloc(transform.position, MonsterStatus.MonsterFieldOfView.Radius, _nearMonsters, MonsterMask, QueryTriggerInteraction.Ignore);

            foreach (var nearMonster in _nearMonsters)
            {
                if (nearMonster == null)
                {
                    continue;
                }

                if (nearMonster.TryGetComponent(out MonsterEntity nearMonsterEntity))
                {
                    if (nearMonsterEntity.StateMachine.CurrentState != nearMonsterEntity.States[(int)EnumTypes.MonsterState.Attack])
                    {
                        if (nearMonsterEntity.StateMachine.CurrentState != nearMonsterEntity.States[(int)EnumTypes.MonsterState.Die])
                        {
                            nearMonsterEntity.ChangeState(EnumTypes.MonsterState.Chasing);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player) &&
            StateMachine.CurrentState != States[(int)EnumTypes.MonsterState.Die])
        {
            MonsterStatus.MonsterFieldOfView.TargetDetection();
            ShowInfoUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            HideInfoUI();
        }

        if (other.gameObject.tag.Contains(Globals.TagName.Player) &&
            StateMachine.CurrentState == States[(int)EnumTypes.MonsterState.Chasing])
        {
            MonsterStatus.MonsterFieldOfView.IsPlayerDetection = false;
            ChangeState(EnumTypes.MonsterState.Walk);
        }
    }

    public override void Setup()
    {
        base.Setup();

        States = new StateOfPlay<MonsterEntity>[5];
        StateMachine = new StateMachine<MonsterEntity>();

        States[(int)EnumTypes.MonsterState.Idle] = new MonsterOwnedStates.Idle();
        States[(int)EnumTypes.MonsterState.Walk] = new MonsterOwnedStates.Walk();
        States[(int)EnumTypes.MonsterState.Chasing] = new MonsterOwnedStates.Chasing();
        States[(int)EnumTypes.MonsterState.Attack] = new MonsterOwnedStates.Attack();
        States[(int)EnumTypes.MonsterState.Die] = new MonsterOwnedStates.Die();

        StateMachine.Setup(this, States[(int)EnumTypes.MonsterState.Idle]);
    }

    public override void Updated()
    {
        StateMachine.Execute();
    }

    public void ChangeState(EnumTypes.MonsterState newState)
    {
        StateMachine.ChangeState(States[(int)newState]);
    }

    public void ShowInfoUI()
    {
        _monsterInfoUI.StatusText.gameObject.SetActive(true);
        _monsterInfoUI.HpSlider.gameObject.SetActive(true);
    }

    public void HideInfoUI()
    {
        _monsterInfoUI.StatusText.gameObject.SetActive(false);
        _monsterInfoUI.HpSlider.gameObject.SetActive(false);
        MonsterQuestMarker.gameObject.SetActive(false);
    }

    public IEnumerator Hit()
    {
        MonsterStatus.IsHit = true;

        yield return new WaitForSeconds(MonsterStatus.InvincibilityTime);

        MonsterStatus.IsHit = false;
    }


    public float HitDamageCalculation(float damage)
    {
        float hitDamage = 0f;

        if (MonsterStatus.Defense > damage)
        {
            hitDamage = 1f;
        }
        else
        {
            hitDamage = (damage - (MonsterStatus.Defense / 2));
        }

        StartCoroutine(Hit());

        return hitDamage;
    }

    public void Reward()
    {
        DataManager.Instance.PlayerStatus.CurrentExp += MonsterStatus.RewardExp;

        foreach (var item in MonsterStatus.ItemData)
        {
            int dropChance = Random.Range(0, 101);

            if (dropChance <= item.DropChance)
            {
                GameObject newItem = item.ItemData.DropItemPrefab;
                newItem.GetComponent<PickupItem>().Item = item.ItemData;
                newItem.GetComponent<PickupItem>().ItemQuantity = item.ItemQuantity;
                _itemSpawnPosition = new Vector3(this.transform.position.x, (this.transform.position.y + 0.1f), this.transform.position.z);
                Instantiate(newItem, _itemSpawnPosition, Quaternion.identity);
            }
        }
    }
}
