using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ExpReward.cs
 * Desc     : 퀘스트 성공하면 받는 경험치
 * Date     : 2024-06-18
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Reward/ExpReward", fileName = "ExpReward_")]
public class ExpReward : QuestReward
{
    public override void Give(Quest quest)
    {
        DataManager.Instance.PlayerStatus.CurrentExp += Quantity;
    }
}
