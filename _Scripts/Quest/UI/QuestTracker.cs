using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * File     : QuestTracker.cs
 * Desc     : 대화 시스템
 * Date     : 2024-06-10
 * Writer   : 정지훈
 */

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _questTitleText;
    [SerializeField]
    private TaskDescriptor _taskDescriptorPrefab;

    private Dictionary<Task, TaskDescriptor> _taskDescriptorsByTask = new Dictionary<Task, TaskDescriptor>();
    [HideInInspector]
    public Quest TargetQuest;

    private void OnDestroy()
    {
        if (TargetQuest != null)
        {
            TargetQuest.onNewTaskGroup -= UpdateTaskDescriptors;
            TargetQuest.onCompleted -= DestroySelf;
        }

        foreach (var tuple in _taskDescriptorsByTask)
        {
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }

    public void Setup(Quest targetQuest, Color titleColor)
    {
        this.TargetQuest = targetQuest;
        _questTitleText.text = targetQuest.Category == null ? targetQuest.DisplayName : $"[{targetQuest.Category.DisplayName}] <color=#FFFFFF> {targetQuest.DisplayName} </color>";

        _questTitleText.color = titleColor;

        targetQuest.onNewTaskGroup += UpdateTaskDescriptors;
        targetQuest.onCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptors(targetQuest, taskGroups[0]);

        if (taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for (int i = 1; i < taskGroups.Count; ++i)
            {
                var taskGroup = taskGroups[i];
                UpdateTaskDescriptors(targetQuest, taskGroup, taskGroups[i - 1]);

                if (taskGroup == targetQuest.CurrentTaskGroup)
                {
                    break;
                }

            }
        }
    }

    private void UpdateTaskDescriptors(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (var task in currentTaskGroup.Tasks)
        {
            var taskDescriptor = Instantiate(_taskDescriptorPrefab, this.transform);
            taskDescriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;

            _taskDescriptorsByTask.Add(task, taskDescriptor);
        }

        if (prevTaskGroup != null)
        {
            foreach (var task in prevTaskGroup.Tasks)
            {
                var taskDescriptor = _taskDescriptorsByTask[task];
                taskDescriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }

    private void UpdateText(Task task, int currentSuccess, int prevSuccess)
    {
        _taskDescriptorsByTask[task].UpdateText(task);
    }

    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }
}
