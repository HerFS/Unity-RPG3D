using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * File     : MonsterInspector.cs
 * Desc     : Monster¿« Custom Inspecter
 * Date     : 2024-06-16
 * Writer   : ¡§¡ˆ»∆
 */

[CustomEditor(typeof(MonsterStatus))]
public class MonsterInspector : Editor
{
    private MonsterStatus _monsterStatus;

    private void OnEnable()
    {
        _monsterStatus = (MonsterStatus)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_monsterStatus == null)
        {
            return;
        }

        switch (_monsterStatus.MonsterAttackType)
        {
            case EnumTypes.MonsterAttackType.MeleeAttak:
                _monsterStatus.WeaponCollider = (BoxCollider)EditorGUILayout.ObjectField("Weapon Collider", _monsterStatus.WeaponCollider, typeof(BoxCollider), true);
                break;
            case EnumTypes.MonsterAttackType.RangeAttak:
                _monsterStatus.BulletSpwanTransform = (Transform)EditorGUILayout.ObjectField("Bullet Spawn Pos", _monsterStatus.BulletSpwanTransform, typeof(Transform), true);
                break;
        }
    }
}
