using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : SimpleSet.cs
 * Desc     : TaskAction ���
 *            ������ ���� ���� ���� ���� ���� ���� ����
 * Date     : 2024-05-06
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Simple Set", fileName = "Simple Set")]
public class SimpleSet : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount;
    }
}
