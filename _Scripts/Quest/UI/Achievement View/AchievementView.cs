using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : AchivementView.cs
 * Desc     : �������� UI�� ����
 * Date     : 2024-06-20
 * Writer   : ������
 */

public class AchievementView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _achievementGroup;
    [SerializeField]
    private AchievementDetailView _achievementDetailViewPrefab;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        CreateDetatilView(questSystem.ActiveAchievements);
        CreateDetatilView(questSystem.CompletedAchievements);
    }

    private void CreateDetatilView(IReadOnlyList<Quest> achievements)
    {
        foreach (var achievement in achievements)
        {
            Instantiate(_achievementDetailViewPrefab, _achievementGroup).Setup(achievement);
        }
    }
}
