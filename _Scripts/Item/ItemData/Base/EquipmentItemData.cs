using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : EquipmentItemData.cs
 * Desc     : ItemData를 상속
 *            장비 아이템 속성
 * Date     : 2024-05-07
 * Writer   : 정지훈
 */

public abstract class EquipmentItemData : ItemData
{
    [Header("Equipment")]
    [Tooltip("장비 종류")]
    public EnumTypes.EquipmentType EquipmentType;
    [SerializeField, Tooltip("수리가 가능한지")]
    private bool _isRepairable;
    [SerializeField, Tooltip("파괴가 가능한지")]
    private bool _isIndestructible;
    [SerializeField]
    private float _maxDurability = 100;

    public bool IsIndestructible => _isIndestructible;
    public float MaxDurability => _maxDurability;
    public bool IsRepairable => _isRepairable;
}
