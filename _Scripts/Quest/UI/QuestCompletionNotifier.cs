using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

/*
 * File     : QuestcompletionNotifier.cs
 * Desc     : 퀘스트 완료 시 받는 보상 UI
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

public class QuestCompletionNotifier : MonoBehaviour
{
    private readonly float _showTime = 3f;

    [SerializeField]
    private string _titleDescription;

    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _rewardText;

    private Queue<Quest> _reservedQuests = new Queue<Quest>();
    private StringBuilder _stringBuilder = new StringBuilder();

    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        questSystem.onQuestCompleted += Notify;
        questSystem.onAchievementCompleted += Notify;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;
        if (questSystem != null)
        {
            questSystem.onQuestCompleted -= Notify;
            questSystem.onAchievementCompleted -= Notify;
        }
    }

    private void Notify(Quest quest)
    {
        _reservedQuests.Enqueue(quest);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine("ShowNotice");
        }
    }

    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(_showTime);

        Quest quest;
        while (_reservedQuests.TryDequeue(out quest))
        {
            _titleText.text = _titleDescription.Replace("%{dn}", quest.DisplayName);

            foreach (var reward in quest.Rewards)
            {
                _stringBuilder.Append(reward.Description);
                _stringBuilder.Append(" +");
                _stringBuilder.Append(reward.Quantity);
                _stringBuilder.Append("\n");
            }
            _rewardText.text = _stringBuilder.ToString();
            _stringBuilder.Clear();

            yield return waitSeconds;
        }

        gameObject.SetActive(false);
    }
}
