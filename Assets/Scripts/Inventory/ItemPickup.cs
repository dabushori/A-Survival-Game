using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public int amount;
    void Update()
    {
    }
    public void PickUp()
    {
        if (Inventory.instance.AddToInventory(item, amount))
        {
            Destroy(gameObject);
        }

    }
}
