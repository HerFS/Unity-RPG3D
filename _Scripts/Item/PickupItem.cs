using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File     : PickupItem.cs
 * Desc     : 플레이어가 획득할 수 있는 아이템
 * Date     : 2024-06-30
 * Writer   : 정지훈
 */

public class PickupItem : MonoBehaviour
{
    private readonly float _turnSpeed = 15f;

    public ItemData Item;
    public uint ItemQuantity;
    [HideInInspector]
    public EnumTypes.ItemType ItemType;
    public uint MoneyValue;
    

    private void Update()
    {
        transform.Rotate(transform.rotation.x, Time.deltaTime * _turnSpeed, transform.rotation.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Contains(Globals.TagName.Player))
        {
            if (InputManager.Instance.IsPickup)
            {
                if (DataManager.Instance.Inventory.AddItem(Item, ref ItemQuantity, this) == 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

}
