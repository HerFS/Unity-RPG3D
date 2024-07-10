using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : EquipmentItemData.cs
 * Desc     : ItemData�� ���
 *            ��� ������ �Ӽ�
 * Date     : 2024-05-07
 * Writer   : ������
 */

public abstract class EquipmentItemData : ItemData
{
    [Header("Equipment")]
    [Tooltip("��� ����")]
    public EnumTypes.EquipmentType EquipmentType;
    [SerializeField, Tooltip("������ ��������")]
    private bool _isRepairable;
    [SerializeField, Tooltip("�ı��� ��������")]
    private bool _isIndestructible;
    [SerializeField]
    private float _maxDurability = 100;

    public bool IsIndestructible => _isIndestructible;
    public float MaxDurability => _maxDurability;
    public bool IsRepairable => _isRepairable;
}
