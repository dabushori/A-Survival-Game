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
        // Debug.Log(hotbarSlots);
        // Debug.Log(inventorySlots);
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

    public InventorySlot selectedSlot;
    // switch between selectedSlot and newSlot
    public void SwitchItems(InventorySlot newSlot)
    {
        Item i1 = selectedSlot.Item;
        int amount1 = selectedSlot.Amount;

        Item i2 = newSlot.Item;
        int amount2 = newSlot.Amount;

        if (i2 != null && amount2 > 0) selectedSlot.AddItem(i2, amount2);
        else selectedSlot.ClearSlot();
        if (i1 != null && amount1 > 0) newSlot.AddItem(i1, amount1);
        else newSlot.ClearSlot();
    }
}
