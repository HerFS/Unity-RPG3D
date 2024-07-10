using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : WeaponItemData.cs
 * Desc     : EquipmentItemData를 상속
 *        무기 아이템 속성
 * Date     : 2024-05-07
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory System/Item Data/Weapon", fileName = "Item_Weapon_")]
public class WeaponItemData : EquipmentItemData
{
    [Header("Weapon")]
    [SerializeField]
    private float _damage;

    public float Damage => _damage;
}
