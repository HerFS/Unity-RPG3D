using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : QuestView.cs
 * Desc     : 퀘스트들을 UI에 생성
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

public class QuestView : MonoBehaviour
{
    [SerializeField]
    private QuestListViewController _questListViewController;
    [SerializeField]
    private QuestDetailView _questDetailView;

    private void OnEnable()
    {
        if (_questDetailView.Target != null)
        {
            _questDetailView.Show(_questDetailView.Target);
        }
    }

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        foreach (var quest in questSystem.ActiveQuests)
        {
            AddQuestToActiveListView(quest);
        }

        foreach (var quest in questSystem.CompletedQuests)
        {
            AddQuestToCompletedListView(quest);
        }

        questSystem.onQuestRegistered += AddQuestToActiveListView;
        questSystem.onQuestCompleted += RemoveQuestFromActiveListView;
        questSystem.onQuestCompleted += AddQuestToCompletedListView;
        questSystem.onQuestCompleted += HideDetailIfQuestCanceled;
        questSystem.onQuestCanceled += HideDetailIfQuestCanceled;
        questSystem.onQuestCanceled += RemoveQuestFromActiveListView;

        foreach (var tab in _questListViewController.Tabs)
        {
            tab.onValueChanged.AddListener(HideDetail);
        }
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;

        if (questSystem)
        {
            questSystem.onQuestRegistered -= AddQuestToActiveListView;
            questSystem.onQuestCompleted -= RemoveQuestFromActiveListView;
            questSystem.onQuestCompleted -= AddQuestToCompletedListView;
            questSystem.onQuestCompleted -= HideDetailIfQuestCanceled;
            questSystem.onQuestCanceled -= HideDetailIfQuestCanceled;
            questSystem.onQuestCanceled -= RemoveQuestFromActiveListView;
        }
    }

    private void ShowDetail(bool isOn, Quest quest)
    {
        if (isOn)
        {
            _questDetailView.Show(quest);
        }
    }

    private void HideDetail(bool isOn)
    {
        _questDetailView.Hide();
    }

    private void AddQuestToActiveListView(Quest quest)
        => _questListViewController.AddQuestToActiveListView(quest, isOn => ShowDetail(isOn, quest));

    private void AddQuestToCompletedListView(Quest quest)
        => _questListViewController.AddQuestToCompletedListView(quest, isOn => ShowDetail(isOn, quest));

    private void HideDetailIfQuestCanceled(Quest quest)
    {
        if (_questDetailView.Target == quest)
        {
            _questDetailView.Hide();
        }
    }

    private void RemoveQuestFromActiveListView(Quest quest)
    {
        _questListViewController.RemoveQuestFromActiveListView(quest);

        if (_questDetailView.Target == quest)
        {
            _questDetailView.Hide();
        }
    }
}
