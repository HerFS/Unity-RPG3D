using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : Eqipment.cs
 * Desc     : ������ �� �ִ� �����۵�� ���Ե�
 * Date     : 2024-05-30
 * Writer   : ������
 */

public class Equipment : DontDestroyObject<Equipment>
{
    public EquipmentSlot[] EquipmentSlots;
    public EquipmentSlot WeaponSlot;

    public List<PlayerParts> HelmetParts;
    public List<PlayerParts> TopParts;
    public List<PlayerParts> BottomParts;
    public List<PlayerParts> ShoeParts;
    public List<PlayerParts> GauntletParts;
    public List<PlayerParts> CloakParts;
    public List<PlayerParts> WeaponParts;

    protected override void Awake()
    {
        base.Awake();
    }
}
