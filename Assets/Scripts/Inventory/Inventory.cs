using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    private static Inventory instance = new Inventory();
    private Inventory() { }
    

    public static Inventory Instance
    {
        get
        {
            return instance;
        }
    }

    public delegate void onItemChanged();
    public onItemChanged onItemChangedCallback;

    public InventorySlot[] inventorySlots;
    public InventorySlot[] hotbarSlots;

    public void setSlots(InventorySlot[] inventorySlots, InventorySlot[] hotbarSlots)
    {
        this.inventorySlots = inventorySlots;
        this.hotbarSlots = hotbarSlots;
        Debug.Log("init");
    }

    // public Dictionary<Item, int> items = new Dictionary<Item, int>();
    // public Dictionary<Item, int> hotbar = new Dictionary<Item, int>();

    public int hotbarSpace = 8;
    private int chosenItemIdx = 0;
    public int space = 20;

    public Item ChosenItem
    {
        get
        {
            return hotbarSlots[chosenItemIdx].Item;
        }
    }

    public void ChooseItem(int slotIdx)
    {
        chosenItemIdx = slotIdx;
    }

    public bool AddToInventory(Item item, int amount)
    {
        if (amount < 0) return false;
        if (amount == 0) return true;
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == item)
            {
                slot.IncAmount(amount);
                onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in inventorySlots) {
            if (slot.Item == item)
            {
                slot.IncAmount(amount);
                onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == null)
            {
                slot.AddItem(item, amount);
                onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.Item == null)
            {
                slot.AddItem(item, amount);
                onItemChangedCallback?.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool RemoveFromInventory(Item item, int amount)
    {
        if (amount < 0) return false;
        if (amount == 0) return true;
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                onItemChangedCallback?.Invoke();
                return true;
            }
        }
        return false;
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
