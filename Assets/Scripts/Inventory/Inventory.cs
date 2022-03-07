using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    private Dictionary<Item, int> items;

    private Item[] displayedItems;

    private int maxItemsAmount;

    private int currentItem;

    public Inventory(int maxItemsAmount)
    {
        items = new Dictionary<Item, int>();
        this.maxItemsAmount = maxItemsAmount;
        displayedItems = new Item[10];
        AddToInventory(new Pickaxe(), 1);
    }

    public void MoveItemToDisplayed(Item item, int slot)
    {
        if (0 <=slot && slot < displayedItems.Length && items.ContainsKey(item))
        {
            for (int i = 0; i < displayedItems.Length; ++i)
            {
                if (displayedItems[i] == item)
                {
                    displayedItems[i] = null;
                }
            }
            displayedItems[slot] = item;
        }
    }

    public bool AddToInventory(Item item, int amount)
    {
        if (amount <= 0) return false;
        if (items.ContainsKey(item))
        {
            items[item] += amount;
        } else
        {
            if (items.Count > maxItemsAmount) return false;
            items.Add(item, amount);
            for (int i = 0; i < displayedItems.Length; ++i)
            {
                if (displayedItems[i] == null)
                {
                    displayedItems[i] = item;
                    break;
                }
            }
        }
        return true;
    }

    public bool RemoveFromInventory(Item item, int amount)
    {
        if (items.ContainsKey(item))
        {
            if (items[item] < amount) return false;
            else if (items[item] > amount)
            {
                items[item] -= amount;
            } 
            else
            {
                items.Remove(item);
                for (int i = 0; i < displayedItems.Length; ++i)
                {
                    if (displayedItems[i] == item)
                    {
                        displayedItems[i] = null;
                    }
                }
            }
        } 
        else
        {
            return false;
        }
        return true;
    }

    public void ChooseItem(int index)
    {
        if (index >= 0 && index < maxItemsAmount) currentItem = index;
    }

    public Item ChosenItem
    {
        get
        {
            return displayedItems[currentItem];
        }
    }

    public Item[] DisplayedItems
    {
        get
        {
            return displayedItems;
        }
    }
}
