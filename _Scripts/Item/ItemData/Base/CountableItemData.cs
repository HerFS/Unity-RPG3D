using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : CountableItemData.cs
 * Desc     : ItemData�� ���
 *            �� �� �ִ� ������ �Ӽ�
 * Date     : 2024-06-30
 * Writer   : ������
 */

public abstract class CountableItemData : ItemData
{
    [Header("Countable")]
    [Range(1, 99)]
    public uint MaxQuantity;
}
