using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    [SerializeField]
    private Dictionary<Item, int> items;

    [SerializeField]
    private int maxItemsAmount;

    public bool AddToInventory(Item i, int amount)
    {
        if (amount <= 0) return false;
        if (items.ContainsKey(i))
        {
            items[i] += amount;
        } else
        {
            if (items.Count > maxItemsAmount) return false;
            items.Add(i, amount);
        }
        return true;
    }

    public bool RemoveFromInventory(Item i, int amount)
    {
        if (items.ContainsKey(i))
        {
            if (items[i] < amount) return false;
            else if (items[i] > amount)
            {
                items[i] -= amount;
            } else
            {
                items.Remove(i);
            }
        } 
        else
        {
            return false;
        }
        return true;
    }
}
