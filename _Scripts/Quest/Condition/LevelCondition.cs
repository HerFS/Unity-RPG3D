using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : LevelCondition.cs
 * Desc     : 레벨 수락 조건
 * Date     : 2024-06-18
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Condition/LevelCondition", fileName = "LevelCondition_")]
public class LevelCondition : Condition
{
    [SerializeField]
    private int _levelValue;

    public override bool IsPass(Quest quest)
    {
        if (DataManager.Instance.PlayerStatus.Level >= _levelValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
