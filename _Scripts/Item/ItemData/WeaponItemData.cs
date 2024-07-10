using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : WeaponItemData.cs
 * Desc     : EquipmentItemData�� ���
 *        ���� ������ �Ӽ�
 * Date     : 2024-05-07
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory System/Item Data/Weapon", fileName = "Item_Weapon_")]
public class WeaponItemData : EquipmentItemData
{
    [Header("Weapon")]
    [SerializeField]
    private float _damage;

    public float Damage => _damage;
}
