using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * File     : TaskGroup.cs
 * Desc     : 작업(Task)들의 모음
 * Date     : 2024-06-17
 * Writer   : 정지훈
 */

public enum TaskGroupState
{
    Inactive = 0,
    Running,
    Complete,
}

[System.Serializable]
public class TaskGroup
{
    [SerializeField]
    private Task[] _tasks;

    public Quest Owner { get; private set; }
    public TaskGroupState State { get; private set; }
    public IReadOnlyList<Task> Tasks => _tasks;
    public bool IsComplete => (State == TaskGroupState.Complete);
    public bool IsAllTaskComplete => _tasks.All(x => x.IsComplete);

    public TaskGroup(TaskGroup copyTarget)
    {
        _tasks = copyTarget.Tasks.Select(x => Object.Instantiate(x)).ToArray();
    }

    public void Setup(Quest owner)
    {
        Owner = owner;

        foreach (var task in _tasks)
        {
            task.Setup(owner);
        }
    }

    public void Start()
    {
        State = TaskGroupState.Running;

        foreach (var task in _tasks)
        {
            task.Start();
        }
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        foreach (var task in _tasks)
        {
            if (task.IsTarget(category, target))
            {
                task.ReceiveReport(successCount);
            }
        }
    }

    public void Complete()
    {
        if (IsComplete)
        {
            return;
        }

        State = TaskGroupState.Complete;

        foreach (var task in _tasks)
        {
            if (!task.IsComplete)
            {
                task.Complete();
            }
        }
    }

    public void End()
    {
        State = TaskGroupState.Complete;

        foreach (var task in _tasks)
        {
            task.End();
        }
    }

    public Task FindTaskByTarget(object target) => _tasks.FirstOrDefault(x => x.ContainsTarget(target));
    public Task FindTaskByTarget(TaskTarget target) => FindTaskByTarget(target.Target);
    public bool ContainsTarget(object target) => _tasks.Any(x => x.ContainsTarget(target));
    public bool ContainsTarget(TaskTarget target) => ContainsTarget(target.Target);
}
