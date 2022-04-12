using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArmorSlot : InventorySlot
{
    public BodyPart bodyPart;
    public override void OnDrop(BaseEventData data)
    {
        if (inventory.selectedSlot.Item.IsSuitableForJob(Jobs.ARMOR) && inventory.selectedSlot.Item.bodyPart == bodyPart)
        {
            inventory.SwitchItems(this);
        }
    }
}
