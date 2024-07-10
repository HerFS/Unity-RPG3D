using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : GameObjectTarget.cs
 * Desc     : TaskTarget 상속
 *            GameObject 타입으로 된 타겟
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Task/Target/GameObject", fileName = "Target_")]
public class GameObjectTarget : TaskTarget
{
    [SerializeField]
    private GameObject _target;
    public override object Target => _target;
    public override bool IsEqual(object target)
    {
        GameObject targetAsGameObject = target as GameObject;

        if (targetAsGameObject == null)
        {
            return false;
        }

        return targetAsGameObject.name.Contains(_target.name);
    }
}
