using Photon.Pun;
using UnityEngine;

/*
 * the class is in control of the inventory UI
 */
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    Transform itemsParent; // item parent object
    [SerializeField]
    Transform hotbarParent; // hotbar parent object
    [SerializeField]
    Transform armorParent; // armor parent object

    Inventory inventory; // the inventory

    InventorySlot[] inventorySlots; // inventory slots
    InventorySlot[] hotbarSlots; // hotbar slots
    InventorySlot[] armorSlots; // armor slots

    /*
     * Creating the entire UI of the inventory
     */
    void Awake()
    {
        // if it is my player
        if (PhotonView.Get(transform.parent.parent.gameObject).IsMine)
        {
            inventory = Inventory.Instance; // save inventory instance
            // save all slots in the parents inside the arrays
            inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
            hotbarSlots = hotbarParent.GetComponentsInChildren<InventorySlot>();
            armorSlots = armorParent.GetComponentsInChildren<InventorySlot>();
            // save the slots inside the inventory
            inventory.setSlots(inventorySlots, hotbarSlots, armorSlots);
        }
    }
}
