using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public int amount;

    public void PickUp()
    {
        if (Inventory.Instance.AddToInventory(item, amount)) Destroy(gameObject);
    }
}
