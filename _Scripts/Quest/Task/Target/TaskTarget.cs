using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : TaskTarget.cs
 * Desc     : ScriptableObject
 *            �۾�(Task)���� � ������Ʈ�� ��ǥ�� �� ������
 * Date     : 2024-05-06
 * Writer   : ������
 */

public abstract class TaskTarget : ScriptableObject
{
    public abstract object Target { get; }
    public abstract bool IsEqual(object target);
}
