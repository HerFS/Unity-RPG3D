using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : InitialStatusSetup.cs
 * Desc     : ScriptableObject
 *            초기 스탯 값들 세팅
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

[CreateAssetMenu(menuName = "ScriptableObjects/Status/Initial Status", fileName = "InitialStatus_")]
public class InitialStatusSetup : ScriptableObject
{
    [SerializeField]
    private string _name;
    [SerializeField, Range(1, 30)]
    private uint _level;
    [SerializeField, Range(0f, 5f)]
    private float _walkSpeed;

    [Tooltip("MaxHp = InitialHp * Level * 1.5f")]
    [SerializeField]
    private float _initialHp;

    [Header("Damage")]
    [SerializeField]
    private float _attackDamage;
    [SerializeField]
    private float _criticalChance;

    [Header("Defense")]
    [SerializeField]
    private float _defense;

    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    public uint Level
    {
        get { return _level; }
        private set { _level = value; }
    }

    public float WalkSpeed
    {
        get { return _walkSpeed; }
        private set { _walkSpeed = value; }
    }

    public float InitialHp
    {
        get { return _initialHp; }
        private set { _initialHp = value; }
    }

    public float AttackDamage
    {
        get { return _attackDamage; }
        private set { _attackDamage = value; }
    }

    public float CriticalChance
    {
        get { return _criticalChance; }
        private set { _criticalChance = value; }
    }

    public float Defense
    {
        get { return _defense; }
        private set { _defense = value; }
    }
}
