using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : SimpleCount.cs
 * Desc     : TaskAction ���
 *            ���� ������ ���� ������ Count ���� ���ؼ� ����
 * Date     : 2024-05-06
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Simple Count", fileName = "Simple Count")]
public class SimpleCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return (currentSuccess + successCount);
    }
}
