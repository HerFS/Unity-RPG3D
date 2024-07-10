using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : StateOfPlay.cs
 * Desc     : ��� ���°� ��ӹ޴� ��� Ŭ����
 * Date     : 2024-04-30
 * Writer   : ������
 */

public abstract class StateOfPlay<T> where T : class
{
    /// <summary>
    /// �ش� ���¸� ������ �� 1ȸ ȣ��
    /// </summary>
    public abstract void Enter(T entity);

    /// <summary>
    /// �ش� ���¸� ������Ʈ �� �� �� ������ ȣ��
    /// </summary>
    public abstract void Execute(T entity);

    /// <summary>
    /// �ش� ���¸� �����ϸ� 1ȸ ȣ��
    /// </summary>
    public abstract void Exit(T entity);
}
