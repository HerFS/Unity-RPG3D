using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/*
 * File     : ShopItemSlot.cs
 * Desc     : ���� ���� ����
 * Date     : 2024-06-16
 * Writer   : ������
 */

public class ShopItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private ShopPopupUI _shopPopupUI;

    //[HideInInspector]
    public ItemData ItemData;

    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private TextMeshProUGUI _itemNameText;
    [SerializeField]
    private TextMeshProUGUI _itemDescriptionText;
    [SerializeField]
    private TextMeshProUGUI _itemPriceText;


    public void ShopSlotSetup()
    {
        if (ItemData != null)
        {
            _iconImage.sprite = ItemData.Icon;
            _itemNameText.text = ItemData.Name;
            _itemDescriptionText.text = ItemData.Description;
            _itemPriceText.text = $"{ItemData.ItemPrice} ��";
        }
        else
        {
            _iconImage.sprite = null;
            _itemNameText.text = "�غ���..";
            _itemDescriptionText.text = "";
            _itemPriceText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ItemData != null)
        {
            _shopPopupUI.SelectShopItemSlot = this;
            _shopPopupUI.InputField.text = 1.ToString();
            _shopPopupUI.ShowUI();

            if (ItemData is CountableItemData)
            {
                _shopPopupUI.InputPanel.SetActive(true);
            }
            else
            {
                _shopPopupUI.InputPanel.SetActive(false);
            }
        }
    }
}