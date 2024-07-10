using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Reward.cs
 * Desc     : ScriptableObject
 *            퀘스트 성공하면 받는 보상
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public abstract class QuestReward : ScriptableObject
{
    [SerializeField, TextArea]
    private string _description;
    [SerializeField]
    private uint _quantity;

    public string Description => _description;
    public uint Quantity => _quantity;

    public abstract void Give(Quest quest);
}
