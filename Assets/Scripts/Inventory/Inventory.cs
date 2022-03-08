using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public delegate void onItemChanged();
    public onItemChanged onItemChangedCallback;


    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    public Dictionary<Item, int> hotbar = new Dictionary<Item, int>();
    public int hotbarSpace = 8;
    public Item chosenItem;
    public int space = 20;

    public Item ChosenItem
    {
        get
        {
        return chosenItem;
        }
        set {
            chosenItem = value;
        }
    }

    public void ChooseItem(int inventorySlot)
    {
        chosenItem = hotbar.Keys.ToArray()[inventorySlot];
    }
    public void MoveItemToDisplayed(Item item, int slot)
    {
        if (0 <= slot && slot < hotbar.Count && items.ContainsKey(item))
        {
            /*for (int i = 0; i < hotbar.Count; ++i)
            {
                if (displayedItems[i] == item)
                {
                    displayedItems[i] = null;
                }
            }
            displayedItems[slot] = item;*/
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
            if (items.Count > space) return false;
            items.Add(item, amount);
            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
            /*for (int i = 0; i < displayedItems.Length; ++i)
            {
                if (displayedItems[i] == null)
                {
                    displayedItems[i] = item;
                    break;
                }
            }*/
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
                if (onItemChangedCallback != null)
                {
                    onItemChangedCallback.Invoke();
                }
                /*for (int i = 0; i < displayedItems.Length; ++i)
                {
                    if (displayedItems[i] == item)
                    {
                        displayedItems[i] = null;
                    }
                }*/
            }
        } 
        else
        {
            return false;
        }
        return true;
    }
    /*public void SwitchHotbarInventory(Item item, int amount)
    {
        //inventory to hotbar, CHECK if we have enaugh space
        foreach (Item i in items.Keys)
        {
            if (i == item)
            {
                if (hotbar.Count >= hotbarSpace)
                {
                    Debug.Log("No more slots available in hotbar");
                    return;
                }
                else
                {
                    hotbar.Add(item, amount);
                    RemoveFromInventory(item, amount);
                    if (onItemChangedCallback != null)
                    {
                        onItemChangedCallback.Invoke();
                    }
                }
            }
        }
        //hotbar to inventory
        foreach (Item i in hotbar.Keys)
        {
            if (i == item)
            {
                if (items.Count >= space)
                {
                    Debug.Log("No more slots available in hotbar");
                    return;
                }
                hotbar.Remove(item);
                items.Add(item, amount);
                if (onItemChangedCallback != null)
                {
                    onItemChangedCallback.Invoke();
                }
                return;
            }
        }
    }*/
}
