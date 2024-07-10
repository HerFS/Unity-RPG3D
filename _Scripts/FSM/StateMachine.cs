using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : StateMachine.cs
 * Desc     : ���¿� ������ ��� �����Ϳ� �޼ҵ带 ����
 * Date     : 2024-04-30
 * Writer   : ������
 */

public class StateMachine<T> where T : class
{
    private T _ownerEntity;
    private StateOfPlay<T> _currentState;
    public StateOfPlay<T> CurrentState => _currentState;

    public void Setup(T owner, StateOfPlay<T> entryState)
    {
        _ownerEntity = owner;
        _currentState = null;

        ChangeState(entryState);
    }

    public void Execute()
    {
        if (_currentState != null)
        {
            _currentState.Execute(_ownerEntity);
        }
    }

    public void ChangeState(StateOfPlay<T> newState)
    {
        if (newState == null)
        {
            return;
        }

        if (_currentState != null)
        {
            _currentState.Exit(_ownerEntity);
        }

        _currentState = newState;
        _currentState.Enter(_ownerEntity);
    }
}
