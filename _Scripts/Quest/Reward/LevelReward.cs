using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : LevelReward.cs
 * Desc     : ����Ʈ �����ϸ� �޴� ����
 * Date     : 2024-06-18
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Reward/LevelReward", fileName = "LevelReward_")]
public class LevelReward : QuestReward
{
    public override void Give(Quest quest)
    {
        DataManager.Instance.PlayerStatus.Level += Quantity;
    }
}
