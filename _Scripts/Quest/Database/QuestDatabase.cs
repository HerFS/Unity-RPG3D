using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * File     : QuestDatabase.cs
 * Desc     : ScriptableObject
 *            퀘스트, 업적들의 데이터베이스
 * Date     : 2024-05-06
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Quest/Database")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> _quests;

    public IReadOnlyList<Quest> Quests => _quests;

    public Quest FindQuestBy(string codeName) => _quests.FirstOrDefault(x => (x.CodeName == codeName));

#if UNITY_EDITOR
    [ContextMenu("Find Quests")]
    private void FindQuests()
    {
        FindQuestsBy<Quest>();
    }

    [ContextMenu("Find Achievements")]
    private void FindAchievements()
    {
        FindQuestsBy<Achievement>();
    }

    private void FindQuestsBy<T>() where T : Quest
    {
        _quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(path);

            if (quest.GetType() == typeof(T))
            {
                _quests.Add(quest);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
