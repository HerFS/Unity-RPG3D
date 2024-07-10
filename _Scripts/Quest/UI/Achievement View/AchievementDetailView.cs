using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File     : AchievmentView.cs
 * Desc     : 업적들의 설명을 보여줌
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

public class AchievementDetailView : MonoBehaviour
{
    private Quest _target;
    private StringBuilder _stringBuilder = new StringBuilder();

    [SerializeField]
    private Image _achievementIcon;
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _description;
    [SerializeField]
    private TextMeshProUGUI _rewardText;
    [SerializeField]
    private GameObject _completionScreen;

    private void OnDestroy()
    {
        if (_target != null)
        {
            _target.onTaskSuccessChanged -= UpdateDescription;
            _target.onCompleted -= ShowCompletionScreen;
        }
    }

    public void Setup(Quest achievement)
    {
        _target = achievement;

        _achievementIcon.sprite = achievement.Icon;
        _titleText.text = achievement.DisplayName;

        var task = achievement.CurrentTaskGroup.Tasks[0];
        _description.text = BuildTaskDescription(task);

        foreach(var reward in achievement.Rewards)
        {
            _rewardText.color = new Color(1f, 1f, 0f);
            _stringBuilder.Append($"{reward.Description} +{reward.Quantity}\n");
        }
        _rewardText.text = _stringBuilder.ToString();
        _stringBuilder.Clear();

        if (achievement.IsComplete)
        {
            _completionScreen.SetActive(true);
        }
        else
        {
            _completionScreen.SetActive(false);
            achievement.onTaskSuccessChanged += UpdateDescription;
            achievement.onCompleted += ShowCompletionScreen;
        }
    }

    private void UpdateDescription(Quest achievement, Task task, int currentSuccess, int prevSuccess)
        => _description.text = BuildTaskDescription(task);

    private void ShowCompletionScreen(Quest achievement)
        => _completionScreen.SetActive(true);

    private string BuildTaskDescription(Task task) => $"- {task.Description} {task.CurrentSuccess} / {task.NeedSuccessToComplete}";
}
