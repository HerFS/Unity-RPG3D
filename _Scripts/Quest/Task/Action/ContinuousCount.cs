using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ContinuousCount.cs
 * Desc     : TaskAction ���
 *            ���� ���� ���� ���������� ����� ���� Count
 * Date     : 2024-05-06
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Continuous Count", fileName = "Continuous Count")]
public class ContinuousCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return (successCount > 0) ? (currentSuccess + successCount) : 0;
    }
}
