using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Condition.cs
 * Desc     : ScriptableObject
 *            퀘스트 수락, 취소 조건
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public abstract class Condition : ScriptableObject
{
    [SerializeField, TextArea]
    private string _descirption;

    public string Description => _descirption;
    public abstract bool IsPass(Quest quest);
}
