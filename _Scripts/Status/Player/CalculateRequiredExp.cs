using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : CalculateRequiredExp.cs
 * Desc     : ScriptableObject
 *            ������ ������ �ʿ��� ����ġ ��� ��. ���� : https://oldschool.runescape.wiki/w/Experience
 * Date     : 2024-05-05
 * Writer   : ������
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Status/Calculate Exp" , fileName = "Calculate Exp")]
public class CalculateRequiredExp : ScriptableObject
{
    [Header("Multipliers Exp")]
    [Range(1f, 300f)]
    private float _additionMultiplier = 300f;
    [Range(1f, 300f)]
    private float _powerMultiplier = 2f;
    [Range(1f, 300f)]
    private float _divisionMultiplier = 7f;

    public float AdditionMultiplier
    {
        get { return _additionMultiplier; }
        private set { _additionMultiplier = value; }
    }
    public float PowerMultiplier
    {
        get { return _powerMultiplier; }
        private set { _powerMultiplier = value; }
    }
    public float DivisionMultiplier
    {
        get { return _divisionMultiplier; }
        private set { _divisionMultiplier = value; }
    }
}
