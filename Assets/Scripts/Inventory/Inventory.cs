using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;
    [SerializeField] InventorySlot[] armorSlots;

    public void setSlots(InventorySlot[] inventorySlots, InventorySlot[] hotbarSlots, InventorySlot[] armorSlots)
    {
        this.inventorySlots = inventorySlots;
        this.hotbarSlots = hotbarSlots;
        this.armorSlots = armorSlots;

        SetSelectedItemColor();
    }

    [SerializeField]
    int hotbarSpace = 8;
    int chosenItemIdx = 0;

    public Item ChosenItem
    {
        get
        {
            return hotbarSlots[chosenItemIdx].Item;
        }
    }

    private void ResetSelectedItemColor()
    {
        hotbarSlots[chosenItemIdx].gameObject.GetComponentInChildren<Image>().color = Color.white;
    }

    private void SetSelectedItemColor()
    {
        hotbarSlots[chosenItemIdx].gameObject.GetComponentInChildren<Image>().color = new Color(0.769f, 0.769f, 0.769f, 0.5f);
        // hotbarSlots[chosenItemIdx].gameObject.GetComponentInChildren<Image>().color = Color.gray;
    }

    public void ChooseItem(int slotIdx)
    {
        ResetSelectedItemColor();
        chosenItemIdx = slotIdx;
        SetSelectedItemColor();
    }

    public void ChooseNextItem()
    {
        ChooseItem((chosenItemIdx + 1) % hotbarSpace);
    }

    public void ChoosePrevItem()
    {
        ChooseItem((chosenItemIdx + hotbarSpace - 1) % hotbarSpace);
    }

    public int GetAmountOfItem(Item item)
    {
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == item)
            {
                return slot.Amount;
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.Item == item)
            {
                return slot.Amount;
            }
        }
        return 0;
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
                //onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in inventorySlots) {
            if (slot.Item == item)
            {
                slot.IncAmount(amount);
                //onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == null)
            {
                slot.AddItem(item, amount);
                //onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.Item == null)
            {
                slot.AddItem(item, amount);
                //onItemChangedCallback?.Invoke();
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
                //onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                //onItemChangedCallback?.Invoke();
                return true;
            }
        }
        foreach (InventorySlot slot in armorSlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                //onItemChangedCallback?.Invoke();
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

    public void SwitchItems(InventorySlot srcSlot, InventorySlot dstSlot)
    {
        Item i1 = srcSlot.Item;
        int amount1 = srcSlot.Amount;

        Item i2 = dstSlot.Item;
        int amount2 = dstSlot.Amount;

        if (i2 != null && amount2 > 0) srcSlot.AddItem(i2, amount2);
        else srcSlot.ClearSlot();
        if (i1 != null && amount1 > 0) dstSlot.AddItem(i1, amount1);
        else dstSlot.ClearSlot();
    }

    public void WearArmor()
    {
        if (ChosenItem != null && ChosenItem.job == Jobs.ARMOR)
        {
            SwitchItems(hotbarSlots[chosenItemIdx], armorSlots[(int)ChosenItem.bodyPart]);
        }
    }

    public float GetCurrentDefenseLevel()
    {
        float sum = 0;
        foreach (var i in armorSlots)
        {
            if (i != null && i.Item != null) sum += i.Item.defenseLevel;
        }
        return sum;
    }
}
