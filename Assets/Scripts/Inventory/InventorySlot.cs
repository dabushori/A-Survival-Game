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
    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.Instance;
    }

    public Item Item
    {
        get
        {
            return item;
        }
        private set
        {
            if (value == null)
            {
                icon.enabled = false; 
                item = null;
                Debug.Log("icon.enabled = " + icon.enabled + " item = " + item);
            }
            else
            {
                icon.sprite = value.icon;
                icon.enabled = true;
                item = value;
            }
        }
    }

    public int Amount
    {
        get
        {
            return amount;
        }
        private set
        {
            if (value == 0)
            {
                amountView.text = "";
                amountView.enabled = false;
                amount = 0;
            } 
            else
            {
                amountView.text = value.ToString();
                amountView.enabled = true;
                amount = value;
            }
        }
    }

    public void IncAmount(int amount = 1)
    {
        Amount += amount;
    }

    public void DecAmount(int amount = 1)
    {
        Amount -= amount;
        if (Amount == 0) ClearSlot();
    }

    public void AddItem(Item newItem, int newAmount)
    {
        Item = newItem;
        Amount = newAmount;

        if (!isHotbar)
        {
            removeButton.interactable = true;
        }
    }

    public void ClearSlot()
    {
        Item = null;
        Amount = 0;

        if (!isHotbar)
        {
            removeButton.interactable = false;
        }
    }

    public void onRemoveButton()
    {
        inventory.RemoveFromInventory(item, 1);
    }

    public void OnDrag(BaseEventData data)
    {
        Debug.Log("drag " + gameObject.name);
        inventory.selectedSlot = this;
    }
    public void OnDrop(BaseEventData data)
    {
        Debug.Log("drop " + gameObject.name);
        inventory.SwitchItems(this);
    }
}
