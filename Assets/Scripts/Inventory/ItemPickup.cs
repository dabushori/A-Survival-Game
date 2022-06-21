using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used for destroying game objects and recieving their
 * drop items into the inventory
 */ 
public class ItemPickup : MonoBehaviour
{
    public Item item; // the item we will get
    public int amount; // the amount of the item

    /*
     * adding the item to the inventory
     */
    public void PickUp()
    {
        if (Inventory.Instance.AddToInventory(item, amount)) Destroy(gameObject);
    }
}
