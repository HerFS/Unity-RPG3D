using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ArmorItemData.cs
 * Desc     : EquipmentItemData¸¦ »ó¼Ó
 *            °©¿Ê ¾ÆÀÌÅÛ ¼Ó¼º
 * Date     : 2024-06-17
 * Writer   : Á¤ÁöÈÆ
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory System/Item Data/Armor", fileName = "Item_Armor_")]
public class ArmorItemData : EquipmentItemData
{
    [Header("Defense")]
    [SerializeField]
    private float _defense;
    [SerializeField]
    private float _hp;
    [SerializeField]
    private float _mp;

    public float Defense => _defense;
    public float Hp => _hp;
    public float Mp => _mp;
}
