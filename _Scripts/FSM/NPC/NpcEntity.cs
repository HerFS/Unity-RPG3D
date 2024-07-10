using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

/*
 * File     : NpcEntity.cs
 * Desc     : BaseGameEntity 상속
 *            Entity의 한 종류 Npc
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(SphereCollider))]
public class NpcEntity : BaseGameEntity
{
    private readonly string _questOpenButtonText = "퀘스트 열기";
    private readonly string _shopOpenButtonText = "상점 열기";

    private Rigidbody _rigidbody;
    private Shop _shop;
    private Quaternion _rotation;
    [SerializeField]
    private TextMeshProUGUI _npcNameText;

    [HideInInspector]
    public StateOfPlay<NpcEntity>[] States;
    [HideInInspector]
    public StateMachine<NpcEntity> StateMachine;

    public string NpcName;
    [TextArea]
    public string FirstDialogMessage;

    public EnumTypes.NpcType NpcType;

    [HideInInspector]
    public QuestMarker QuestMarker;
    [HideInInspector]
    public List<Quest> InactiveQuests;
    [HideInInspector]
    public List<Quest> ActiveQuests = new List<Quest>();
    [HideInInspector]
    public string ShopName;
    [HideInInspector]
    public ItemData[] ShopItemData;
    [HideInInspector]
    public DialogSystem DialogSystem;
    public int Branch;

    private void Awake()
    {
        Setup();

        _rigidbody = GetComponent<Rigidbody>();
        QuestMarker = GetComponentInChildren<QuestMarker>();

        States = new StateOfPlay<NpcEntity>[2];
        StateMachine = new StateMachine<NpcEntity>();

        States[(int)EnumTypes.NpcState.Idle] = new NpcOwnedStates.Idle();
        States[(int)EnumTypes.NpcState.Talk] = new NpcOwnedStates.Talk();

        StateMachine.Setup(this, States[(int)EnumTypes.NpcState.Idle]);
    }

    private void Start()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _rotation = transform.rotation;
        _npcNameText.text = NpcName;

        if (DialogSystem == null)
        {
            DialogSystem = FindObjectOfType<DialogSystem>();
        }

        if (NpcType == EnumTypes.NpcType.Quest)
        {
            var questSystem = QuestSystem.Instance;

            questSystem.onQuestRegistered += AddToRegisterQuest;
            questSystem.onQuestCanceled += RemoveToActiveQuest;
            questSystem.onQuestStateChanged += UpdateMark;

            UpdateQuests();
            UpdateMark();
        }
        else if (NpcType == EnumTypes.NpcType.Shop)
        {
            _shop = FindObjectOfType<Shop>();
        }

        if (NpcType != EnumTypes.NpcType.Quest)
        {
            QuestMarker.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Updated();
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;
        if (questSystem)
        {
            questSystem.onQuestRegistered -= AddToRegisterQuest;
            questSystem.onQuestCanceled -= RemoveToActiveQuest;
            questSystem.onQuestStateChanged -= UpdateMark;
        }
    }

    public override void Updated()
    {
        StateMachine.Execute();
    }

    public void ChangeState(EnumTypes.NpcState newState)
    {
        StateMachine.ChangeState(States[(int)newState]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            UIManager.Instance.InteractionInfoPanel.SetActive(true);
            UIManager.Instance.InteractionInfoText.text = $"\"{NpcName}\" 대화하기 (F)";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player) &&
            InputManager.Instance.IsInteraction &&
            PlayerEntity.StateMachine.CurrentState == PlayerEntity.States[(int)EnumTypes.PlayerState.Idle])
        {
            UIManager.Instance.InteractionInfoPanel.SetActive(false);
            transform.LookAt(new Vector3(GameManager.Instance.Player.position.x, transform.position.y, GameManager.Instance.Player.position.z));

            #region DialogSystem Setup
            DialogSystem.Branch = Branch;
            DialogSystem.TextName.text = NpcName;
            DialogSystem.CurrentNpc = this;
            DialogSystem.DialogDBSetting();
            DialogSystem.Dialogs[0] = FirstDialogMessage;
            DialogSystem.IsFirst = true;
            DialogSystem.UpdateDialog();
            DialogSystem.ButtonPanel.SetActive(true);

            PlayerEntity.ChangeState(EnumTypes.PlayerState.Talk);

            GameManager.Instance.ShowMouse();

            switch (NpcType)
            {
                case EnumTypes.NpcType.Default:
                    DialogSystem.OpenButton.gameObject.SetActive(false);
                    break;
                case EnumTypes.NpcType.Quest:
                    DialogSystem.OpenButton.gameObject.SetActive(true);
                    DialogSystem.OpenText.text = _questOpenButtonText;
                    break;
                case EnumTypes.NpcType.Shop:
                    for (int i = 0; i < _shop.InitialSlotCount; ++i)
                    {
                        if (ShopItemData.Length > i)
                        {
                            _shop.ItemSlots[i].ItemData = ShopItemData[i];
                        }
                        else
                        {
                            _shop.ItemSlots[i].ItemData = null;
                        }
                        
                        _shop.ItemSlots[i].ShopSlotSetup();
                    }

                    DialogSystem.OpenButton.gameObject.SetActive(true);
                    DialogSystem.OpenText.text = _shopOpenButtonText;
                    break;
            }
            #endregion
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains(Globals.TagName.Player))
        {
            UIManager.Instance.ShopPanel.SetActive(false);
            UIManager.Instance.QuestPanel.SetActive(false);
            UIManager.Instance.InteractionInfoPanel.SetActive(false);
            DialogSystem.DialogPanel.SetActive(false);
            DialogSystem.ButtonPanel.SetActive(false);

            transform.rotation = _rotation;
        }
    }

    private void AddToRegisterQuest(Quest quest)
    {
        for (int i = 0; i < InactiveQuests.Count; ++i)
        {
            if (InactiveQuests[i].CodeName == quest.CodeName)
            {
                ActiveQuests.Add(InactiveQuests[i]);
                InactiveQuests.Remove(InactiveQuests[i]);

                UpdateMark();
                return;
            }
        }
    }

    private void RemoveToActiveQuest(Quest quest)
    {
        for (int i = 0; i < ActiveQuests.Count; ++i)
        {
            if (ActiveQuests[i].CodeName == quest.CodeName)
            {
                InactiveQuests.Insert(0, ActiveQuests[i]);
                ActiveQuests.Remove(ActiveQuests[i]);

                UpdateMark();
                return;
            }
        }
    }
    
    public void UpdateMark()
    {
        QuestMarker.gameObject.SetActive(false);

        if (InactiveQuests.Count > 0)
        {
            QuestMarker.gameObject.SetActive(true);
            QuestMarker.Renderer.material = QuestMarker.ExclamationMarkMaterial;
        }
        
        if (ActiveQuests.Count > 0)
        {
            QuestMarker.gameObject.SetActive(true);
            QuestMarker.Renderer.material.color = new Color(1f, 1f, 1f);
            QuestMarker.Renderer.material = QuestMarker.QuestionMarkMaterial;

            foreach (var quest in QuestSystem.Instance.ActiveQuests)
            {
                foreach (var npcQuest in ActiveQuests)
                {
                    if (quest.CodeName == npcQuest.CodeName && quest.IsCompletable)
                    {
                        QuestMarker.Renderer.material.color = new Color(1f, 1f, 0f);
                        return;
                    }
                }
            }
        }
    }

    public void UpdateQuests()
    {
        var questSystem = QuestSystem.Instance;

        foreach (var quest in questSystem.ActiveQuests)
        {
            for (int i = 0; i < InactiveQuests.Count; ++i)
            {
                if (quest.CodeName == InactiveQuests[i].CodeName)
                {
                    ActiveQuests.Add(InactiveQuests[i]);
                    InactiveQuests.Remove(InactiveQuests[i]);
                }
            }
        }

        foreach (var quest in questSystem.CompletedQuests)
        {
            for (int i = 0; i < InactiveQuests.Count; ++i)
            {
                if (quest.CodeName == InactiveQuests[i].CodeName && quest.IsComplete)
                {
                    InactiveQuests.Remove(InactiveQuests[i]);
                }
            }

            for (int i = 0; i < ActiveQuests.Count; ++i)
            {
                if (quest.CodeName == ActiveQuests[i].CodeName && quest.IsComplete)
                {
                    ActiveQuests.Remove(ActiveQuests[i]);
                }
            }
        }
    }
}
