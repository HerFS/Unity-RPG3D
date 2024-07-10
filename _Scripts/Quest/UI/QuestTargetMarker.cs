using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * File     : QuestTargetMarker.cs
 * Desc     : 퀘스트 목표 대상 마크 표시
 * Date     : 2024-06-17
 * Writer   : 정지훈
 */

public class QuestTargetMarker : MonoBehaviour
{

    [SerializeField]
    private TaskTarget _target;
    [SerializeField]
    private Material _markerMaterial;
    [SerializeField]
    private Category[] _markerMaterialCategorys;

    private Dictionary<Quest, Task> _targetTasksByQuest = new Dictionary<Quest, Task>();
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        gameObject.SetActive(false);

        questSystem.onQuestRegistered += TryAddTargetQuest;
        questSystem.onQuestCanceled += CancelQuest;

        foreach (var quest in QuestSystem.Instance.ActiveQuests)
        {
            TryAddTargetQuest(quest);
        }
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;

        if (questSystem)
        {
            questSystem.onQuestRegistered -= TryAddTargetQuest;
            questSystem.onQuestCanceled -= CancelQuest;
        }

        foreach ((Quest quest, Task task) in _targetTasksByQuest)
        {
            quest.onNewTaskGroup -= UpdateTargetTask;
            quest.onCompleted -= RemoveTargetQuest;
            task.onStateChanged -= UpdateRunningTargetTaskCount;
        }
    }

    private void TryAddTargetQuest(Quest quest)
    {
        if (_target != null && quest.ContainsTarget(_target))
        {
            quest.onNewTaskGroup += UpdateTargetTask;
            quest.onCompleted += RemoveTargetQuest;

            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }

    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        _targetTasksByQuest.Remove(quest);

        var task = currentTaskGroup.FindTaskByTarget(_target);
        if (task != null)
        {
            _targetTasksByQuest[quest] = task;
            task.onStateChanged += UpdateRunningTargetTaskCount;

            UpdateRunningTargetTaskCount(task, task.State);
        }
    }

    private void RemoveTargetQuest(Quest quest) => _targetTasksByQuest.Remove(quest);

    private void UpdateRunningTargetTaskCount(Task task, TaskState currentState, TaskState prevState = TaskState.Inactive)
    {
        if (currentState == TaskState.Running)
        {
            _renderer.material = _markerMaterial;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void CancelQuest(Quest quest)
    {
        if (quest.IsCancel)
        {
            gameObject.SetActive(false);
        }
    }
}
