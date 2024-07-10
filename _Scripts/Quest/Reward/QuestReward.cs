using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Reward.cs
 * Desc     : ScriptableObject
 *            ����Ʈ �����ϸ� �޴� ����
 * Date     : 2024-05-06
 * Writer   : ������
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
