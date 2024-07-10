using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : TaskTarget.cs
 * Desc     : ScriptableObject
 *            작업(Task)에서 어떤 오브젝트를 목표로 할 것인지
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public abstract class TaskTarget : ScriptableObject
{
    public abstract object Target { get; }
    public abstract bool IsEqual(object target);
}
