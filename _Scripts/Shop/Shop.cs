using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Shop.cs
 * Desc     : 상점
 * Date     : 2024-06-16
 * Writer   : 정지훈
 */

public class Shop : DontDestroyObject<Shop>
{
    public readonly int InitialSlotCount = 10;
    public ShopItemSlot[] ItemSlots;
}
