using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : NegativeCount.cs
 * Desc     : TaskAction 상속
 *            들어온 성공 값이 음수일 때만 Count
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Negative Count", fileName = "Negative Count")]
public class NegativeCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return (successCount < 0) ? (currentSuccess + successCount) : currentSuccess;
    }
}
