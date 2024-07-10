using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using TMPro;

/*
 * File     : NpcEntityInspector.cs
 * Desc     : NpcEntity¿« Custom Inspecter
 * Date     : 2024-06-18
 * Writer   : ¡§¡ˆ»∆
 */

[CustomEditor(typeof(NpcEntity))]
public class NpcEntityInspector : Editor
{
    private NpcEntity _npcEntity;

    private void OnEnable()
    {
        _npcEntity = (NpcEntity)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_npcEntity == null)
        {
            return;
        }

        switch (_npcEntity.NpcType)
        {
            case EnumTypes.NpcType.Quest:
                var inactiveQuests = serializedObject.FindProperty("InactiveQuests");
                serializedObject.Update();
                EditorGUILayout.PropertyField(inactiveQuests, true);
                serializedObject.ApplyModifiedProperties();
                _npcEntity.QuestMarker = (QuestMarker)EditorGUILayout.ObjectField("QuestMarker", _npcEntity.QuestMarker, typeof(QuestMarker), true);
                break;
            case EnumTypes.NpcType.Shop:
                _npcEntity.ShopName = EditorGUILayout.TextField("ShopName", _npcEntity.ShopName);
                var itemData = serializedObject.FindProperty("ShopItemData");
                serializedObject.Update();
                EditorGUILayout.PropertyField(itemData, true);
                serializedObject.ApplyModifiedProperties();
                break;
        }
    }
}
