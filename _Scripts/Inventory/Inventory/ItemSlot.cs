using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/*
 * File     : ItemSlot.cs
 * Desc     : Inventory의 Slot들
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private delegate void ChangedItemHandler(ItemSlot itemSlot, ItemData currentItem, ItemData prevItem);
    private delegate void ChangedItemQuantityHandler(ItemSlot itemSlot, uint currentItemQuantity, uint prevItemQuantity = 0);

    //[HideInInspector]
    private ItemData _item;
    private uint _itemQuantity;
    public Image IconImage;

    public TextMeshProUGUI QuantityText;

    private RectTransform _rectTransform;

    private event ChangedItemHandler onChangedItem;
    private event ChangedItemQuantityHandler onChangedItemQuantity;

    public ItemData Item
    {
        get
        { 
            return _item; 
        }

        set
        {
            ItemData prevItem = _item;
            _item = value;

            if (_item != prevItem)
            {
                onChangedItem?.Invoke(this, _item, prevItem);
            }
        }
    }

    public uint ItemQuantity
    {
        get
        {
            return _itemQuantity;
        }

        set
        {
            uint prevItemQuantity = _itemQuantity;
            _itemQuantity = value;

            if (_item is CountableItemData countableData)
            {
                _itemQuantity = (uint)Mathf.Clamp(value, 0, countableData.MaxQuantity);
            }
            else if (_item != null)
            {
                _itemQuantity = 1;
            }

            if (_itemQuantity != prevItemQuantity)
            {
                onChangedItemQuantity?.Invoke(this, _itemQuantity, prevItemQuantity);
            }
        }
    }

    public bool IsEmpty => (Item == null || ItemQuantity <= 0);
    public bool HasItem => (Item != null);
    public bool IsSplit = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        #region Initial
        QuantityText.color = Color.white;
        UpdateIcon();
        UpdateText();
        #endregion

        int itemSlotIndex = int.Parse(Regex.Replace(this.name, @"\D", ""));

        onChangedItem += (itemSlot, currentItem, prevItem) => // 수정 필요
        {
            if (currentItem == null)
            {
                DataManager.Instance.SaveItemDatas[itemSlotIndex] = null;
            }
            else
            {
                DataManager.Instance.SaveItemDatas[itemSlotIndex].ItemName = currentItem.name;
                if (currentItem is EquipmentItemData equipmentData)
                {
                    DataManager.Instance.SaveItemDatas[itemSlotIndex].EquipmentType = equipmentData.EquipmentType;
                }
            }

            DataManager.Instance.SaveItemData();
        };

        onChangedItemQuantity += (itemSlot, currentItemQuantity, prevItemQuantity) =>
        {
            if (IsEmpty)
            {
                ClearSlot();
                DataManager.Instance.SaveItemDatas[itemSlotIndex] = null;
            }
            else
            {
                DataManager.Instance.SaveItemDatas[itemSlotIndex].ItemQuantity = currentItemQuantity;
                itemSlot.UpdateText();
            }

            DataManager.Instance.SaveItemData();
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HasItem)
        {
            UIManager.Instance.ItemToolTipPanel.MyRectTransform.position = new Vector2(eventData.position.x + _rectTransform.rect.width, eventData.position.y - UIManager.Instance.ItemToolTipPanel.MyRectTransform.rect.height);
            UIManager.Instance.ItemToolTipPanel.UpdateToolTip(Item);
            UIManager.Instance.ItemToolTipPanel.ShowToolTip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.ItemToolTipPanel.HideToolTip();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlot[] itemSlots = DataManager.Instance.Inventory.ItemSlots;
        GameObject draggedItem = eventData.pointerDrag;
        ItemIcon draggedItemIcon;
        EquipmentIcon draggedEquipmentIcon;

        int currentSiblingIndex = this.transform.GetSiblingIndex();

        if (draggedItem.TryGetComponent<ItemIcon>(out draggedItemIcon))
        {
            int draggedSiblingIndex = draggedItemIcon.ParentAfterDrag.GetSiblingIndex();

            // 빈 슬롯 + 쉬프트 + 셀 수 있는 아이템
            if (InputManager.Instance.IsLeftShift && !itemSlots[currentSiblingIndex].HasItem && itemSlots[draggedSiblingIndex].Item is CountableItemData)
            {
                UIManager.Instance.ItemPopupPanel.ShowQuantityUI();
                UIManager.Instance.ItemPopupPanel.SetQuantityItemName(itemSlots[draggedSiblingIndex].Item.Name);
                itemSlots[draggedSiblingIndex].IsSplit = true;

                UIManager.Instance.ItemPopupPanel.QuantityOkBtn.onClick.AddListener(() =>
                {
                    if (itemSlots[draggedSiblingIndex].IsSplit && itemSlots[currentSiblingIndex].Item == null)
                    {
                        uint.TryParse(UIManager.Instance.ItemPopupPanel.QuantityInputField.text, out uint inputQuantity);

                        uint quantitySplit = itemSlots[draggedSiblingIndex].ItemQuantity - inputQuantity;

                        if (quantitySplit <= 0)
                        {
                            SwapSlot(itemSlots, currentSiblingIndex, draggedSiblingIndex);
                        }
                        else
                        {
                            SetupItem(itemSlots[draggedSiblingIndex].Item, inputQuantity);

                            itemSlots[draggedSiblingIndex].ItemQuantity = quantitySplit;
                            itemSlots[draggedSiblingIndex].UpdateText();
                        }

                        UIManager.Instance.ItemPopupPanel.InitalInputField();
                        UIManager.Instance.ItemPopupPanel.HideQuantityUI();
                        itemSlots[draggedSiblingIndex].IsSplit = false;
                    }
                });
            }
            // 같은 아이템 + 셀 수 있는 아이템 + 다른 슬롯
            else if ((itemSlots[currentSiblingIndex].Item == itemSlots[draggedSiblingIndex].Item) && (itemSlots[currentSiblingIndex].Item is CountableItemData currentCountableData) && (currentSiblingIndex != draggedSiblingIndex))
            {
                if (itemSlots[currentSiblingIndex].ItemQuantity < currentCountableData.MaxQuantity)
                {
                    uint quantitySplit = itemSlots[currentSiblingIndex].ItemQuantity - itemSlots[draggedSiblingIndex].ItemQuantity;
                    uint totalQuantity = itemSlots[currentSiblingIndex].ItemQuantity + itemSlots[draggedSiblingIndex].ItemQuantity;

                    if (totalQuantity <= currentCountableData.MaxQuantity)
                    {
                        itemSlots[currentSiblingIndex].Item = itemSlots[draggedSiblingIndex].Item;
                        itemSlots[currentSiblingIndex].ItemQuantity += itemSlots[draggedSiblingIndex].ItemQuantity;

                        itemSlots[draggedSiblingIndex].ClearSlot();

                        itemSlots[currentSiblingIndex].UpdateText();
                    }
                    else
                    {
                        itemSlots[draggedSiblingIndex].ItemQuantity = totalQuantity - currentCountableData.MaxQuantity;
                        itemSlots[currentSiblingIndex].ItemQuantity = currentCountableData.MaxQuantity;

                        itemSlots[currentSiblingIndex].UpdateText();
                        itemSlots[draggedSiblingIndex].UpdateText();
                    }
                }
            }
            else
            {
                SwapSlot(itemSlots, currentSiblingIndex, draggedSiblingIndex);
            }
        }
        else if (draggedItem.TryGetComponent<EquipmentIcon>(out draggedEquipmentIcon))
        {
            EquipmentSlot parentEquipmentSlot = draggedEquipmentIcon.ParentAfterDrag.GetComponent<EquipmentSlot>();
            if (itemSlots[currentSiblingIndex].HasItem)
            {
                if (itemSlots[currentSiblingIndex].Item is EquipmentItemData eqipmentData &&
                    eqipmentData.EquipmentType == parentEquipmentSlot.EquipmentType)
                {
                    ItemData tempItemData = parentEquipmentSlot.EquipmentItem;
                    parentEquipmentSlot.EquipmentItem = itemSlots[currentSiblingIndex].Item;
                    itemSlots[currentSiblingIndex].Item = tempItemData;
                    itemSlots[currentSiblingIndex].ItemQuantity = 1;

                    itemSlots[currentSiblingIndex].UpdateIcon();
                    parentEquipmentSlot.UpdateIcon();
                }
            }
            else
            {
                itemSlots[currentSiblingIndex].Item = parentEquipmentSlot.EquipmentItem;
                itemSlots[currentSiblingIndex].ItemQuantity = 1;

                itemSlots[currentSiblingIndex].UpdateIcon();
                parentEquipmentSlot.ClearSlot();
            }
        }
    }

    public void UpdateIcon()
    {
        if (HasItem)
        {
            IconImage.color = new Color(255f, 255f, 255f, 1f);
            IconImage.sprite = this.Item.Icon;
            IconImage.raycastTarget = true;
        }
        else
        {
            IconImage.color = new Color(255f, 255f, 255f, 0f);
            IconImage.sprite = null;
            IconImage.raycastTarget = false;
        }
    }

    public void UpdateText()
    {
        if (HasItem && this.Item is CountableItemData)
        {
            QuantityText.gameObject.SetActive(true);
            QuantityText.text = ItemQuantity.ToString();
        }
        else
        {
            QuantityText.gameObject.SetActive(false);
        }
    }

    public void SetupItem(ItemData item, uint quantity = 1)
    {
        Item = item;
        IconImage.sprite = item.Icon;

        if (item is CountableItemData)
        {
            ItemQuantity = quantity;
            UpdateText();
        }
        else
        {
            ItemQuantity = 1;
        }

        UpdateIcon();
    }

    public void SetSlotCount(ItemData item, uint quantity)
    {
        if (item is CountableItemData ciData)
        {
            if (quantity < ciData.MaxQuantity)
            {
                ItemQuantity += quantity;
            }
            else
            {
                ItemQuantity = quantity;
            }
            
            UpdateText();
        }

        if (ItemQuantity <= 0)
        {
            UpdateIcon();
            UpdateText();
        }
    }

    public void ClearSlot()
    {
        Item = null;
        ItemQuantity = 0;
        UIManager.Instance.ItemToolTipPanel.HideToolTip();
        UpdateIcon();
        UpdateText();
    }

    private void SwapSlot(ItemSlot[] itemslots, int currentSiblingIndex, int draggedSiblingIndex)
    {
        if (itemslots[currentSiblingIndex].HasItem)
        {
            ItemData tempItem = itemslots[draggedSiblingIndex].Item;
            uint tempItemCount = itemslots[draggedSiblingIndex].ItemQuantity;

            itemslots[draggedSiblingIndex].Item = itemslots[currentSiblingIndex].Item;
            itemslots[draggedSiblingIndex].ItemQuantity = itemslots[currentSiblingIndex].ItemQuantity;

            itemslots[currentSiblingIndex].Item = tempItem;
            itemslots[currentSiblingIndex].ItemQuantity = tempItemCount;
        }
        else
        {
            itemslots[currentSiblingIndex].Item = itemslots[draggedSiblingIndex].Item;
            itemslots[currentSiblingIndex].ItemQuantity = itemslots[draggedSiblingIndex].ItemQuantity;

            itemslots[draggedSiblingIndex].Item = null;
            itemslots[draggedSiblingIndex].ItemQuantity = 0;
        }

        itemslots[currentSiblingIndex].onChangedItemQuantity?.Invoke(this, itemslots[currentSiblingIndex].ItemQuantity);
        itemslots[draggedSiblingIndex].onChangedItemQuantity?.Invoke(this, itemslots[draggedSiblingIndex].ItemQuantity);

        itemslots[currentSiblingIndex].UpdateIcon();
        itemslots[currentSiblingIndex].UpdateText();

        itemslots[draggedSiblingIndex].UpdateIcon();
        itemslots[draggedSiblingIndex].UpdateText();
    }
}
