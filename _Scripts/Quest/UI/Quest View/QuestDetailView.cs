using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : QuestDetailView.cs
 * Desc     : ƒ˘Ω∫∆ÆµÈ¿« º≥∏Ì¿ª ∫∏ø©¡‹
 * Date     : 2024-06-30
 * Writer   : ¡§¡ˆ»∆
 */

public class QuestDetailView : MonoBehaviour
{
    [SerializeField]
    private List<QuestTracker> _taskTrackerList = new List<QuestTracker>();

    [SerializeField]
    private GameObject _displayGroup;
    [SerializeField]
    private Button _trackerButton;
    [SerializeField]
    private Button _cancelButton;

    [Header("Quest Description")]
    [SerializeField]
    private Image _questIcon;
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _description;

    [Header("Task Description")]
    [SerializeField]
    private RectTransform _taskDescriptorGroup;
    [SerializeField]
    private TaskDescriptor _taskDescriptorPrefab;
    [SerializeField]
    private int _taskDescriptorPoolCount;

    [Header("Reward Description")]
    [SerializeField]
    private RectTransform _rewardDescriptionGroup;
    [SerializeField]
    private TextMeshProUGUI _rewardDescriptionPrefab;
    [SerializeField]
    private int _rewardDescriptionPoolCount;

    private List<TaskDescriptor> _taskDescriptorPool;
    private List<TextMeshProUGUI> _rewardDescriptionPool;

    public Quest Target { get; private set; }

    private void Awake()
    {
        _taskDescriptorPool = CreatePool(_taskDescriptorPrefab, _taskDescriptorPoolCount, _taskDescriptorGroup);
        _rewardDescriptionPool = CreatePool(_rewardDescriptionPrefab, _rewardDescriptionPoolCount, _rewardDescriptionGroup);
        _displayGroup.SetActive(false);
    }

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        questSystem.onQuestRegistered += AddQuestTracker;
        _cancelButton.onClick.AddListener(CancelQuest);
        _cancelButton.gameObject.SetActive(false);
        _trackerButton.onClick.AddListener(ShowTrackerView);
        _trackerButton.gameObject.SetActive(false);
    }

    private List<T> CreatePool<T>(T prefab, int count, RectTransform parent) where T : MonoBehaviour
    {
        var pool = new List<T>(count);
        for (int i = 0; i < count; ++i)
        {
            pool.Add(Instantiate(prefab, parent));
        }

        return pool;
    }

    private void CancelQuest()
    {
        if (Target.IsCancelable)
        {
            foreach (var taskTracker in _taskTrackerList)
            {
                if (Target == taskTracker.TargetQuest)
                {
                    _trackerButton.gameObject.SetActive(false);
                    Destroy(taskTracker.gameObject);
                }
            }

            Target.Cancel();
        }
    }

    private void ShowTrackerView()
    {
        foreach (var taskTracker in _taskTrackerList)
        {
            if (!UIManager.Instance.QuestTrackerView.activeSelf)
            {
                UIManager.Instance.QuestTrackerView.SetActive(true);
            }

            if (Target == taskTracker.TargetQuest)
            {
                AddQuestTracker();
                taskTracker.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void Show(Quest quest)
    {
        _displayGroup.SetActive(true);
        Target = quest;

        _questIcon.sprite = quest.Icon;
        _title.text = quest.DisplayName;
        _description.text = quest.Description;

        int taskIndex = 0;

        foreach (var taskGroup in quest.TaskGroups)
        {
            foreach (var task in taskGroup.Tasks)
            {
                var poolObject = _taskDescriptorPool[taskIndex++];
                poolObject.gameObject.SetActive(true);

                if (task.IsComplete)
                {
                    poolObject.UpdateTextUsingStrikeThrough(task);
                }
                else
                {
                    poolObject.UpdateText(task);
                }
            }
        }

        for (int i = taskIndex; i < _taskDescriptorPool.Count; ++i)
        {
            _taskDescriptorPool[i].gameObject.SetActive(false);
        }

        var rewards = quest.Rewards;
        var rewardCount = rewards.Count;

        for (int i = 0; i < _rewardDescriptionPoolCount; ++i)
        {
            var poolObject = _rewardDescriptionPool[i];
            if (i < rewardCount)
            {
                var reward = rewards[i];
                poolObject.text = $"- {reward.Description} +{reward.Quantity}";
                poolObject.gameObject.SetActive(true);
            }
            else
            {
                poolObject.gameObject.SetActive(false);
            }
        }

        _trackerButton.gameObject.SetActive(!quest.IsComplete);
        _cancelButton.gameObject.SetActive(quest.IsCancelable && !quest.IsComplete);
    }

    public void Hide()
    {
        Target = null;
        _displayGroup.SetActive(false);
        _cancelButton.gameObject.SetActive(false);
        _trackerButton.gameObject.SetActive(false);
    }

    public void AddQuestTracker(Quest quest = null)
    {
        var taskTrackers = FindObjectsOfType<QuestTracker>();

        foreach (var taskTracker in taskTrackers)
        {
            _taskTrackerList.Add(taskTracker);
        }
    }
}
