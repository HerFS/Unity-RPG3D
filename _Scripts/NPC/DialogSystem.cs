using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : DialogSystem.cs
 * Desc     : 대화 시스템
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

[System.Serializable]
public struct DialogData
{
    public string Name;
    [TextArea(3, 5)]
    public string Dialog;     // 대사
}

public class DialogSystem : DontDestroyObject<DialogSystem>
{
    [SerializeField]
    private DialogDB DialogDB;
    private string _currentDialog;                   // 현재 분기의 대사 목록 배열
    private StringBuilder _stringBuilder = new StringBuilder();
    [SerializeField]
    private int _currentDialogIndex;

    [HideInInspector]
    public bool IsFirst = true;                // 최초 1회만 호출하기 위한 변수

    [HideInInspector]
    public int Branch;
    //[HideInInspector]
    public string[] Dialogs;                   // 현재 분기의 대사 목록 배열
    [HideInInspector]
    public NpcEntity CurrentNpc;

    public GameObject DialogPanel;       // 대화창 Image UI
    public TextMeshProUGUI TextName;            // 현재 대사중인 캐릭터 이름 출력 Text UI
    public TextMeshProUGUI TextDialogue;        // 현재 대사 출력 Text UI
    public GameObject ObjectArrow;      // 대사가 완료되었을 때 출력되는 커서 오브젝트

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
                    _questGiver.QuestRewardDescription.text = "없음.";
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
        // 대사 분기가 시작될 때 1회만 호출
        if (IsFirst == true)
        {
            // 초기화. 캐릭터 이미지는 활성화하고, 대사 관련 UI는 모두 비활성화
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
            // 대사가 남아있을 경우 다음 대사 진행
            if (Dialogs[_currentDialogIndex] != null)
            {
                SetNextDialog();
            }
            // 대사가 더 이상 없을 경우 모든 오브젝트를 비활성화하고 true 반환
            else
            {
                // 현재 대화에 참여했던 모든 캐릭터, 대화 관련 UI를 보이지 않게 비활성화
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
        // 현재 화자의 대화 관련 오브젝트 활성화
        DialogPanel.SetActive(true);

        _currentDialog = Dialogs[_currentDialogIndex++];
        // 현재 화자 이름 텍스트 설정
        TextDialogue.text = _currentDialog;
    }

    public void DialogDBSetting()
    {
        int index = 1;
        Dialogs = new string[DialogDB.Entities.Count + 1]; // 1부터 시작

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

