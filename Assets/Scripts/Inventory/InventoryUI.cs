using Photon.Pun;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public Transform hotbarParent;
    public Transform armorParent;
    Inventory inventory;

    InventorySlot[] inventorySlots;
    InventorySlot[] hotbarSlots;
    InventorySlot[] armorSlots;
    /**
     * Creating the entire UI of the inventory
     */
    void Awake()
    {
        if (PhotonView.Get(transform.parent.parent.gameObject).IsMine)
        {
            inventory = Inventory.Instance;
            inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
            hotbarSlots = hotbarParent.GetComponentsInChildren<InventorySlot>();
            armorSlots = armorParent.GetComponentsInChildren<InventorySlot>();
            inventory.setSlots(inventorySlots, hotbarSlots, armorSlots);
        }
    }
}
