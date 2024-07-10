using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : SimpleCount.cs
 * Desc     : ScriptableObject
 *            특정한 행동을 했는지(=성공했는지) 확인하는 액션
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public abstract class TaskAction : ScriptableObject
{
    public abstract int Run(Task task, int currentSuccess, int successCount);
}
