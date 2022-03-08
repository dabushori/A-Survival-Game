using UnityEngine;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    public TMPro.TMP_InputField amountView;
    public bool isHotbar = false;
    Item item;
    int amount;
    //bool isBeingDraged = false;
    public void AddItem(Item newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;

        icon.sprite = item.icon;
        icon.enabled = true;

        amountView.text = amount.ToString();
        amountView.enabled = true;

        if (!isHotbar)
        {
            removeButton.interactable = true;
        }
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;

        amountView.text = "";
        amountView.enabled = false;
        
        icon.sprite = null;
        icon.enabled = false;

        if (!isHotbar)
        {
        removeButton.interactable = false;
        }
    }

    public void onRemoveButton()
    {
        Inventory.instance.RemoveFromInventory(item, 1);
    }
}
