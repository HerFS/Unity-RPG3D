using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : ItemPopupUI.cs
 * Desc     : Item을 Inventory에서 버리거나 옮길 때 활성화 되는 Popup UI
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class ItemPopupUI : MonoBehaviour
{
    private readonly string _bronzeMoneyPath = "Money/Bronze_Coin";
    private readonly string _silverMoneyPath = "Money/Silver_Coin";
    private readonly string _goldMoneyPath = "Money/Gold_Coin";
    private readonly string _moneyName = "Money";
    private readonly float _spawnHeight = 0.1f;

    [SerializeField]
    private RectTransform _myRectTransform;
    [SerializeField]
    private RectTransform _followRectTransform;
    [SerializeField]
    private GameObject _background;

    private ItemIcon[] _itemIcons;
    private Vector3 _itemSpawnPosition;

    [Header("Money")]
    [SerializeField]
    private Button _moneyBtn;

    [Header("Throw Panel")]
    [SerializeField]
    private RectTransform _throwPanel;
    [SerializeField]
    private TextMeshProUGUI _throwItemName;
    public Button ThrowOkBtn;
    public Button ThrowCancelBtn;

    [Header("Throw Input Panel")]
    [SerializeField]
    private GameObject _throwInputPanel;
    [SerializeField]
    private Button _throwPlusBtn;
    [SerializeField]
    private Button _throwMinusBtn;
    public InputField ThrowInputField;

    [Header("Quantity Panel")]
    [SerializeField]
    private GameObject _quantityPanel;
    [SerializeField]
    private TextMeshProUGUI _quantityItemName;
    [SerializeField]
    private Button _quantityPlusBtn;
    [SerializeField]
    private Button _quantityMinusBtn;
    public Button QuantityOkBtn;
    public Button QuantityCancelBtn;
    public InputField QuantityInputField;

    [Header("Sell Panel")]
    [SerializeField]
    private GameObject _sellPanel;
    [SerializeField]
    public GameObject SellInputPanel;
    [SerializeField]
    private TextMeshProUGUI _sellItemName;
    [SerializeField]
    public TextMeshProUGUI SellItemDescription;
    [SerializeField]
    private Button _sellPlusBtn;
    [SerializeField]
    private Button _sellMinusBtn;
    public Button SellOkBtn;
    public Button SellCancelBtn;
    public InputField SellInputField;

    [HideInInspector]
    public uint ItemSlotIndex;

    private void Awake()
    {
        if (_myRectTransform is null)
        {
            _myRectTransform = GetComponent<RectTransform>();
        }

        _itemIcons = FindObjectsOfType<ItemIcon>();

        Array.Sort(_itemIcons, (a, b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });
    }

    private void Start()
    {
        #region Button Events

        _sellPlusBtn.onClick.AddListener(() =>
        {
            ItemSlot[] itemSlots = DataManager.Instance.Inventory.ItemSlots;

            uint.TryParse(SellInputField.text, out uint quantity);

            if (itemSlots[ItemSlotIndex] != null && itemSlots[ItemSlotIndex].Item is CountableItemData countableData)
            {
                if (quantity < countableData.MaxQuantity)
                {
                    uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity + 10 : quantity + 1;

                    if (nextQuantity > countableData.MaxQuantity)
                    {
                        nextQuantity = countableData.MaxQuantity;
                    }

                    SellInputField.text = nextQuantity.ToString();
                }
            }
        });

        _sellMinusBtn.onClick.AddListener(() =>
        {
            uint.TryParse(SellInputField.text, out uint quantity);

            if (quantity > 1)
            {
                uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity - 10 : quantity - 1;

                if (nextQuantity < 1)
                {
                    nextQuantity = 1;
                }

                SellInputField.text = nextQuantity.ToString();
            }
        });

        SellOkBtn.onClick.AddListener(() =>
        {
            ItemSlot[] itemSlots = DataManager.Instance.Inventory.ItemSlots;

            uint.TryParse(SellInputField.text, out uint quantity);

            uint sellItemCount = itemSlots[ItemSlotIndex].ItemQuantity - quantity;

            DataManager.Instance.PlayerStatus.Money += (uint)itemSlots[ItemSlotIndex].Item.SalePrice * quantity;

            if (sellItemCount > 0 && quantity > 0)
            {
                itemSlots[ItemSlotIndex].ItemQuantity -= quantity;
            }
            else
            {
                itemSlots[ItemSlotIndex].ClearSlot();
            }

            HideSellUI();
        });

        SellCancelBtn.onClick.AddListener(() =>
        {
            HideSellUI();
        });

        _moneyBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ItemPopupPanel.SetThrowItemName(_moneyName);
            UIManager.Instance.ItemPopupPanel.ShowThrowUI();
            ThrowInputField.characterLimit = 10;
        });

        ThrowOkBtn.onClick.AddListener(() =>
        {
            ItemSlot[] itemSlots = DataManager.Instance.Inventory.ItemSlots;

            Vector3 playerPosition = GameManager.Instance.Player.transform.position;
            _itemSpawnPosition = new Vector3(playerPosition.x, (playerPosition.y + _spawnHeight), playerPosition.z);

            if (_throwItemName.text == _moneyName)
            {
                uint.TryParse(ThrowInputField.text, out uint throwMoney);
                PickupItem newItem;

                if (DataManager.Instance.PlayerStatus.Money > 0 && throwMoney > 0)
                {
                    if (throwMoney >= DataManager.Instance.PlayerStatus.Money)
                    {
                        throwMoney = DataManager.Instance.PlayerStatus.Money;
                        DataManager.Instance.PlayerStatus.Money = 0;
                    }
                    else
                    {
                        DataManager.Instance.PlayerStatus.Money -= throwMoney;
                    }

                    if (throwMoney <= 100)
                    {
                        newItem = Resources.Load<PickupItem>(_bronzeMoneyPath);
                    }
                    else if (throwMoney <= 1000)
                    {
                        newItem = Resources.Load<PickupItem>(_silverMoneyPath);
                    }
                    else
                    {
                        newItem = Resources.Load<PickupItem>(_goldMoneyPath);
                    }

                    GameObject newObj = newItem.Item.DropItemPrefab;

                    DefaultItemData defaultData = newItem.Item as DefaultItemData;

                    newObj.GetComponent<PickupItem>().Item = newItem.Item;
                    newObj.GetComponent<PickupItem>().ItemQuantity = 1;
                    newObj.GetComponent<PickupItem>().MoneyValue = throwMoney;
                    Instantiate(newObj, _itemSpawnPosition, Quaternion.identity);

                    InitalInputField();
                    HideThrowUI();

                    return;
                }
            }
            else
            {
                ItemSlot parentSlot = itemSlots[ItemSlotIndex];
                GameObject newObj = parentSlot.Item.DropItemPrefab;
                newObj.GetComponent<PickupItem>().Item = parentSlot.Item;

                if (_throwInputPanel.activeSelf)
                {
                    uint.TryParse(ThrowInputField.text, out uint throwQuantity);
                    uint setItemQuantity = parentSlot.ItemQuantity - throwQuantity;
                    if (throwQuantity > 0)
                    {
                        if (setItemQuantity <= 0)
                        {
                            newObj.GetComponent<PickupItem>().Item = parentSlot.Item;
                            newObj.GetComponent<PickupItem>().ItemQuantity = parentSlot.ItemQuantity;

                            Instantiate(newObj, _itemSpawnPosition, Quaternion.identity);

                            parentSlot.Item = null;
                            parentSlot.ItemQuantity = 0;
                            parentSlot.UpdateIcon();
                            parentSlot.UpdateText();
                        }
                        else
                        {
                            newObj.GetComponent<PickupItem>().Item = parentSlot.Item;
                            newObj.GetComponent<PickupItem>().ItemQuantity = throwQuantity;

                            parentSlot.ItemQuantity = setItemQuantity;
                            parentSlot.UpdateText();

                            Instantiate(newObj, _itemSpawnPosition, Quaternion.identity);
                        }
                    }
                }
                else
                {
                    if (parentSlot.Item is CountableItemData)
                    {
                        newObj.GetComponent<PickupItem>().ItemQuantity = parentSlot.ItemQuantity;
                    }
                    else
                    {
                        newObj.GetComponent<PickupItem>().ItemQuantity = 1;
                    }

                    Instantiate(newObj, _itemSpawnPosition, Quaternion.identity);

                    parentSlot.Item = null;
                    parentSlot.ItemQuantity = 0;
                    parentSlot.UpdateIcon();
                    parentSlot.UpdateText();
                }
            }

            InitalInputField();
            HideThrowUI();
        });

        ThrowCancelBtn.onClick.AddListener(() =>
        {
            InitalInputField();
            HideThrowUI();
        });

        QuantityCancelBtn.onClick.AddListener(() =>
        {
            InitalInputField();
            HideQuantityUI();
        });

        _throwPlusBtn.onClick.AddListener(() =>
        {
            ItemSlot[] itemSlots = DataManager.Instance.Inventory.ItemSlots;

            uint.TryParse(ThrowInputField.text, out uint quantity);

            if (_throwItemName.text == _moneyName)
            {
                if (quantity < DataManager.Instance.PlayerStatus.Money)
                {
                    uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity + 1000 : quantity + 10;

                    if (nextQuantity > DataManager.Instance.PlayerStatus.Money)
                    {
                        nextQuantity = DataManager.Instance.PlayerStatus.Money;
                    }

                    ThrowInputField.text = nextQuantity.ToString();

                    return;
                }
            }

            if (itemSlots[ItemSlotIndex] != null && itemSlots[ItemSlotIndex].Item is CountableItemData countableData)
            {
                if (quantity < countableData.MaxQuantity)
                {
                    uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity + 10 : quantity + 1;

                    if (nextQuantity > countableData.MaxQuantity)
                    {
                        nextQuantity = countableData.MaxQuantity;
                    }

                    ThrowInputField.text = nextQuantity.ToString();
                }
            }
        });

        _throwMinusBtn.onClick.AddListener(() =>
        {
            uint.TryParse(ThrowInputField.text, out uint quantity);

            if (quantity > 1)
            {
                uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity - 10 : quantity - 1;

                if (nextQuantity < 1)
                {
                    nextQuantity = 1;
                }

                ThrowInputField.text = nextQuantity.ToString();
            }
        });

        _quantityPlusBtn.onClick.AddListener(() =>
        {
            ItemSlot[] itemSlots = DataManager.Instance.Inventory.ItemSlots;

            uint.TryParse(ThrowInputField.text, out uint quantity);

            if (itemSlots[ItemSlotIndex] != null && itemSlots[ItemSlotIndex].Item is CountableItemData countableData)
            {
                if (quantity < countableData.MaxQuantity)
                {
                    uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity + 10 : quantity + 1;

                    if (nextQuantity > countableData.MaxQuantity)
                    {
                        nextQuantity = countableData.MaxQuantity;
                    }

                    QuantityInputField.text = nextQuantity.ToString();
                }
            }
        });

        _quantityMinusBtn.onClick.AddListener(() =>
        {
            uint.TryParse(ThrowInputField.text, out uint quantity);

            if (quantity > 1)
            {
                uint nextQuantity = InputManager.Instance.IsLeftShift ? quantity - 10 : quantity - 1;

                if (nextQuantity < 1)
                {
                    nextQuantity = 1;
                }

                QuantityInputField.text = nextQuantity.ToString();
            }
        });
        #endregion

        HideThrowUI();
        HideSellUI();
        HideQuantityUI();
    }

    private void Update()
    {
        if (UIManager.Instance.InventoryPanel.activeSelf)
        {
            _myRectTransform.position = _followRectTransform.position;
        }
    }

    public void ShowThrowUI(ItemData item = null)
    {
        _background.SetActive(true);

        if (item == null)
        {
            InitalInputField();
            _throwInputPanel.gameObject.SetActive(true);
            _throwPanel.sizeDelta = new Vector2(_throwPanel.sizeDelta.x, 180f);

            _throwPanel.gameObject.SetActive(true);
            return;
        }

        if (InputManager.Instance.IsLeftShift)
        {
            if (item is CountableItemData)
            {
                InitalInputField();
                _throwInputPanel.gameObject.SetActive(true);
                _throwPanel.sizeDelta = new Vector2(_throwPanel.sizeDelta.x, 180f);
            }
            else
            {
                _throwInputPanel.gameObject.SetActive(false);
                _throwPanel.sizeDelta = new Vector2(_throwPanel.sizeDelta.x, 150f);
            }
        }
        else
        {
            _throwInputPanel.gameObject.SetActive(false);
            _throwPanel.sizeDelta = new Vector2(_throwPanel.sizeDelta.x, 150f);
        }

        _throwPanel.gameObject.SetActive(true);
    }

    public void HideThrowUI()
    {
        _background.SetActive(false);
        _throwPanel.gameObject.SetActive(false);
    }
    public void ShowQuantityUI()
    {
        _background.SetActive(true);
        _quantityPanel.gameObject.SetActive(true);
    }
    public void HideQuantityUI()
    {
        _background.SetActive(false);
        _quantityPanel.gameObject.SetActive(false);
    }

    public void ShowSellUI()
    {
        _background.SetActive(true);
        _sellPanel.gameObject.SetActive(true);
    }
    public void HideSellUI()
    {
        _background.SetActive(false);
        _sellPanel.gameObject.SetActive(false);
    }

    public void SetThrowItemName(string name)
    {
        _throwItemName.text = name;
    }

    public void SetQuantityItemName(string name)
    {
        _quantityItemName.text = name;
    }

    public void SetSellItemName(string name)
    {
        _sellItemName.text = name;
    }

    public void InitalInputField()
    {
        ThrowInputField.text = 1.ToString();
        QuantityInputField.text = 1.ToString();
    }
}
