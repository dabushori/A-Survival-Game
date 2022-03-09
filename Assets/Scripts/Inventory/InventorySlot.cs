using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    public TMPro.TMP_InputField amountView;
    public bool isHotbar = false;
    Item item;
    int amount;

    public Item Item
    {
        get
        {
            return item;
        }
    }

    public int Amount
    {
        get
        {
            return amount;
        }
    }

    public void IncAmount(int amount = 1)
    {
        this.amount += amount;
    }

    public void DecAmount(int amount = 1)
    {
        this.amount += amount;
        if (amount == 0) ClearSlot();
    }

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
        Inventory.Instance.RemoveFromInventory(item, 1);
        // --amount;
        // if (amount == 0) ClearSlot();
    }

    public void OnDrag(BaseEventData data)
    {
        Debug.Log(data);
        Debug.Log("drag " + gameObject.name);
    }
    public void OnDrop(BaseEventData data)
    {
        Debug.Log(data);
        Debug.Log("drop " + gameObject.name);
    }
}
