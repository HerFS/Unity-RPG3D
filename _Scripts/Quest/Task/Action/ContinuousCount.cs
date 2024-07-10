using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ContinuousCount.cs
 * Desc     : TaskAction 상속
 *            들어온 성공 값이 연속적으로 양수일 때만 Count
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Continuous Count", fileName = "Continuous Count")]
public class ContinuousCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return (successCount > 0) ? (currentSuccess + successCount) : 0;
    }
}
