using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

/*
 * File     : QuestDatabase.cs
 * Desc     : ScriptableObject
 *            퀘스트, 업적들의 데이터베이스
 * Date     : 2024-06-20
 * Writer   : 정지훈
 */

public class QuestSystem : MonoBehaviour
{
    #region Save Path
    private readonly string _questDataFilePath = Application.dataPath + "/Resources/QuestData.json";
    private const string _activeQuestsSavePath = "ActiveQuests";
    private const string _completedQuestsSavePath = "CompletedQuests";
    private const string _activeAchievementsSavePath = "ActiveAchievements";
    private const string _completedAchievementsSavePath = "CompletedAchievements";
    #endregion

    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    public delegate void QuestStateChangedHandler();    
    #endregion

    private static QuestSystem _instance;
    private static bool _isApplicationQuitting;

    public static QuestSystem Instance
    {
        get
        {
            if (!_isApplicationQuitting && _instance == null)
            {
                _instance = FindAnyObjectByType<QuestSystem>();

                if (_instance == null)
                {
                    _instance = new GameObject("Quest System").AddComponent<QuestSystem>();
                }
            }

            return _instance;
        }
    }

    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();

    private List<Quest> _activeAchievements = new List<Quest>();
    private List<Quest> _completedAchievements = new List<Quest>();

    private QuestDatabase _questDatabase;
    private QuestDatabase _achievementDatabase;

    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;
    public event QuestStateChangedHandler onQuestStateChanged;

    public event QuestRegisteredHandler onAchievementRegistered;
    public event QuestCompletedHandler onAchievementCompleted;

    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;
    public IReadOnlyList<Quest> ActiveAchievements => _activeAchievements;
    public IReadOnlyList<Quest> CompletedAchievements => _completedAchievements;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            if (transform.root != null && transform.parent != null)
            {
                DontDestroyOnLoad(this.transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject, Time.deltaTime);
        }

        _questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        _achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase");

        if (!Load())
        {
            foreach (var achievement in _achievementDatabase.Quests)
            {
                Register(achievement);
            }
        }
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
        Save();
    }

    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();

        if (newQuest is Achievement)
        {
            newQuest.onCompleted += OnAchievementCompleted;

            _activeAchievements.Add(newQuest);
            newQuest.OnRegister();
            onAchievementRegistered?.Invoke(newQuest);
        }
        else
        {
            newQuest.onCompleted += OnQuestCompleted;
            newQuest.onCanceled += OnQuestCanceled;

            _activeQuests.Add(newQuest);
            newQuest.OnRegister();
            onQuestRegistered?.Invoke(newQuest);
        }

        return newQuest;
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(_activeQuests, category, target, successCount);
        ReceiveReport(_activeAchievements, category, target, successCount);
    }

    public void ReceiveReport(Category category, TaskTarget target, int successCount)
        => ReceiveReport(category.CodeName, target.Target, successCount);

    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        foreach (var quest in quests.ToArray())
        {
            quest.onStateChanged += OnQuestStateChanged;
            quest.ReceiveReport(category, target, successCount);
        }
    }

    public bool ContainsInActivedQuests(Quest quest) => _activeQuests.Any(x => (x.CodeName == quest.CodeName));
    public bool ContainsInCompletedQuests(Quest quest) => _completedQuests.Any(x => (x.CodeName == quest.CodeName));
    public bool ContainsInActivedAchievements(Quest quest) => _activeAchievements.Any(x => (x.CodeName == quest.CodeName));
    public bool ContainsInCompletedAchievements(Quest quest) => _completedAchievements.Any(x => (x.CodeName == quest.CodeName));

    private void Save()
    {
        var root = new JObject();

        root.Add(_activeQuestsSavePath, CreateSaveDatas(_activeQuests));
        root.Add(_completedQuestsSavePath, CreateSaveDatas(_completedQuests));
        root.Add(_activeAchievementsSavePath, CreateSaveDatas(_activeAchievements));
        root.Add(_completedAchievementsSavePath, CreateSaveDatas(_completedAchievements));

        File.WriteAllText(_questDataFilePath, root.ToString());
    }

    private bool Load()
    {
        if (File.Exists(_questDataFilePath))
        {
            var root = File.ReadAllText(_questDataFilePath);
            var test = JObject.Parse(root);
            
            LoadSaveData(test[_activeQuestsSavePath], _questDatabase, LoadActiveQuest);
            LoadSaveData(test[_completedQuestsSavePath], _questDatabase, LoadCompletedQuests);

            LoadSaveData(test[_activeAchievementsSavePath], _achievementDatabase, LoadActiveQuest);
            LoadSaveData(test[_completedAchievementsSavePath], _achievementDatabase, LoadCompletedQuests);

            return true;
        }
        else
        {
            return false;
        }
    }

    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach (var quest in quests)
        {
            if (quest.IsSavable)
            {
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));
            }
        }

        return saveDatas;
    }

    private void LoadSaveData(JToken datasToken, QuestDatabase database, System.Action<QuestSaveData, Quest> onSuccess)
    {
        var datas = datasToken as JArray;
        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveData.CodeName);
            onSuccess.Invoke(saveData, quest);
        }
    }

    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }

    private void LoadCompletedQuests(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        if (newQuest is Achievement)
        {
            _completedAchievements.Add(newQuest);
        }
        else
        {
            _completedQuests.Add(newQuest);
        }
    }

    #region Callback

    private void OnQuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        _activeQuests.Remove(quest);

        onQuestCanceled?.Invoke(quest);
        Destroy(quest, Time.deltaTime);
    }

    private void OnAchievementCompleted(Quest achievement)
    {
        _activeAchievements.Remove(achievement);
        _completedAchievements.Add(achievement);

        onAchievementCompleted?.Invoke(achievement);
    }

    private void OnQuestStateChanged()
    {
        onQuestStateChanged?.Invoke();
    }
    #endregion
}
