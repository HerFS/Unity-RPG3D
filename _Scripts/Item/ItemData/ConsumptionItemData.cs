using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PotionItemData.cs
 * Desc     : CountableItemData를 상속
 *            포션이나 사용 아이템의 속성
 * Date     : 2024-05-07
 * Writer   : 정지훈
 */

public enum ConsumptionType
{
    Hp,
    Mp,
    Exp
}

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory System/Item Data/Consumption", fileName = "Item_Consumption_")]
public class ConsumptionItemData : CountableItemData
{
    [Header("Consumption")]
    [SerializeField]
    private ConsumptionType _consumptionType;
    [SerializeField, Tooltip("회복량, 버프량 등 오르는 치수")]
    private float _value;

    public ConsumptionType ConsumptionType => _consumptionType;
    public float Value => _value;
}
