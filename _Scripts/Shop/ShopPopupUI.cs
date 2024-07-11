using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : ShopPopupUI.cs
 * Desc     : »óÁ¡ ÆË¾÷ UI
 * Date     : 2024-06-30
 * Writer   : Á¤ÁöÈÆ
 */

public class ShopPopupUI : MonoBehaviour
{
    private Inventory _inventory;

    [Header("Panel")]
    [SerializeField]
    private RectTransform _throwPanel;
    public GameObject InputPanel;
    [SerializeField]
    private GameObject _background;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI _ItemName;
    public Button OkBtn;
    public Button CancelBtn;
    [SerializeField]
    private Button _plusBtn;
    [SerializeField]
    private Button _minusBtn;
    public InputField InputField;

    [HideInInspector]
    public ShopItemSlot SelectShopItemSlot;

    private void Awake()
    {
        _inventory = FindObjectOfType<Inventory>();

        HideUI();

        #region Button Events
        OkBtn.onClick.AddListener(() =>
        {
            uint quantity = 0;
            uint.TryParse(InputField.text, out quantity);

            if (DataManager.Instance.PlayerStatus.Money >= (SelectShopItemSlot.ItemData.ItemPrice * quantity))
            {
                _inventory.AddItem(SelectShopItemSlot.ItemData, ref quantity);
                DataManager.Instance.PlayerStatus.Money -= (uint)(SelectShopItemSlot.ItemData.ItemPrice * quantity);
            }

            HideUI();
        });

        CancelBtn.onClick.AddListener(() =>
        {
            HideUI();
        });

        _plusBtn.onClick.AddListener(() =>
        {
            uint.TryParse(InputField.text, out uint quantity);

            if (SelectShopItemSlot.ItemData is CountableItemData countableData)
            {
                if (quantity < countableData.MaxQuantity)
                {
                    uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity + 10 : quantity + 1;

                    if (nextQuantity > countableData.MaxQuantity)
                    {
                        nextQuantity = countableData.MaxQuantity;
                    }

                    InputField.text = nextQuantity.ToString();
                }
            }
        });

        _minusBtn.onClick.AddListener(() =>
        {
            uint.TryParse(InputField.text, out uint quantity);

            if (quantity > 1)
            {
                uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity - 10 : quantity - 1;

                if (nextQuantity < 1)
                {
                    nextQuantity = 1;
                }

                InputField.text = nextQuantity.ToString();
            }
        });
        #endregion
    }

    private void OnEnable()
    {
        if (SelectShopItemSlot != null)
        {
            _ItemName.text = SelectShopItemSlot.ItemData.Name;
        }
    }

    private void OnDisable()
    {
        HideUI();
    }

    public void ShowUI()
    {
        this.gameObject.SetActive(true);
        _throwPanel.gameObject.SetActive(true);
        _background.SetActive(true);
    }

    public void HideUI()
    {
        this.gameObject.SetActive(false);
        _throwPanel.gameObject.SetActive(false);
        _background.SetActive(false);
    }
}
