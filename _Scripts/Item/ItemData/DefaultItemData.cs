using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ETCItemData.cs
 * Desc     : CountableItemData를 상속
 *            기타 아이템 속성
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Inventory System/Item Data/Default", fileName = "Item_Default_")]
public class DefaultItemData : CountableItemData
{
    public bool IsMoney;
}
