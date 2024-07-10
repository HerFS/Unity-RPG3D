using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : LevelCondition.cs
 * Desc     : ���� ���� ����
 * Date     : 2024-06-18
 * Writer   : ������
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
