using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : DialogSystem.cs
 * Desc     : ��ȭ �ý���
 * Date     : 2024-06-30
 * Writer   : ������
 */

[System.Serializable]
public struct DialogData
{
    public string Name;
    [TextArea(3, 5)]
    public string Dialog;     // ���
}

public class DialogSystem : DontDestroyObject<DialogSystem>
{
    [SerializeField]
    private DialogDB DialogDB;
    private string _currentDialog;                   // ���� �б��� ��� ��� �迭
    private StringBuilder _stringBuilder = new StringBuilder();
    [SerializeField]
    private int _currentDialogIndex;

    [HideInInspector]
    public bool IsFirst = true;                // ���� 1ȸ�� ȣ���ϱ� ���� ����

    [HideInInspector]
    public int Branch;
    //[HideInInspector]
    public string[] Dialogs;                   // ���� �б��� ��� ��� �迭
    [HideInInspector]
    public NpcEntity CurrentNpc;

    public GameObject DialogPanel;       // ��ȭâ Image UI
    public TextMeshProUGUI TextName;            // ���� ������� ĳ���� �̸� ��� Text UI
    public TextMeshProUGUI TextDialogue;        // ���� ��� ��� Text UI
    public GameObject ObjectArrow;      // ��簡 �Ϸ�Ǿ��� �� ��µǴ� Ŀ�� ������Ʈ

    [Header("UI")]
    [SerializeField]
    private Button _talkButton;
    public TextMeshProUGUI OpenText;
    public GameObject ButtonPanel;
    public Button OpenButton;
    [SerializeField]
    private Button _closeButton;

    [Header("Quest")]
    [SerializeField]
    private GameObject _questPanel;
    [SerializeField]
    private QuestGiver _questGiver;
    public GameObject QuestButtonPanel;
    [SerializeField]
    private Button _questAcceptButton;
    public Button QuestCompletionButton;
    [SerializeField]
    private Button _backButton;

    [Header("Shop")]
    [SerializeField]
    private GameObject _shopPanel;
    private TextMeshProUGUI _shopNameText;

    protected override void Awake()
    {
        base.Awake();

        Setup();
        QuestCompletionButton.gameObject.SetActive(false);

        _talkButton.onClick.AddListener(() =>
        {
            CurrentNpc.ChangeState(EnumTypes.NpcState.Talk);

            UpdateDialog();
            ButtonPanel.SetActive(false);
        });

        OpenButton.onClick.AddListener(() =>
        {
            ButtonPanel.SetActive(false);

            switch (CurrentNpc.NpcType)
            {
                case EnumTypes.NpcType.Quest:
                    Dialogs[0] = Globals.NpcDialogue.ActiveQuestDialogue;
                    IsFirst = true;
                    UpdateDialog();
                    QuestButtonPanel.SetActive(true);
                    QuestCompletionButton.gameObject.SetActive(false);

                    foreach (var quest in QuestSystem.Instance.ActiveQuests)
                    {
                        foreach (var npcQuest in CurrentNpc.ActiveQuests)
                        {
                            if (quest.CodeName == npcQuest.CodeName && quest.IsCompletable)
                            {
                                QuestCompletionButton.gameObject.SetActive(true);
                                return;
                            }
                        }
                    }
                    break;
                case EnumTypes.NpcType.Shop:
                    DialogPanel.SetActive(false);
                    _shopPanel.SetActive(true);
                    UIManager.Instance.InventoryPanel.SetActive(true);
                    UIManager.Instance.InventoryPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(500f, 300f);

                    InputManager.Instance.UIObject.Push(UIManager.Instance.InventoryPanel);
                    InputManager.Instance.UIObject.Push(_shopPanel);
                    if (_shopNameText == null)
                    {
                        GameObject obj = GameObject.Find("Shop Name Text");
                        _shopNameText = obj.GetComponent<TextMeshProUGUI>();
                    }

                    _shopNameText.text = CurrentNpc.ShopName;

                    PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);
                    break;
            }
        });

        _closeButton.onClick.AddListener(() =>
        {
            ButtonPanel.SetActive(false);
            DialogPanel.SetActive(false);
            GameManager.Instance.HideMouse();

            IsFirst = true;
            PlayerEntity.ChangeState(EnumTypes.PlayerState.Idle);
        });

        _questAcceptButton.onClick.AddListener(() =>
        {
            if (CurrentNpc.InactiveQuests.Count == 0)
            {
                Dialogs[0] = Globals.NpcDialogue.NoMoreQuestDialouge;
                IsFirst = true;
                UpdateDialog();
                QuestButtonPanel.SetActive(true);
            }
            else
            {
                Color color = CurrentNpc.InactiveQuests[0].Category.TitleColor;
                string r = ((int)(color.r * 255)).ToString("X2");
                string g = ((int)(color.g * 255)).ToString("X2");
                string b = ((int)(color.b * 255)).ToString("X2");
                string a = ((int)(color.a * 255)).ToString("X2");
                string resultColor = string.Format($"{r}{g}{b}{a}");

                _questPanel.SetActive(true);
                _questGiver.NpcEntity = CurrentNpc;
                _questGiver.QuestIcon.sprite = CurrentNpc.InactiveQuests[0].Icon;
                _questGiver.QuestName.text = $"<color=#{resultColor}>[{CurrentNpc.InactiveQuests[0].Category.DisplayName}]</color> " + CurrentNpc.InactiveQuests[0].DisplayName;
                _questGiver.QuestDescription.text = CurrentNpc.InactiveQuests[0].Description;
                foreach (var taskGroup in CurrentNpc.InactiveQuests[0].TaskGroups)
                {
                    foreach(var task in taskGroup.Tasks)
                    {
                        _stringBuilder.Append("- ");
                        _stringBuilder.Append(task.Description);
                        _stringBuilder.Append($" (0 / {task.NeedSuccessToComplete})");
                        _stringBuilder.Append("\n");
                    }
                }

                _questGiver.TaskDescription.text = _stringBuilder.ToString();
                _stringBuilder.Clear();

                foreach (var reward in CurrentNpc.InactiveQuests[0].Rewards)
                {
                    _stringBuilder.Append("- ");
                    _stringBuilder.Append(reward.Description);
                    _stringBuilder.Append(" +");
                    _stringBuilder.Append(reward.Quantity);
                    _stringBuilder.Append("\n");

                    _questGiver.QuestRewardDescription.text = _stringBuilder.ToString();
                }

                if (CurrentNpc.InactiveQuests[0].Rewards.Count == 0)
                {
                    _questGiver.QuestRewardDescription.text = "����.";
                }

                _stringBuilder.Clear();

                foreach (var condition in CurrentNpc.InactiveQuests[0].AcceptionConditions)
                {
                    _stringBuilder.Append("* ");
                    _stringBuilder.Append(condition.Description);

                    _stringBuilder.Append("\n");

                    _questGiver.QeustConditions.text = _stringBuilder.ToString();
                }
                _stringBuilder.Clear();
            }
        });

        QuestCompletionButton.onClick.AddListener(() =>
        {
            bool isComplete = false;

            foreach (var quest in QuestSystem.Instance.ActiveQuests)
            {
                foreach (var npcQuest in CurrentNpc.ActiveQuests)
                {
                    if (quest.CodeName == npcQuest.CodeName && quest.IsCompletable)
                    {
                        isComplete = true;
                        quest.Complete();
                        CurrentNpc.ActiveQuests.Remove(npcQuest);
                        CurrentNpc.UpdateMark();
                        goto Exit;
                    }
                }
            }

            Exit:
            if (isComplete)
            {
                Dialogs[0] = Globals.NpcDialogue.CompleteQuestDialouge;
                IsFirst = true;
                UpdateDialog();
                QuestCompletionButton.gameObject.SetActive(false);
                QuestButtonPanel.SetActive(true);
            }
        });

        _backButton.onClick.AddListener(() =>
        {
            Dialogs[0] = CurrentNpc.FirstDialogMessage;
            IsFirst = true;
            UpdateDialog();
            ButtonPanel.SetActive(true);
            QuestButtonPanel.SetActive(false);
        });
    }

    private void Setup()
    {
        DialogPanel.SetActive(false);
        ButtonPanel.SetActive(false);
        QuestButtonPanel.SetActive(false);
    }

    public bool UpdateDialog()
    {
        // ��� �бⰡ ���۵� �� 1ȸ�� ȣ��
        if (IsFirst == true)
        {
            // �ʱ�ȭ. ĳ���� �̹����� Ȱ��ȭ�ϰ�, ��� ���� UI�� ��� ��Ȱ��ȭ
            Setup();

            _currentDialogIndex = 0;

            SetNextDialog();

            IsFirst = false;

            if (CurrentNpc.NpcType != EnumTypes.NpcType.Default)
            {
                return false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // ��簡 �������� ��� ���� ��� ����
            if (Dialogs[_currentDialogIndex] != null)
            {
                SetNextDialog();
            }
            // ��簡 �� �̻� ���� ��� ��� ������Ʈ�� ��Ȱ��ȭ�ϰ� true ��ȯ
            else
            {
                // ���� ��ȭ�� �����ߴ� ��� ĳ����, ��ȭ ���� UI�� ������ �ʰ� ��Ȱ��ȭ
                DialogPanel.SetActive(false);

                DialogDBSetting();
                IsFirst = true;
                return true;
            }
        }

        return false;
    }

    public void SetNextDialog()
    {
        // ���� ȭ���� ��ȭ ���� ������Ʈ Ȱ��ȭ
        DialogPanel.SetActive(true);

        _currentDialog = Dialogs[_currentDialogIndex++];
        // ���� ȭ�� �̸� �ؽ�Ʈ ����
        TextDialogue.text = _currentDialog;
    }

    public void DialogDBSetting()
    {
        int index = 1;
        Dialogs = new string[DialogDB.Entities.Count + 1]; // 1���� ����

        for (int i = 1; i < DialogDB.Entities.Count; ++i)
        {
            if (DialogDB.Entities[i].Branch == Branch)
            {
                _currentDialog = DialogDB.Entities[i].Dialog;
                Dialogs[index++] = _currentDialog;
            }
        }
    }
}

