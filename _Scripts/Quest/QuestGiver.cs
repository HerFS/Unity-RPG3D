using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : QuestGiver.cs
 * Desc     : 퀘스트를 전달하는 창
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

public class QuestGiver : DontDestroyObject<QuestGiver>
{
    [HideInInspector]
    public NpcEntity NpcEntity;
    [Header("UI")]
    public GameObject QuestPanel;
    public Image QuestIcon;
    public TextMeshProUGUI QuestName;
    public TextMeshProUGUI QuestDescription;
    public TextMeshProUGUI TaskDescription;
    [Header("Reward")]
    public TextMeshProUGUI QuestRewardDescription;
    public TextMeshProUGUI QeustConditions;
    [Header("Button")]
    [SerializeField]
    private Button _acceptButton;
    [SerializeField]
    private Button _refuseButton;

    protected override void Awake()
    {
        base.Awake();

        _acceptButton.onClick.AddListener(() =>
        {
            Quest newQuest = NpcEntity.InactiveQuests[0];
            if (newQuest.IsAcceptable && !QuestSystem.Instance.ContainsInCompletedQuests(newQuest))
            {
                QuestSystem.Instance.Register(newQuest);
                NpcEntity.DialogSystem.Dialogs[0] = Globals.NpcDialogue.StartQuestDialogue;
            }
            else
            {
                NpcEntity.DialogSystem.Dialogs[0] = Globals.NpcDialogue.ConditionQuestDialogue;
            }

            NpcEntity.DialogSystem.IsFirst = true;
            NpcEntity.DialogSystem.UpdateDialog();
            NpcEntity.DialogSystem.QuestButtonPanel.SetActive(true);

            QuestPanel.SetActive(false);
            UIManager.Instance.QuestTrackerView.SetActive(true);

            GameManager.Instance.HideMouse();
        });

        _refuseButton.onClick.AddListener(() =>
        {
            QuestPanel.SetActive(false);
            GameManager.Instance.HideMouse();
        });
    }
}
