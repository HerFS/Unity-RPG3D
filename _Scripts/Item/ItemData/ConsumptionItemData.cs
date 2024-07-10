using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PotionItemData.cs
 * Desc     : CountableItemData�� ���
 *            �����̳� ��� �������� �Ӽ�
 * Date     : 2024-05-07
 * Writer   : ������
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
    [SerializeField, Tooltip("ȸ����, ������ �� ������ ġ��")]
    private float _value;

    public ConsumptionType ConsumptionType => _consumptionType;
    public float Value => _value;
}
