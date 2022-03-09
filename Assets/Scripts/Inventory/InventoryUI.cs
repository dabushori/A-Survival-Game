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

        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
        hotbarSlots = hotbarParent.GetComponentsInChildren<InventorySlot>();
        Debug.Log(inventorySlots);
        Debug.Log(hotbarSlots);
        inventory.setSlots(inventorySlots, hotbarSlots);
    }
}
