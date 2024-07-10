using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
 * File     : QuestListViewController.cs
 * Desc     : Äù½ºÆ® ¸®½ºÆ® ºäµéÀ» Á¦¾î
 * Date     : 2024-06-19
 * Writer   : Á¤ÁöÈÆ
 */

public class QuestListViewController : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup _tabGroup;
    [SerializeField]
    private QuestListView _activeQuestListView;
    [SerializeField]
    private QuestListView _completedQuestListView;

    public IEnumerable<Toggle> Tabs => _tabGroup.ActiveToggles();

    public void AddQuestToActiveListView(Quest quest, UnityAction<bool> onClicked)
        => _activeQuestListView.AddElement(quest, onClicked);

    public void AddQuestToCompletedListView(Quest quest, UnityAction<bool> onClicked)
    => _completedQuestListView.AddElement(quest, onClicked);

    public void RemoveQuestFromActiveListView(Quest quest)
        => _activeQuestListView.RemoveElement(quest);
}
