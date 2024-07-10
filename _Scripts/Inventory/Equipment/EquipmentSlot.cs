using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File     : EqipmentSlot.cs
 * Desc     : Equipment의 Slot들
 * Date     : 2024-06-17
 * Writer   : 정지훈
 */

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private readonly string _helmetIconPath = "UI/Equipment/Helmet";
    private readonly string _topIconPath = "UI/Equipment/Top";
    private readonly string _bottomIconPath = "UI/Equipment/Bottom";
    private readonly string _shoesIconPath = "UI/Equipment/Shoes";
    private readonly string _weaponIconPath = "UI/Equipment/Weapon";
    private readonly string _gauntletsIconPath = "UI/Equipment/Gauntlets";
    private readonly string _cloakIconPath = "UI/Equipment/Cloak";

    public delegate void ChangedEquipmentHandler(EquipmentSlot equipmentSlot, ItemData currentEquipment, ItemData prevEquipment);

    private ItemData _equipmentItem;
    private RectTransform _rectTransform;

    public Image IconImage;
    public EnumTypes.EquipmentType EquipmentType;

    public event ChangedEquipmentHandler onChangedEquipment;

    public ItemData EquipmentItem
    {
        get { return _equipmentItem; }
        set
        {
            ItemData prevEquipment = _equipmentItem;
            _equipmentItem = value;
            if (_equipmentItem != prevEquipment)
            {
                onChangedEquipment?.Invoke(this, _equipmentItem, prevEquipment);
            }
        }
    }

    public bool IsEquipped
    {
        get
        {
            return EquipmentItem != null;
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        onChangedEquipment += (equipmentSlot, currentEquipment, prevEquipment) =>
        {
            int findIndex;
            if (currentEquipment == null)
            {
                if (prevEquipment != null)
                {
                    EquipmentItemData prevEquipmentData = prevEquipment as EquipmentItemData;
                    int equipmentIndex;

                    for (equipmentIndex = 0; equipmentIndex < DataManager.Instance.Equipment.EquipmentSlots.Length; ++equipmentIndex)
                    {
                        if (DataManager.Instance.Equipment.EquipmentSlots[equipmentIndex].EquipmentType == prevEquipmentData.EquipmentType)
                        {
                            break;
                        }
                    }

                    if (prevEquipmentData is ArmorItemData armorData)
                    {
                        DataManager.Instance.PlayerStatus.Defense -= armorData.Defense;
                        DataManager.Instance.PlayerStatus.MaxHp -= armorData.Hp;
                        DataManager.Instance.PlayerStatus.MaxMp -= armorData.Mp;
                        switch (prevEquipmentData.EquipmentType)
                        {
                            case EnumTypes.EquipmentType.Helmet:
                                findIndex = DataManager.Instance.Equipment.HelmetParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.HelmetParts[findIndex].gameObject.SetActive(false);
                                }
                                break;
                            case EnumTypes.EquipmentType.Top:
                                findIndex = DataManager.Instance.Equipment.TopParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.TopParts[0].gameObject.SetActive(true); // 기본 몸
                                    DataManager.Instance.Equipment.TopParts[findIndex].gameObject.SetActive(false);
                                }
                                break;
                            case EnumTypes.EquipmentType.Bottom:
                                findIndex = DataManager.Instance.Equipment.BottomParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.BottomParts[0].gameObject.SetActive(true); // 기본 다리
                                    DataManager.Instance.Equipment.BottomParts[findIndex].gameObject.SetActive(false);
                                }
                                break;
                            case EnumTypes.EquipmentType.Shoes:
                                findIndex = DataManager.Instance.Equipment.ShoeParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.ShoeParts[0].gameObject.SetActive(true); // 기본 손
                                    DataManager.Instance.Equipment.ShoeParts[findIndex].gameObject.SetActive(false);
                                }
                                break;
                            case EnumTypes.EquipmentType.Gauntlets:
                                findIndex = DataManager.Instance.Equipment.GauntletParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.GauntletParts[0].gameObject.SetActive(true); // 기본 손
                                    DataManager.Instance.Equipment.GauntletParts[findIndex].gameObject.SetActive(false);
                                }
                                break;
                            case EnumTypes.EquipmentType.Cloak:
                                findIndex = DataManager.Instance.Equipment.CloakParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.CloakParts[findIndex].gameObject.SetActive(false);
                                }
                                break;
                        }
                    }
                    else if (prevEquipmentData is WeaponItemData weaponData)
                    {
                        DataManager.Instance.PlayerStatus.AttackDamage -= weaponData.Damage;
                        DataManager.Instance.PlayerStatus.CriticalDamage -= weaponData.Damage;
                        findIndex = DataManager.Instance.Equipment.WeaponParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                        if (findIndex >= 0)
                        {
                            DataManager.Instance.Equipment.WeaponParts[findIndex].gameObject.SetActive(false);
                        }
                    }

                    DataManager.Instance.SaveEquipmentDatas[equipmentIndex].ItemName = null;
                    DataManager.Instance.SaveEquipmentDatas[equipmentIndex].EquipmentType = EnumTypes.EquipmentType.Default;
                    DataManager.Instance.SaveEquipmentData();
                }
            }
            else
            {
                EquipmentItemData currentEquipmentData = currentEquipment as EquipmentItemData;
                EquipmentItemData prevEquipmentData = prevEquipment as EquipmentItemData;

                int equipmentIndex;
                for (equipmentIndex = 0; equipmentIndex < DataManager.Instance.Equipment.EquipmentSlots.Length; ++equipmentIndex)
                {
                    if (DataManager.Instance.Equipment.EquipmentSlots[equipmentIndex].EquipmentType == currentEquipmentData.EquipmentType)
                    {
                        break;
                    }
                }

                if (prevEquipment == null)
                {
                    if (currentEquipmentData is ArmorItemData armorData)
                    {
                        DataManager.Instance.PlayerStatus.Defense += armorData.Defense;
                        DataManager.Instance.PlayerStatus.MaxHp += armorData.Hp;
                        DataManager.Instance.PlayerStatus.MaxMp += armorData.Mp;

                        switch (currentEquipmentData.EquipmentType)
                        {
                            case EnumTypes.EquipmentType.Helmet:
                                findIndex = DataManager.Instance.Equipment.HelmetParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.HelmetParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Top:
                                findIndex = DataManager.Instance.Equipment.TopParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.TopParts[0].gameObject.SetActive(false); // 기본 몸
                                    DataManager.Instance.Equipment.TopParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Bottom:
                                findIndex = DataManager.Instance.Equipment.BottomParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.BottomParts[0].gameObject.SetActive(false); ; // 기본 다리
                                    DataManager.Instance.Equipment.BottomParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Shoes:
                                findIndex = DataManager.Instance.Equipment.ShoeParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.ShoeParts[0].gameObject.SetActive(false); // 기본 손
                                    DataManager.Instance.Equipment.ShoeParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Gauntlets:
                                findIndex = DataManager.Instance.Equipment.GauntletParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.GauntletParts[0].gameObject.SetActive(false); // 기본 손
                                    DataManager.Instance.Equipment.GauntletParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Cloak:
                                findIndex = DataManager.Instance.Equipment.CloakParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.CloakParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                        }
                    }
                    else if (currentEquipmentData is WeaponItemData weaponData)
                    {
                        DataManager.Instance.PlayerStatus.AttackDamage += weaponData.Damage;
                        DataManager.Instance.PlayerStatus.CriticalDamage += weaponData.Damage;
                        findIndex = DataManager.Instance.Equipment.WeaponParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                        DataManager.Instance.Equipment.WeaponParts[findIndex].gameObject.SetActive(true);
                    }

                    DataManager.Instance.SaveEquipmentDatas[equipmentIndex].ItemName = currentEquipment.name;
                    DataManager.Instance.SaveEquipmentDatas[equipmentIndex].EquipmentType = currentEquipmentData.EquipmentType;
                    DataManager.Instance.SaveEquipmentData();
                }
                else
                {
                    if (currentEquipmentData is ArmorItemData armorData)
                    {
                        ArmorItemData prevArmorData = prevEquipment as ArmorItemData;
                        DataManager.Instance.PlayerStatus.Defense -= prevArmorData.Defense;
                        DataManager.Instance.PlayerStatus.MaxHp -= armorData.Hp;
                        DataManager.Instance.PlayerStatus.MaxMp -= armorData.Mp;

                        DataManager.Instance.PlayerStatus.Defense += armorData.Defense;
                        DataManager.Instance.PlayerStatus.MaxHp += armorData.Hp;
                        DataManager.Instance.PlayerStatus.MaxMp += armorData.Mp;

                        switch (currentEquipmentData.EquipmentType)
                        {
                            case EnumTypes.EquipmentType.Helmet:
                                findIndex = DataManager.Instance.Equipment.HelmetParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.HelmetParts[findIndex].gameObject.SetActive(false);
                                }

                                findIndex = DataManager.Instance.Equipment.HelmetParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.HelmetParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Top:
                                findIndex = DataManager.Instance.Equipment.TopParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.TopParts[findIndex].gameObject.SetActive(false);
                                }

                                findIndex = DataManager.Instance.Equipment.TopParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.TopParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Bottom:
                                findIndex = DataManager.Instance.Equipment.BottomParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.BottomParts[findIndex].gameObject.SetActive(false);
                                }

                                findIndex = DataManager.Instance.Equipment.BottomParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.BottomParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Shoes:
                                findIndex = DataManager.Instance.Equipment.ShoeParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.ShoeParts[findIndex].gameObject.SetActive(false);
                                }

                                findIndex = DataManager.Instance.Equipment.ShoeParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.ShoeParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Gauntlets:
                                findIndex = DataManager.Instance.Equipment.GauntletParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.GauntletParts[findIndex].gameObject.SetActive(false);
                                }

                                findIndex = DataManager.Instance.Equipment.GauntletParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.GauntletParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                            case EnumTypes.EquipmentType.Cloak:
                                findIndex = DataManager.Instance.Equipment.CloakParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.CloakParts[findIndex].gameObject.SetActive(false);
                                }

                                findIndex = DataManager.Instance.Equipment.CloakParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                                if (findIndex >= 0)
                                {
                                    DataManager.Instance.Equipment.CloakParts[findIndex].gameObject.SetActive(true);
                                }
                                break;
                        }
                    }
                    else if (currentEquipmentData is WeaponItemData weaponData)
                    {
                        WeaponItemData prevWeaponData = prevEquipment as WeaponItemData;
                        DataManager.Instance.PlayerStatus.AttackDamage -= prevWeaponData.Damage;
                        DataManager.Instance.PlayerStatus.CriticalDamage -= prevWeaponData.Damage;
                        DataManager.Instance.PlayerStatus.AttackDamage += weaponData.Damage;
                        DataManager.Instance.PlayerStatus.CriticalDamage += weaponData.Damage;

                        findIndex = DataManager.Instance.Equipment.WeaponParts.FindIndex(x => x.PartsId == prevEquipmentData.Id);
                        DataManager.Instance.Equipment.WeaponParts[findIndex].gameObject.SetActive(false);
                        findIndex = DataManager.Instance.Equipment.WeaponParts.FindIndex(x => x.PartsId == currentEquipmentData.Id);
                        DataManager.Instance.Equipment.WeaponParts[findIndex].gameObject.SetActive(true);
                    }

                    DataManager.Instance.SaveEquipmentDatas[equipmentIndex].ItemName = currentEquipment.name;
                    DataManager.Instance.SaveEquipmentDatas[equipmentIndex].EquipmentType = currentEquipmentData.EquipmentType;
                    DataManager.Instance.SaveEquipmentData();
                }
            }
        };
    }

    private void Start()
    {
        #region Initial
        UpdateIcon();
        #endregion
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsEquipped)
        {
            UIManager.Instance.ItemToolTipPanel.MyRectTransform.position = new Vector2(eventData.position.x + _rectTransform.rect.width, eventData.position.y - UIManager.Instance.ItemToolTipPanel.MyRectTransform.rect.height);
            UIManager.Instance.ItemToolTipPanel.UpdateToolTip(EquipmentItem);
            UIManager.Instance.ItemToolTipPanel.ShowToolTip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.ItemToolTipPanel.HideToolTip();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedItem = eventData.pointerDrag;

        ItemIcon draggedItemIconUI;
        ItemSlot parentItemSlot;

        if (draggedItem.TryGetComponent<ItemIcon>(out draggedItemIconUI))
        {
            if (draggedItemIconUI.ParentAfterDrag.TryGetComponent<ItemSlot>(out parentItemSlot))
            {
                if (parentItemSlot.Item is EquipmentItemData equipmentData)
                {
                    if (EquipmentType == equipmentData.EquipmentType)
                    {
                        if (IsEquipped)
                        {
                            ItemData tempItemData = EquipmentItem;
                            EquipmentItem = parentItemSlot.Item;
                            parentItemSlot.Item = tempItemData;

                            UpdateIcon();
                            parentItemSlot.UpdateIcon();
                        }
                        else
                        {
                            EquipmentItem = parentItemSlot.Item;
                            UpdateIcon();
                            parentItemSlot.ClearSlot();
                        }
                    }
                }
            }
        }
    }

    public void UpdateIcon()
    {
        if (IsEquipped)
        {
            IconImage.sprite = this.EquipmentItem.Icon;
            IconImage.raycastTarget = true;
        }
        else
        {
            switch (EquipmentType)
            {
                case EnumTypes.EquipmentType.Helmet:
                    IconImage.sprite = Resources.Load<Sprite>(_helmetIconPath);
                    break;
                case EnumTypes.EquipmentType.Top:
                    IconImage.sprite = Resources.Load<Sprite>(_topIconPath);
                    break;
                case EnumTypes.EquipmentType.Bottom:
                    IconImage.sprite = Resources.Load<Sprite>(_bottomIconPath);
                    break;
                case EnumTypes.EquipmentType.Shoes:
                    IconImage.sprite = Resources.Load<Sprite>(_shoesIconPath);
                    break;
                case EnumTypes.EquipmentType.Weapon:
                    IconImage.sprite = Resources.Load<Sprite>(_weaponIconPath);
                    break;
                case EnumTypes.EquipmentType.Gauntlets:
                    IconImage.sprite = Resources.Load<Sprite>(_gauntletsIconPath);
                    break;
                case EnumTypes.EquipmentType.Cloak:
                    IconImage.sprite = Resources.Load<Sprite>(_cloakIconPath);
                    break;
            }

            IconImage.raycastTarget = false;
        }
    }

    public void ClearSlot()
    {
        EquipmentItem = null;
        UIManager.Instance.ItemToolTipPanel.HideToolTip();
        UpdateIcon();
    }
}
