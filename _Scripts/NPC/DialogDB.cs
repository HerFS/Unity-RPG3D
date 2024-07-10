using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : DialogDB.cs
 * Desc     : ScriptableObject
 *            Dialog Exel파일에서 데이터를 얻어옴
 * Date     : 2024-06-10
 * Writer   : 정지훈
 */

[ExcelAsset]
public class DialogDB : ScriptableObject
{
    public List<DialogDBEntity> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
