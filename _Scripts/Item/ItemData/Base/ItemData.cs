using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ItemData.cs
 * Desc     : ScriptableObject
 *            Item들의 속성
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public abstract class ItemData : ScriptableObject
{
    [Header("Setting")]
    public int Id;
    public Sprite Icon;
    public string Name;
    [TextArea]
    public string Description;
    public GameObject DropItemPrefab;
    public bool CanSellable = true;
    [HideInInspector]
    public int ItemPrice;
    [HideInInspector]
    public int SalePrice;
}
