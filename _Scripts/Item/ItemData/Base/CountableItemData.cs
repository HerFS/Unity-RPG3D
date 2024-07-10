using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : CountableItemData.cs
 * Desc     : ItemData를 상속
 *            셀 수 있는 아이템 속성
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public abstract class CountableItemData : ItemData
{
    [Header("Countable")]
    [Range(1, 99)]
    public uint MaxQuantity;
}
