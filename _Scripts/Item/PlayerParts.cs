using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PlayerParts.cs
 * Desc     : Player가 입을 수 있는 아이템 파츠
 * Date     : 2024-05-30
 * Writer   : 정지훈
 */

public class PlayerParts : MonoBehaviour
{
    public EnumTypes.EquipmentType EquipmentType;

    [SerializeField]
    private int _partsId;
    public int PartsId { get { return _partsId; } }
}
