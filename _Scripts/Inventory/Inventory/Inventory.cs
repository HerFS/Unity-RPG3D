using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * File     : Inventory.cs
 * Desc     : 인벤토리 관리 및 저장
 * Date     : 2024-06-30
 * Writer   : 정지훈
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
                // 1. 같은 아이템 있는 지 찾기 => 없으면 -1 반환
                index = FindCountableItemSlotIndex(pickupItemData, index + 1);

                // 2-1. 같은 아이템이 없는지 체크
                if (index == -1)
                {
                    // 3. 빈 슬롯 찾기
                    index = FindEmptySlotIndex(index + 1);

                    // 4-1. 빈 슬롯이 없으면
                    if (index == -1)
                    {
                        // 5. 남은 개수 반환
                        return quantity;
                    }
                    // 4-2. 빈 슬롯이 있으면
                    else
                    {
                        // 5. 먹을 수량이 최대 값보다 적은지 체크
                        if (countableData.MaxQuantity >= quantity)
                        {
                            // 6-1. 먹을 수량이 최대 값보다 적으면 아이템 quantity 만큼 추가 하고 루프 나감
                            ItemSlots[index].SetupItem(pickupItemData, quantity);
                            return 0;
                        }
                        else
                        {
                            // 6-2. 최대 개수만 줍고 나머지는 빈 칸을 또 찾아서 넣어 줄 것인지, 말 것인지 체크
                            quantity = quantity - countableData.MaxQuantity;
                            ItemSlots[index].SetupItem(pickupItemData, countableData.MaxQuantity); // 최대값만 넣어줌
                        }
                    }
                }
                // 2-2. 같은 아이템이 있으면
                else
                {
                    var addQuantity = ItemSlots[index].ItemQuantity + quantity;
                    // 3-1. 수량과 수량 개수 더한 값이 맥스 값보다 적으면 
                    if (addQuantity < countableData.MaxQuantity)
                    {
                        // 4. 같은 아이템에 수량만 더함
                        ItemSlots[index].SetSlotCount(pickupItemData, quantity);

                        return 0;
                    }
                    // 3-2. 수량과 수량 개수 더한 값이 더 많으면
                    else
                    {
                        // 4. 같은 아이템에 최대 값만 남김
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
