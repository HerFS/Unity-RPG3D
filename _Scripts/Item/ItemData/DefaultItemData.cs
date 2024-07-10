using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ETCItemData.cs
 * Desc     : CountableItemData�� ���
 *            ��Ÿ ������ �Ӽ�
 * Date     : 2024-06-30
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory System/Item Data/Default", fileName = "Item_Default_")]
public class DefaultItemData : CountableItemData
{
    public bool IsMoney;
}
