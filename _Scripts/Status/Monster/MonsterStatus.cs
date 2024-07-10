using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * File : MonsterStatus.cs
 * Desc : BaseEntitiyStatus 상속
 *        몬스터의 데이터들
 * Date : 2024-05-05
 */

[System.Serializable]
public struct RewardItem
{
    public ItemData ItemData;
    public uint ItemQuantity;
    public float DropChance;
}

[RequireComponent(typeof(FieldOfView))]
public class MonsterStatus : BaseEntityStatus
{
    [Header("Reward")]
    [SerializeField]
    private float _rewardExp;
    [SerializeField]
    private RewardItem[] _rewardItem;
    // 아이템 추가, 돈 추가

    [Header("Attack")]
    [SerializeField]
    private EnumTypes.MonsterAttackType _monsterAttackType;
    [SerializeField, Range(1f, 20f)]
    private float _attackRange;
    [SerializeField, Range(0f, 5f)]
    private float _attackSpeed;

    [HideInInspector]
    public BoxCollider WeaponCollider;
    [HideInInspector]
    public Transform BulletSpwanTransform;
    [HideInInspector]
    public FieldOfView MonsterFieldOfView;
    [HideInInspector]
    public ObjectPool BulletPool;

    public float AttackRange
    {
        get { return _attackRange; }
    }

    public float AttackSpeed
    {
        get { return _attackSpeed; }
    }

    public float RewardExp
    {
        get { return _rewardExp; }
    }
    
    public IReadOnlyList<RewardItem> ItemData
    {
        get { return _rewardItem; }
    }


    public EnumTypes.MonsterAttackType MonsterAttackType
    {
        get { return _monsterAttackType; }
    }

    protected override void Awake()
    {
        base.Awake();

        BulletPool = GetComponent<ObjectPool>();
        MonsterFieldOfView = GetComponent<FieldOfView>();
    }

    private void Start()
    {
        #region Setting
        if (WeaponCollider)
        {
            WeaponCollider.isTrigger = true;
            WeaponCollider.enabled = false;
        }

        InvincibilityTime = 0.3f;
        #endregion
    }

    // added animation event for melee attack
    public void EnableMeleeAttackCollider()
    {
        WeaponCollider.enabled = true;
    }

    // added animation event for melee attack
    public void DisableMeleeAttackCollider()
    {
        WeaponCollider.enabled = false;
    }

    // added animation event for range attack
    public void RangeAttack()
    {
        GameObject newBullet = BulletPool.Allocate();
        newBullet.transform.position = BulletSpwanTransform.position;
        newBullet.GetComponent<MonsterBullet>().BulletDamage = AttackDamage;
    }
}
