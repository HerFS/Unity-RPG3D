using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : DialogDB.cs
 * Desc     : ScriptableObject
 *            Dialog Exel���Ͽ��� �����͸� ����
 * Date     : 2024-06-10
 * Writer   : ������
 */

[ExcelAsset]
public class DialogDB : ScriptableObject
{
    public List<DialogDBEntity> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
