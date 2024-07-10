using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * File     : QuestReporter.cs
 * Desc     : 퀘스트 성공값을 전달함
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

public class QuestReporter : MonoBehaviour
{
    [SerializeField]
    private Category _category;
    [SerializeField]
    private TaskTarget _target;
    [SerializeField]
    private int _successCount;
    [SerializeField]
    private string[] _colliderTags;

    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReportIfPassCondition(collision);
    }

    public void Report()
    {
        QuestSystem.Instance.ReceiveReport(_category, _target, _successCount);
    }

    private void ReportIfPassCondition(Component other)
    {
        if (_colliderTags.Any(x => other.CompareTag(x)))
        {
            Report();
        }
    }
}
