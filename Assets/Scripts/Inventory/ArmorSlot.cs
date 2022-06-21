using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/*
 * Slot that is only used for armor (The armor the player wear)
 */
public class ArmorSlot : InventorySlot
{
    [SerializeField]
    public BodyPart bodyPart; // body part of the armor slot

    /*
     * when dropping an item inside the slot
     */
    public override void OnDrop(BaseEventData data)
    {
        // if the item is an armor item and it is the right bodypart
        if (inventory.selectedSlot.Item != null && inventory.selectedSlot.Item.IsSuitableForJob(Jobs.ARMOR) && inventory.selectedSlot.Item.bodyPart == bodyPart)
        {
            inventory.SwitchItems(this); // switch items
        }
    }
}
