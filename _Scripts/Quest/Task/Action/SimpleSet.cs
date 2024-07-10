using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : SimpleSet.cs
 * Desc     : TaskAction 상속
 *            스스로 들어온 성공 값을 현재 성공 값에 대입
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Action/Simple Set", fileName = "Simple Set")]
public class SimpleSet : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount;
    }
}
