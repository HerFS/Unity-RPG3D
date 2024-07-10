using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * File     : QuestTrackerView.cs
 * Desc     : 대화 시스템
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class QuestTrackerView : MonoBehaviour
{
    public Category[] Categorys;
    [SerializeField]
    private QuestTracker _questTrackerPrefab;
    [SerializeField]
    private QuestDetailView _questDetailView;

    private void Start()
    {
        QuestSystem.Instance.onQuestRegistered += CreateQuestTracker;

        foreach (var quest in QuestSystem.Instance.ActiveQuests)
        {
            CreateQuestTracker(quest);
        }

        _questDetailView.AddQuestTracker();
    }

    private void OnDestroy()
    {
        if (QuestSystem.Instance)
        {
            QuestSystem.Instance.onQuestRegistered -= CreateQuestTracker;
        }
    }

    private void CreateQuestTracker(Quest quest)
    {
        var categoryColor = Categorys.FirstOrDefault(x => x == quest.Category);
        var color = categoryColor == null ? Color.white : categoryColor.TitleColor;
        Instantiate(_questTrackerPrefab, this.transform).Setup(quest, color);
    }
}
