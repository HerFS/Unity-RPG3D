using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * File     : Inventory.cs
 * Desc     : �κ��丮 ���� �� ����
 * Date     : 2024-06-30
 * Writer   : ������
 */

public class Inventory : DontDestroyObject<Inventory>
{
    [HideInInspector]
    public ItemSlot[] ItemSlots;

    [Header("Setting")]
    public GameObject SlotsPanel;
    [SerializeField]
    private GameObject _slotUIPrefab;
    [SerializeField, Range(6, 60)]
    private int _initialSlotCount = 6;

    public int Capacity { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Capacity = _initialSlotCount;
        ItemSlots = new ItemSlot[Capacity];

        for (int i = 0; i < Capacity; ++i)
        {
            GameObject newSlot = Instantiate(_slotUIPrefab, SlotsPanel.transform);
            newSlot.name = $"ItemSlot[{i:D2}]";
            ItemSlots[i] = newSlot.GetComponent<ItemSlot>();
        }
    }

    public uint AddItem(ItemData pickupItemData, ref uint quantity, PickupItem pickupItem = null)
    {
        int index = 0;

        if (pickupItemData is CountableItemData countableData)
        {
            if (pickupItemData is DefaultItemData defaultItemData)
            {
                if (defaultItemData.IsMoney)
                {
                    DataManager.Instance.PlayerStatus.Money += pickupItem.MoneyValue;
                    return 0;
                }
            }

            index = -1;
            while (quantity > 0)
            {
                // 1. ���� ������ �ִ� �� ã�� => ������ -1 ��ȯ
                index = FindCountableItemSlotIndex(pickupItemData, index + 1);

                // 2-1. ���� �������� ������ üũ
                if (index == -1)
                {
                    // 3. �� ���� ã��
                    index = FindEmptySlotIndex(index + 1);

                    // 4-1. �� ������ ������
                    if (index == -1)
                    {
                        // 5. ���� ���� ��ȯ
                        return quantity;
                    }
                    // 4-2. �� ������ ������
                    else
                    {
                        // 5. ���� ������ �ִ� ������ ������ üũ
                        if (countableData.MaxQuantity >= quantity)
                        {
                            // 6-1. ���� ������ �ִ� ������ ������ ������ quantity ��ŭ �߰� �ϰ� ���� ����
                            ItemSlots[index].SetupItem(pickupItemData, quantity);
                            return 0;
                        }
                        else
                        {
                            // 6-2. �ִ� ������ �ݰ� �������� �� ĭ�� �� ã�Ƽ� �־� �� ������, �� ������ üũ
                            quantity = quantity - countableData.MaxQuantity;
                            ItemSlots[index].SetupItem(pickupItemData, countableData.MaxQuantity); // �ִ밪�� �־���
                        }
                    }
                }
                // 2-2. ���� �������� ������
                else
                {
                    var addQuantity = ItemSlots[index].ItemQuantity + quantity;
                    // 3-1. ������ ���� ���� ���� ���� �ƽ� ������ ������ 
                    if (addQuantity < countableData.MaxQuantity)
                    {
                        // 4. ���� �����ۿ� ������ ����
                        ItemSlots[index].SetSlotCount(pickupItemData, quantity);

                        return 0;
                    }
                    // 3-2. ������ ���� ���� ���� ���� �� ������
                    else
                    {
                        // 4. ���� �����ۿ� �ִ� ���� ����
                        ItemSlots[index].SetSlotCount(pickupItemData, countableData.MaxQuantity);
                        quantity = addQuantity - countableData.MaxQuantity;
                    }
                }
            }
        }
        else
        {
            index = FindEmptySlotIndex();
            if (index != -1)
            {
                ItemSlots[index].SetupItem(pickupItemData);
                ItemSlots[index].UpdateText();
                return 0;
            }
        }

        return quantity;
    }

    public int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; ++i)
        {
            if (ItemSlots[i].Item == null)
            {
                return i;
            }
        }
        return -1;
    }

    public int FindCountableItemSlotIndex(ItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; ++i)
        {
            var currentItem = ItemSlots[i].Item;
            if (currentItem == null)
            {
                continue;
            }

            if (currentItem == target && currentItem is CountableItemData countableData)
            {
                if (ItemSlots[i].ItemQuantity != countableData.MaxQuantity)
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
