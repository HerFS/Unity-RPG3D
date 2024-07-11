using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * File     : DataManager.cs
 * Desc     : 데이터 관리
 * Date     : 2024-07-10
 * Writer   : 정지훈
 */

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}

[System.Serializable]
public struct PlayerInfo
{
    public string Name;
    public float WalkSpeed;
    public float RunSpeed;
    public float CurrentHp;
    public float MaxHp;
    public float CurrentMp;
    public float MaxMp;
    public float CurrentStamina;
    public float AttackDamage;
    public float CriticalDamage;
    public float CriticalChance;
    public float Defense;
    public float CurrentExp;
    public float RequiredExp;
    public uint Level;
    public uint Money;
    public string CurrentScene;
}

[System.Serializable]
public class ItemSlotInfo
{
    public string ItemName;
    public uint ItemQuantity;
    public EnumTypes.EquipmentType EquipmentType;
}

[System.Serializable]
public class EquipmentSlotInfo
{
    public string ItemName;
    public EnumTypes.EquipmentType EquipmentType;
}

public class DataManager : Singleton<DataManager>
{
    #region ReadOnly
    private readonly string _statusFilePath = Application.dataPath + "/Resources/PlayerData.json";
    private readonly string _itemFilePath = Application.dataPath + "/Resources/PlayerInventory.json";
    private readonly string _equipmentFilePath = Application.dataPath + "/Resources/PlayerEquipment.json";

    private readonly string _helmetItemDataPath = "ItemData/Armor/Helmet/";
    private readonly string _topItemDataPath = "ItemData/Armor/Top/";
    private readonly string _bottomItemDataPath = "ItemData/Armor/Bottom/";
    private readonly string _shoesItemDataPath = "ItemData/Armor/Shoes/";
    private readonly string _gauntletsItemDataPath = "ItemData/Armor/Gauntlets/";
    private readonly string _cloakItemDataPath = "ItemData/Armor/Cloak/";
    private readonly string _weaponItemDataPath = "ItemData/Weapon/";
    private readonly string _consumptionItemDataPath = "ItemData/Consumption/";
    private readonly string _defaultItemDataPath = "ItemData/Default/";
    #endregion

    private float _timer;

    [HideInInspector]
    public PlayerStatus PlayerStatus;
    [HideInInspector]
    public PlayerInfo PlayerData;

    [Header("Inventory")]
    public Inventory Inventory;
    [HideInInspector]
    public ItemSlotInfo[] SaveItemDatas;

    [Header("Equipment")]
    public Equipment Equipment;
    [HideInInspector]
    public EquipmentSlotInfo[] SaveEquipmentDatas;

    protected override void Awake()
    {
        base.Awake();

        PlayerStatus = GetComponent<PlayerStatus>();

        if (Inventory == null)
        {
            Inventory = FindObjectOfType<Inventory>();
        }

        if (Equipment == null)
        {
            Equipment = FindObjectOfType<Equipment>();
        }
    }

    private void Start()
    {
        SaveItemDatas = new ItemSlotInfo[Inventory.Capacity];
        SaveEquipmentDatas = new EquipmentSlotInfo[Globals.EquipmentSlotCount];

        #region Status Save
        PlayerData = new PlayerInfo();

        if (File.Exists(_statusFilePath))
        {
            string saveFile = File.ReadAllText(_statusFilePath);
            PlayerData = JsonUtility.FromJson<PlayerInfo>(saveFile);

            PlayerStatus.Name = PlayerData.Name;
            PlayerStatus.Level = PlayerData.Level;
            PlayerStatus.MaxHp = PlayerData.MaxHp;
            PlayerStatus.RunSpeed = PlayerData.RunSpeed;
            PlayerStatus.CurrentHp = PlayerData.CurrentHp;
            PlayerStatus.MaxMp = PlayerData.MaxMp;
            PlayerStatus.CurrentMp = PlayerData.CurrentMp;
            PlayerStatus.CurrentStamina = PlayerData.CurrentStamina;
            PlayerStatus.AttackDamage = PlayerData.AttackDamage;
            PlayerStatus.CriticalDamage = PlayerData.CriticalDamage;
            PlayerStatus.CriticalChance = PlayerData.CriticalChance;
            PlayerStatus.Defense = PlayerData.Defense;
            PlayerStatus.RequiredExp = PlayerData.RequiredExp;
            PlayerStatus.CurrentExp = PlayerData.CurrentExp;
            PlayerStatus.Money = PlayerData.Money;
            PlayerStatus.CurrentScene = PlayerData.CurrentScene;
        }
        #endregion

        #region SetupUIStatus
        UIManager.Instance.PlayerInfoPanel.NameText.text = PlayerStatus.Name;
        UIManager.Instance.PlayerInfoPanel.LevelText.text = PlayerStatus.Level.ToString();
        UIManager.Instance.PlayerInfoPanel.HpText.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.CurrentHp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", PlayerStatus.MaxHp.ToString("F1"));
        UIManager.Instance.PlayerInfoPanel.MpText.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.CurrentMp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", PlayerStatus.MaxMp.ToString("F1"));
        UIManager.Instance.PlayerInfoPanel.ExpText.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.CurrentExp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", PlayerStatus.RequiredExp.ToString("F1")) + " (" + (100 * (PlayerStatus.CurrentExp / PlayerStatus.RequiredExp)).ToString("N1") + "%)";

        UIManager.Instance.PlayerInfoPanel.HpSlider.maxValue = PlayerStatus.MaxHp;
        UIManager.Instance.PlayerInfoPanel.MpSlider.maxValue = PlayerStatus.MaxMp;
        UIManager.Instance.PlayerInfoPanel.ExpSlider.maxValue = PlayerStatus.RequiredExp;
        UIManager.Instance.PlayerInfoPanel.StaminaSlider.maxValue = PlayerStatus.MaxStaminaValue;

        UIManager.Instance.PlayerInfoPanel.HpSlider.value = PlayerStatus.CurrentHp;
        UIManager.Instance.PlayerInfoPanel.MpSlider.value = PlayerStatus.CurrentMp;
        UIManager.Instance.PlayerInfoPanel.ExpSlider.value = PlayerStatus.CurrentExp;
        UIManager.Instance.PlayerInfoPanel.StaminaSlider.value = PlayerStatus.CurrentStamina;


        UIManager.Instance.PlayerStatusText.Name.text = PlayerStatus.Name;
        UIManager.Instance.PlayerStatusText.Level.text = PlayerStatus.Level.ToString();
        UIManager.Instance.PlayerStatusText.Exp.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.CurrentExp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", PlayerStatus.RequiredExp.ToString("F1"));
        UIManager.Instance.PlayerStatusText.Hp.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.CurrentHp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", PlayerStatus.MaxHp.ToString("F1"));
        UIManager.Instance.PlayerStatusText.Mp.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.CurrentMp.ToString("F1")) + " / " + string.Format("{0: #,###; -#,###;0}", PlayerStatus.MaxMp.ToString("F1"));
        UIManager.Instance.PlayerStatusText.Attack.text = PlayerStatus.AttackDamage.ToString("F1");
        UIManager.Instance.PlayerStatusText.CriticalChance.text = PlayerStatus.CriticalChance.ToString("F1");
        UIManager.Instance.PlayerStatusText.Defense.text = PlayerStatus.Defense.ToString("F1");
        UIManager.Instance.PlayerStatusText.Money.text = string.Format("{0: #,###; -#,###;0}", PlayerStatus.Money);
        #endregion

        #region Item Save
        if (File.Exists(_itemFilePath))
        {
            string inventoryJsonData = File.ReadAllText(_itemFilePath);
            string itemPath;
            SaveItemDatas = JsonHelper.FromJson<ItemSlotInfo>(inventoryJsonData);

            for (int i = 0; i < Inventory.Capacity; ++i)
            {
                if (SaveItemDatas[i].ItemName.Contains(EnumTypes.ItemType.Armor.ToString()))
                {
                    switch (SaveItemDatas[i].EquipmentType)
                    {
                        case EnumTypes.EquipmentType.Helmet:
                            itemPath = _helmetItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Top:
                            itemPath = _topItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Bottom:
                            itemPath = _bottomItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Shoes:
                            itemPath = _shoesItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Gauntlets:
                            itemPath = _gauntletsItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Cloak:
                            itemPath = _cloakItemDataPath;
                            break;
                        default:
                            itemPath = null;
                            return;
                    }
                }
                else if (SaveItemDatas[i].ItemName.Contains(EnumTypes.ItemType.Weapon.ToString()))
                {
                    itemPath = _weaponItemDataPath;
                }
                else if (SaveItemDatas[i].ItemName.Contains(EnumTypes.ItemType.Consumption.ToString()))
                {
                    itemPath = _consumptionItemDataPath;
                }
                else if (SaveItemDatas[i].ItemName.Contains(EnumTypes.ItemType.Default.ToString()))
                {
                    itemPath = _defaultItemDataPath;
                }
                else
                {
                    continue;
                }

                ItemData saveItemData = Resources.Load<ItemData>(itemPath + SaveItemDatas[i].ItemName);
                Inventory.ItemSlots[i].Item = saveItemData;
                Inventory.ItemSlots[i].ItemQuantity = SaveItemDatas[i].ItemQuantity;

                Inventory.ItemSlots[i].UpdateIcon();
                Inventory.ItemSlots[i].UpdateText();
            }
        }
        else
        {
            SaveItemData();
        }
        #endregion

        #region Equipment Save
        if (File.Exists(_equipmentFilePath))
        {
            string equipmentJsonData = File.ReadAllText(_equipmentFilePath);
            string itemPath;
            SaveEquipmentDatas = JsonHelper.FromJson<EquipmentSlotInfo>(equipmentJsonData);

            for (int saveDataIndex = 0; saveDataIndex < Globals.EquipmentSlotCount; ++saveDataIndex)
            {
                if (SaveEquipmentDatas[saveDataIndex].ItemName.Contains(EnumTypes.ItemType.Armor.ToString()))
                {
                    switch (SaveEquipmentDatas[saveDataIndex].EquipmentType)
                    {
                        case EnumTypes.EquipmentType.Helmet:
                            itemPath = _helmetItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Top:
                            itemPath = _topItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Bottom:
                            itemPath = _bottomItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Shoes:
                            itemPath = _shoesItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Gauntlets:
                            itemPath = _gauntletsItemDataPath;
                            break;
                        case EnumTypes.EquipmentType.Cloak:
                            itemPath = _cloakItemDataPath;
                            break;
                        default:
                            itemPath = null;
                            return;
                    }
                }
                else if (SaveEquipmentDatas[saveDataIndex].ItemName.Contains(EnumTypes.ItemType.Weapon.ToString()))
                {
                    itemPath = _weaponItemDataPath;
                }
                else
                {
                    continue;
                }

                ItemData saveItemData = Resources.Load<ItemData>(itemPath + SaveEquipmentDatas[saveDataIndex].ItemName);

                for (int slotIndex = 0; slotIndex < Globals.EquipmentSlotCount; ++slotIndex)
                {
                    if (SaveEquipmentDatas[saveDataIndex].EquipmentType == Equipment.EquipmentSlots[slotIndex].EquipmentType)
                    {
                        Equipment.EquipmentSlots[slotIndex].EquipmentItem = saveItemData;
                        Equipment.EquipmentSlots[slotIndex].UpdateIcon();
                        break;
                    }
                }
            }
        }
        else
        {
            SaveEquipmentData();
        }
        #endregion
    }

    public void SaveStausData()
    {
        if (PlayerStatus.Name.Length != 0)
        {
            string jsonData = JsonUtility.ToJson(PlayerData, true);
            File.WriteAllText(_statusFilePath, jsonData);
        }
    }

    public void SaveItemData()
    {
        string jsonData = JsonHelper.ToJson(SaveItemDatas, true);
        File.WriteAllText(_itemFilePath, jsonData);
    }

    public void SaveEquipmentData()
    {
        string jsonData = JsonHelper.ToJson(SaveEquipmentDatas, true);
        File.WriteAllText(_equipmentFilePath, jsonData);
    }

    public void CreatePlayerStatusFile(string name)
    {
        PlayerStatus.Name = name;

        PlayerData.Name = PlayerStatus.Name;
        PlayerData.Level = PlayerStatus.Level;
        PlayerData.WalkSpeed = PlayerStatus.WalkSpeed;
        PlayerData.RunSpeed = PlayerStatus.RunSpeed;
        PlayerData.CurrentHp = PlayerStatus.MaxHp;
        PlayerData.MaxHp = PlayerStatus.MaxHp;
        PlayerData.CurrentMp = PlayerStatus.MaxMp;
        PlayerData.MaxMp = PlayerStatus.MaxMp;
        PlayerData.CurrentStamina = PlayerStatus.CurrentStamina;
        PlayerData.AttackDamage = PlayerStatus.AttackDamage;
        PlayerData.CriticalDamage = PlayerStatus.CriticalDamage;
        PlayerData.CriticalChance = PlayerStatus.CriticalChance;
        PlayerData.Defense = PlayerStatus.Defense;
        PlayerData.CurrentExp = PlayerStatus.CurrentExp;
        PlayerData.RequiredExp = PlayerStatus.RequiredExp;
        PlayerData.Money = PlayerStatus.Money;
        PlayerData.CurrentScene = EnumTypes.SceneName.Village.ToString();

        SaveStausData();
    }

    private void OnApplicationQuit()
    {
        for (int i = 0; i < Globals.EquipmentSlotCount; ++i)
        {
            if (Equipment.EquipmentSlots[i].EquipmentItem is ArmorItemData armorData)
            {
                PlayerStatus.MaxHp -= armorData.Hp;
                PlayerStatus.MaxMp -= armorData.Mp;
                PlayerStatus.Defense -= armorData.Defense;
            }
            else if (Equipment.EquipmentSlots[i].EquipmentItem is WeaponItemData weaponData)
            {
                PlayerStatus.AttackDamage -= weaponData.Damage;
                PlayerStatus.CriticalDamage -= weaponData.Damage;
            }
        }
    }
}