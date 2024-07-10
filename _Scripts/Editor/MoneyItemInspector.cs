using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * File     : MoneyItemInspector.cs
 * Desc     : MoneyItem¿« Custom Inspecter
 * Date     : 2024-06-16
 * Writer   : ¡§¡ˆ»∆
 */

[CustomEditor(typeof(DefaultItemData))]
public class MoneyItemInspector : Editor
{
    private DefaultItemData _defaultData;

    private void OnEnable()
    {
        _defaultData = (DefaultItemData)target;
    }

    public override void OnInspectorGUI()
    {
        if (_defaultData == null)
        {
            return;
        }

        if (_defaultData.IsMoney)
        {
            _defaultData.Id = EditorGUILayout.IntField("Id", _defaultData.Id);
            _defaultData.DropItemPrefab = (GameObject)EditorGUILayout.ObjectField("DropItemPrefab", _defaultData.DropItemPrefab, typeof(GameObject), true);
            _defaultData.IsMoney = EditorGUILayout.Toggle("Is Money", _defaultData.IsMoney);
        }
        else
        {
            base.OnInspectorGUI();
        }
    }
}
