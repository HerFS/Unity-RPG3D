using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : StringTarget.cs
 * Desc     : TaskTarget 상속
 *            String 타입으로 된 타겟
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Target/String", fileName = "Target_")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string _target;
    public override object Target => _target;
    public override bool IsEqual(object target)
    {
        string targetAsString = target as string;

        if (targetAsString == null)
        {
            return false;
        }

        return (_target == targetAsString);
    }
}
