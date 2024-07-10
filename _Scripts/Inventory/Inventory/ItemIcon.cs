using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/*
 * File     : ItemIcon.cs
 * Desc     : Inventory에서 Item의 Icon을 관리
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private EquipmentSlot[] _equipmentSlots;
    private ItemSlot _parentItemSlot;

    private Texture2D _handCursor;
    private Texture2D _originalCursor;

    public Transform ParentAfterDrag;

    public Image ItemImage;
    public TextMeshProUGUI QuantiyText;

    private uint _itemSlotIndex;

    private void Awake()
    {
        #region InitValues
        ItemImage = GetComponent<Image>();
        QuantiyText = GetComponentInChildren<TextMeshProUGUI>();
        _equipmentSlots = FindObjectsOfType<EquipmentSlot>();

        ParentAfterDrag = this.transform.parent;
        #endregion

        #region Cursor
        _handCursor = Resources.Load<Texture2D>(Globals.CursorPath.Hand);
        _originalCursor = Resources.Load<Texture2D>(Globals.CursorPath.Original);
        #endregion
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (UIManager.Instance.ShopPanel.activeSelf && Input.GetMouseButtonDown((int)EnumTypes.MouseButton.Right))
        {
            ItemData selectItem = ParentAfterDrag.GetComponent<ItemSlot>().Item;
            _itemSlotIndex = uint.Parse(Regex.Replace(ParentAfterDrag.name, @"[^0-9]", ""));

            if (selectItem.CanSellable)
            {
                if (selectItem is CountableItemData)
                {
                    UIManager.Instance.ItemPopupPanel.SellInputPanel.SetActive(true);
                }
                else
                {
                    UIManager.Instance.ItemPopupPanel.SellInputPanel.SetActive(false);
                }

                UIManager.Instance.ItemPopupPanel.ShowSellUI();
                UIManager.Instance.ItemPopupPanel.SellOkBtn.gameObject.SetActive(true);
                UIManager.Instance.ItemPopupPanel.SellItemDescription.text = "아이템을 판매하시겠습니까?";

                UIManager.Instance.ItemPopupPanel.SellInputField.text = 1.ToString();
                UIManager.Instance.ItemPopupPanel.ItemSlotIndex = _itemSlotIndex;
            }
            else
            {
                UIManager.Instance.ItemPopupPanel.ShowSellUI();
                UIManager.Instance.ItemPopupPanel.SellInputPanel.SetActive(false);
                UIManager.Instance.ItemPopupPanel.SellOkBtn.gameObject.SetActive(false);
                UIManager.Instance.ItemPopupPanel.SellItemDescription.text = "판매할 수 없는 아이템 입니다.";
            }
        }
        else
        {
            if (Input.GetMouseButtonDown((int)EnumTypes.MouseButton.Right))
            {
                _parentItemSlot = ParentAfterDrag.GetComponent<ItemSlot>();
                if (_parentItemSlot.Item is ConsumptionItemData consumptionData)
                {
                    switch (consumptionData.ConsumptionType)
                    {
                        case ConsumptionType.Hp:
                            if (!(DataManager.Instance.PlayerStatus.CurrentHp == DataManager.Instance.PlayerStatus.MaxHp))
                            {
                                DataManager.Instance.PlayerStatus.CurrentHp += UsedItem(consumptionData.Value, _parentItemSlot);
                            }
                            break;
                        case ConsumptionType.Mp:
                            if (!(DataManager.Instance.PlayerStatus.CurrentMp == DataManager.Instance.PlayerStatus.MaxMp))
                            {
                                DataManager.Instance.PlayerStatus.CurrentMp += UsedItem(consumptionData.Value, _parentItemSlot);
                            }
                            break;
                        case ConsumptionType.Exp:
                            DataManager.Instance.PlayerStatus.CurrentExp += UsedItem(consumptionData.Value, _parentItemSlot);
                            break;
                    }
                }

                if (_parentItemSlot.Item is EquipmentItemData equipmentData)
                {
                    for (int i = 0; i < _equipmentSlots.Length; ++i)
                    {
                        // equipment slot에 같은 아이템이 장착되어있는지 확인 하고 장착
                        if (_equipmentSlots[i].IsEquipped)
                        {
                            if (_equipmentSlots[i].EquipmentType == equipmentData.EquipmentType)
                            {
                                ItemData tempItemData = _equipmentSlots[i].EquipmentItem;
                                _equipmentSlots[i].EquipmentItem = _parentItemSlot.Item;
                                _parentItemSlot.Item = tempItemData;

                                _equipmentSlots[i].UpdateIcon();
                                _parentItemSlot.UpdateIcon();
                                UIManager.Instance.ItemToolTipPanel.UpdateToolTip(tempItemData);
                            }
                        }
                        else
                        {
                            if (_equipmentSlots[i].EquipmentType == equipmentData.EquipmentType)
                            {
                                _equipmentSlots[i].EquipmentItem = _parentItemSlot.Item;
                                _equipmentSlots[i].UpdateIcon();
                                _parentItemSlot.ClearSlot();
                            }
                        }
                    }
                }
            }
            else
            {
                Cursor.SetCursor(_handCursor, new Vector2((_handCursor.width / 3f), 0f), CursorMode.Auto);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.SetCursor(_originalCursor, new Vector2(0f, 0f), CursorMode.Auto);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton((int)EnumTypes.MouseButton.Left))
        {
            this.transform.position = Input.mousePosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ParentAfterDrag = this.transform.parent;
        this.transform.SetParent(transform.root);
        this.transform.SetAsLastSibling();
        ItemImage.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsOverUI();

        if (ParentAfterDrag.GetComponent<ItemSlot>().HasItem)
        {
            ItemImage.raycastTarget = true;
        }

        this.transform.SetParent(ParentAfterDrag);
    }

    private void IsOverUI()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _parentItemSlot = ParentAfterDrag.GetComponent<ItemSlot>();
            _itemSlotIndex = uint.Parse(Regex.Replace(ParentAfterDrag.name, @"[^0-9]", ""));

            if (_parentItemSlot.Item is CountableItemData)
            {
                // 개수 나눠서 버리기
                if (InputManager.Instance.IsLeftShift)
                {
                    UIManager.Instance.ItemPopupPanel.SetThrowItemName(_parentItemSlot.Item.Name);
                    UIManager.Instance.ItemPopupPanel.ShowThrowUI(_parentItemSlot.Item);
                    UIManager.Instance.ItemPopupPanel.ItemSlotIndex = _itemSlotIndex;
                    UIManager.Instance.ItemPopupPanel.ThrowInputField.characterLimit = 2;
                }
                else
                {
                    // 그냥버리기
                    UIManager.Instance.ItemPopupPanel.SetThrowItemName(_parentItemSlot.Item.Name + " x " + _parentItemSlot.ItemQuantity);
                    UIManager.Instance.ItemPopupPanel.ShowThrowUI(_parentItemSlot.Item);
                    UIManager.Instance.ItemPopupPanel.ItemSlotIndex = _itemSlotIndex;
                }
            }
            else
            {
                // 그냥버리기
                UIManager.Instance.ItemPopupPanel.SetThrowItemName(_parentItemSlot.Item.Name);
                UIManager.Instance.ItemPopupPanel.ShowThrowUI(_parentItemSlot.Item);
                UIManager.Instance.ItemPopupPanel.ItemSlotIndex = _itemSlotIndex;
            }
        }
    }

    private float UsedItem(float value, ItemSlot parentItemSlot)
    {
        parentItemSlot.ItemQuantity -= 1;
        parentItemSlot.UpdateText();

        return value;
    }
}
