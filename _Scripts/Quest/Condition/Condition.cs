using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Condition.cs
 * Desc     : ScriptableObject
 *            ����Ʈ ����, ��� ����
 * Date     : 2024-05-06
 * Writer   : ������
 */

public abstract class Condition : ScriptableObject
{
    [SerializeField, TextArea]
    private string _descirption;

    public string Description => _descirption;
    public abstract bool IsPass(Quest quest);
}
