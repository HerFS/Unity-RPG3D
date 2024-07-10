using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Shop.cs
 * Desc     : ����
 * Date     : 2024-06-16
 * Writer   : ������
 */

public class Shop : DontDestroyObject<Shop>
{
    public readonly int InitialSlotCount = 10;
    public ShopItemSlot[] ItemSlots;
}
