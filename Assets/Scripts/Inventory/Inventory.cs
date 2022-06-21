using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{
    private static Inventory instance = new Inventory(); // singleton

    /*
     * constructor
     */
    private Inventory() { }
    
    // singelton get
    public static Inventory Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    InventorySlot[] itemSlots; // item slots
    [SerializeField]
    InventorySlot[] hotbarSlots; // hotbar slots
    [SerializeField]
    InventorySlot[] armorSlots; // armor slots

    /*
     * return the armor slots
     */
    public InventorySlot[] GetArmorSlots()
    {
        return armorSlots;
    }

    /*
     * check if the inventory is full
     */
    public bool isFull()
    {
        // check items and hotbar
        foreach(InventorySlot slot in itemSlots)
        {
            if (slot.Item == null) return false;
        }
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == null) return false;
        }
        return true;
    }

    /*
     * connect the slots in the inventory UI to the game slots
     */
    public void setSlots(InventorySlot[] itemSlots, InventorySlot[] hotbarSlots, InventorySlot[] armorSlots)
    {
        this.itemSlots = itemSlots;
        this.hotbarSlots = hotbarSlots;
        this.armorSlots = armorSlots;

        SetSelectedItemColor();
    }

    [SerializeField]
    int hotbarSpace = 8; // hotbar space
    int chosenItemIdx = 0; // chosen item index

    // return the chosen item right now
    public Item ChosenItem
    {
        get
        {
            return hotbarSlots[chosenItemIdx].Item;
        }
    }

    /*
     * reset the selected color on the slot back to normal
     */
    private void ResetSelectedItemColor()
    {
        hotbarSlots[chosenItemIdx].gameObject.GetComponentInChildren<Image>().color = Color.white;
    }

    /*
     * set the selected color on the chosen slot
     */
    private void SetSelectedItemColor()
    {
        hotbarSlots[chosenItemIdx].gameObject.GetComponentInChildren<Image>().color = new Color(0.769f, 0.769f, 0.769f, 0.5f); // change color of chosen slot
    }

    /*
     * set chosen item
     */
    public void ChooseItem(int slotIdx)
    {
        ResetSelectedItemColor(); // reset last chosen coloer
        chosenItemIdx = slotIdx; // change chosen
        SetSelectedItemColor(); // set new chosen color
    }

    /*
     * choose the next item
     */
    public void ChooseNextItem()
    {
        ChooseItem((chosenItemIdx + 1) % hotbarSpace);
    }

    /*
     * choose the previous item
     */
    public void ChoosePrevItem()
    {
        ChooseItem((chosenItemIdx + hotbarSpace - 1) % hotbarSpace);
    }

    /*
     * get the amount of the item
     */
    public int GetAmountOfItem(Item item)
    {
        // search in hotbar
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == item)
            {
                return slot.Amount;
            }
        }
        // search in items
        foreach (InventorySlot slot in itemSlots)
        {
            if (slot.Item == item)
            {
                return slot.Amount;
            }
        }
        return 0;
    }

    /*
     * add the item to the inventory
     */
    public bool AddToInventory(Item item, int amount)
    {
        // edge
        if (amount < 0) return false;
        if (amount == 0) return true;

        // search in hotbar
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == item)
            {
                slot.IncAmount(amount);
                return true;
            }
        }
        // search in items
        foreach (InventorySlot slot in itemSlots) {
            if (slot.Item == item)
            {
                slot.IncAmount(amount);
                return true;
            }
        }
        // search in hotbar for place
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == null)
            {
                slot.AddItem(item, amount);
                return true;
            }
        }
        // search in items for place
        foreach (InventorySlot slot in itemSlots)
        {
            if (slot.Item == null)
            {
                slot.AddItem(item, amount);
                return true;
            }
        }
        // if there is no place at all return false
        return false;
    }

    /*
     * remove the item from the inventory
     */
    public bool RemoveFromInventory(Item item, int amount)
    {
        // edge
        if (amount < 0) return false;
        if (amount == 0) return true;
        // search in hotbar
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                return true;
            }
        }
        // search in items
        foreach (InventorySlot slot in itemSlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                return true;
            }
        }
        // search in armor
        foreach (InventorySlot slot in armorSlots)
        {
            if (slot.Item == item)
            {
                slot.DecAmount(amount);
                return true;
            }
        }
        // if cant be found return false
        return false;
    }

    public InventorySlot selectedSlot; // the selected slot that we  will switch an item with

    /*
     *  switch between selectedSlot and newSlot
     */
    public void SwitchItems(InventorySlot newSlot)
    {
        if (selectedSlot == null) return;
        // selected item
        Item i1 = selectedSlot.Item;
        int amount1 = selectedSlot.Amount;

        // new item
        Item i2 = newSlot.Item;
        int amount2 = newSlot.Amount;
        // put new item in selected slot if there is a new item
        if (i2 != null && amount2 > 0) selectedSlot.AddItem(i2, amount2);
        else selectedSlot.ClearSlot(); // else clear the slot
        // put slected item in new slot if there is a selected item
        if (i1 != null && amount1 > 0) newSlot.AddItem(i1, amount1);
        else newSlot.ClearSlot(); // else clear the slot

        selectedSlot = null;
    }

    /*
     * switch between srcSlot and destSlot (works for armor)
     */
    public void SwitchItems(InventorySlot srcSlot, InventorySlot dstSlot)
    {
        // src item
        Item i1 = srcSlot.Item;
        int amount1 = srcSlot.Amount;
        // dest item
        Item i2 = dstSlot.Item;
        int amount2 = dstSlot.Amount;

        // put dest item in src slot if there is a dest item
        if (i2 != null && amount2 > 0) srcSlot.AddItem(i2, amount2);
        else srcSlot.ClearSlot();  // else clear the slot
        // put src item in dest slot if there is a src item
        if (i1 != null && amount1 > 0) dstSlot.AddItem(i1, amount1);
        else dstSlot.ClearSlot(); // else clear the slot
    }

    /*
     * put the item in the armor inventory
     */
    public void WearArmor()
    {
        // if the item is an armor wear it
        if (ChosenItem != null && ChosenItem.job == Jobs.ARMOR)
        {
            SwitchItems(hotbarSlots[chosenItemIdx], armorSlots[(int)ChosenItem.bodyPart]);
        }
    }

    /*
     * get the defense level of the items from the armor slot
     */
    public float GetCurrentDefenseLevel()
    {
        float sum = 0;
        // sum the defense level
        foreach (var i in armorSlots)
        {
            if (i != null && i.Item != null) sum += i.Item.defenseLevel;
        }
        return sum;
    }

    /*
     * clear the inventory
     */
    public void ClearInventory()
    {
        foreach (InventorySlot slot in hotbarSlots)
        {
            slot.ClearSlot();
        }
        foreach (InventorySlot slot in itemSlots)
        {
            slot.ClearSlot();
        }
        foreach (InventorySlot slot in armorSlots)
        {
            slot.ClearSlot();
        }
    }
}
