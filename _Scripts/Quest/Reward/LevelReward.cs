using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : LevelReward.cs
 * Desc     : 퀘스트 성공하면 받는 레벨
 * Date     : 2024-06-18
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Reward/LevelReward", fileName = "LevelReward_")]
public class LevelReward : QuestReward
{
    public override void Give(Quest quest)
    {
        DataManager.Instance.PlayerStatus.Level += Quantity;
    }
}
