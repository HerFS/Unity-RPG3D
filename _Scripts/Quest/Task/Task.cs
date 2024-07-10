using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * File     : Task.cs
 * Desc     : ScriptableObject
 *            퀘스트에서의 구성요소 중 실제로 해야하는 작업(Task)
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

public enum TaskState
{
    Inactive = 0,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [Header("Category")]
    [SerializeField]
    private Category _category;

    [Header("Text")]
    [SerializeField]
    private string _codeName;
    [SerializeField, TextArea]
    private string _description;

    [Header("Action")]
    [SerializeField]
    private TaskAction _action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] _targets;

    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue _initialSuccessValue;
    [SerializeField]
    private int _needSuccessToComplete;
    [SerializeField]
    private bool _canReceiveReportDuringCompletion;

    private TaskState _currentState;
    private int _currentSuccess;

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    public Quest Owner { get; private set; }
    public TaskState State
    {
        get => _currentState;
        set
        {
            TaskState prevState = _currentState;
            _currentState = value;
            onStateChanged?.Invoke(this, _currentState, prevState);
        }
    }
    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, _needSuccessToComplete);
            if (_currentSuccess != prevSuccess)
            {
                State = (_currentSuccess == _needSuccessToComplete) ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, _currentSuccess, prevSuccess);
            }
        }
    }

    public Category Category => _category;
    public string CodeName => _codeName;
    public string Description => _description;
    public int NeedSuccessToComplete => _needSuccessToComplete;
    public bool IsComplete => (State == TaskState.Complete);

    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    public void Start()
    {
        State = TaskState.Running;

        if (_initialSuccessValue)
        {
            CurrentSuccess = _initialSuccessValue.GetValue(this);
        }
    }

    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = _action.Run(this, CurrentSuccess, successCount);
    }

    public void Complete()
    {
        CurrentSuccess = _needSuccessToComplete;
    }

    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }

    public bool IsTarget(string category, object target)
        => (Category == category) &&
        _targets.Any(x => x.IsEqual(target)) &&
        (!IsComplete || (IsComplete && _canReceiveReportDuringCompletion));

    public bool ContainsTarget(object target) => _targets.Any(x => x.IsEqual(target));
}
