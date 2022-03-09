using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public Transform hotbarParent;
    Inventory inventory;

    InventorySlot[] inventorySlots;
    InventorySlot[] hotbarSlots;
    void Start()
    {
        inventory = Inventory.Instance;
        // inventory.onItemChangedCallback += UpdateUI;

        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
        hotbarSlots = hotbarParent.GetComponentsInChildren<InventorySlot>();
        inventory.setSlots(inventorySlots, hotbarSlots);
    }
    /*
    void UpdateUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                inventorySlots[i].AddItem(inventory.items.Keys.ToArray()[i],inventory.items.Values.ToArray()[i]);
            } else
            {
                inventorySlots[i].ClearSlot();
            }
        }
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (i < inventory.hotbar.Count)
            {
                hotbarSlots[i].AddItem(inventory.hotbar.Keys.ToArray()[i], inventory.hotbar.Values.ToArray()[i]);
            }
            else
            {
                hotbarSlots[i].ClearSlot();
            }
    }
    */
}
