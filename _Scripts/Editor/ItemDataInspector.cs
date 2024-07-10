using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * File     : ItemDataInspector.cs
 * Desc     : ItemData¿« Custom Inspecter
 * Date     : 2024-06-16
 * Writer   : ¡§¡ˆ»∆
 */

[CustomEditor(typeof(ItemData), true)]
public class ItemDataInspector : Editor
{
    private ItemData _itemData;

    private void OnEnable()
    {
        _itemData = (ItemData)target;
    }

    public override void OnInspectorGUI()
    {
        if (_itemData == null)
        {
            return;
        }

        base.OnInspectorGUI();

        if (_itemData.CanSellable)
        {
            _itemData.ItemPrice = EditorGUILayout.IntField("ItemPrice", _itemData.ItemPrice);
            _itemData.SalePrice = EditorGUILayout.IntField("SalePrice", (int)_itemData.SalePrice);
        }
    }
}
