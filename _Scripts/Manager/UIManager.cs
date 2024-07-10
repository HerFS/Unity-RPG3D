using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * File     : UIManager.cs
 * Desc     : UI °ü¸®
 * Date     : 2024-06-30
 * Writer   : Á¤ÁöÈÆ
 */

[System.Serializable]
public class PlayerStatusText
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Exp;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI Mp;
    public TextMeshProUGUI Attack;
    public TextMeshProUGUI CriticalChance;
    public TextMeshProUGUI Defense;
    public TextMeshProUGUI Money;
}

public class UIManager : Singleton<UIManager>
{
    public GameObject BloodUI;

    [Header("Inventory UI")]
    public GameObject InventoryPanel;
    public ItemToolTipUI ItemToolTipPanel;
    public ItemPopupUI ItemPopupPanel;

    [Header("Status UI")]
    public GameObject StatusPanel;
    public PlayerStatusText PlayerStatusText;

    [Header("Player Info UI")]
    public PlayerInfoUI PlayerInfoPanel;

    [Header("Equipment UI")]
    public GameObject EquipmentPanel;

    [Header("Quest UI")]
    public GameObject QuestPanel;
    public GameObject QuestViewPanel;
    public GameObject QuestTrackerView;
    public GameObject AchievementViewPanel;

    [Header("Shop UI")]
    public TextMeshProUGUI ShopName;
    public GameObject ShopPanel;

    [Header("Dialog UI")]
    public GameObject DialogPanel;

    [Header("Chracter Create UI")]
    public GameObject CharacterCreatePanel;

    [Header("Interaction Info UI")]
    public GameObject InteractionInfoPanel;
    public TextMeshProUGUI InteractionInfoText;

    [Header("Died UI")]
    public DieUI DiedPanel;

    [Header("Option UI")]
    public PauseUI PausePanel;
    public GameObject OptionPanel;

    public bool IsUIActive =>
        InventoryPanel.activeSelf ||StatusPanel.activeSelf ||
        EquipmentPanel.activeSelf || ShopPanel.activeSelf ||
        DialogPanel.activeSelf || QuestPanel.activeSelf ||
        QuestViewPanel.activeSelf || AchievementViewPanel.activeSelf ||
        PausePanel.gameObject.activeSelf || DiedPanel.gameObject.activeSelf;

    protected override void Awake()
    {
        base.Awake();

        #region Null Check
        if (ItemToolTipPanel == null)
        {
            ItemToolTipPanel = FindObjectOfType<ItemToolTipUI>();
        }

        if (ItemPopupPanel == null)
        {
            ItemPopupPanel = FindObjectOfType<ItemPopupUI>();
        }
        #endregion
    }

    private void Start()
    {
        HidePanel();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.IsGameStart)
        {
            if (InputManager.Instance.UIObject.Count != 0)
            {
                InputManager.Instance.UIObject.Pop().SetActive(false);

                if (ItemPopupPanel.gameObject.activeSelf)
                {
                    ItemPopupPanel.HideThrowUI();
                    ItemPopupPanel.HideQuantityUI();
                    ItemToolTipPanel.gameObject.SetActive(false);
                }

                if (!IsUIActive)
                {
                    GameManager.Instance.HideMouse();
                }
            }
            else if (PausePanel.gameObject.activeSelf)
            {
                if (OptionPanel.activeSelf)
                {
                    InputManager.Instance.UIObject.Pop().SetActive(false);
                }
                else
                {
                    Time.timeScale = 1f;
                    InputManager.Instance.gameObject.SetActive(true);
                    PausePanel.gameObject.SetActive(false);
                }

                GameManager.Instance.HideMouse();
            }
            else
            {
                InputManager.Instance.gameObject.SetActive(false);
                GameManager.Instance.ShowMouse();
                PausePanel.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void HidePanel()
    {
        PlayerInfoPanel.gameObject.SetActive(false);
        StatusPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        EquipmentPanel.SetActive(false);
        QuestPanel.SetActive(false);
        QuestViewPanel.SetActive(false);
        QuestTrackerView.SetActive(false);
        AchievementViewPanel.SetActive(false);
        ShopPanel.SetActive(false);
        CharacterCreatePanel.SetActive(false);
        InteractionInfoPanel.SetActive(false);
        DiedPanel.gameObject.SetActive(false);
        PausePanel.gameObject.SetActive(false);
        OptionPanel.SetActive(false);
        BloodUI.SetActive(false);
    }
}
