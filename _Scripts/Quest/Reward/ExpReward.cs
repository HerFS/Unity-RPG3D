using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : ExpReward.cs
 * Desc     : ����Ʈ �����ϸ� �޴� ����ġ
 * Date     : 2024-06-18
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Reward/ExpReward", fileName = "ExpReward_")]
public class ExpReward : QuestReward
{
    public override void Give(Quest quest)
    {
        DataManager.Instance.PlayerStatus.CurrentExp += Quantity;
    }
}
