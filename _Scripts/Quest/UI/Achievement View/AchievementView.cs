using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : AchivementView.cs
 * Desc     : 업적들을 UI에 생성
 * Date     : 2024-06-20
 * Writer   : 정지훈
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
