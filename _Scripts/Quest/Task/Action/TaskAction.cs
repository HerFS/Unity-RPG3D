using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : SimpleCount.cs
 * Desc     : ScriptableObject
 *            Ư���� �ൿ�� �ߴ���(=�����ߴ���) Ȯ���ϴ� �׼�
 * Date     : 2024-05-06
 * Writer   : ������
 */

public abstract class TaskAction : ScriptableObject
{
    public abstract int Run(Task task, int currentSuccess, int successCount);
}
