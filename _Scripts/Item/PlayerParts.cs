using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PlayerParts.cs
 * Desc     : Player�� ���� �� �ִ� ������ ����
 * Date     : 2024-05-30
 * Writer   : ������
 */

public class PlayerParts : MonoBehaviour
{
    public EnumTypes.EquipmentType EquipmentType;

    [SerializeField]
    private int _partsId;
    public int PartsId { get { return _partsId; } }
}
