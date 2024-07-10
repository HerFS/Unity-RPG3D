using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : SimpleCount.cs
 * Desc     : TaskAction 상속
 *            현재 성공한 값에 성공한 Count 값을 더해서 전달
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Simple Count", fileName = "Simple Count")]
public class SimpleCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return (currentSuccess + successCount);
    }
}
