using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    Image icon; // icon of the item

    [SerializeField]
    Button removeButton; // remove button for removing items

    [SerializeField]
    TMPro.TMP_InputField amountView; // amount of the item

    [SerializeField]
    bool isHotbar = false; // is this slot hotbar

    protected Item item; // the item in the slot
    protected int amount; // the amount of the item
    protected Inventory inventory; // the inventory that the slot belongs to

    /*
     * Singelton inventory
     */
    private void Start()
    {
        inventory = Inventory.Instance;
    }

    /*
     * Getter Setter to the item
     */
    public Item Item
    {
        get
        {
            return item;
        }
        private set
        {
            // if the item is null disable the visuals
            if (value == null)
            {
                icon.enabled = false; 
                item = null;
            }
            else
            {
                icon.sprite = value.icon;
                icon.enabled = true;
                item = value;
            }
        }
    }
    /*
     * Getter Setter to the item
     */
    public int Amount
    {
        get
        {
            return amount;
        }
        private set
        {
            // if the amount is 0 disable the visuals
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

    /*
     * increase amount of item by 1 in the slot
     */
    public void IncAmount(int amount = 1)
    {
        Amount += amount;
    }

    /*
     * decrease amount of item by 1 in the slot
     */
    public void DecAmount(int amount = 1)
    {
        Amount -= amount;
        if (Amount == 0) ClearSlot();
    }

    /*
     * add item to the slot
     */
    public void AddItem(Item newItem, int newAmount)
    {
        Item = newItem;
        Amount = newAmount;

        if (!isHotbar)
        {
            removeButton.interactable = true;
        }
    }

    /*
     * clear the slot from the item
     */
    public void ClearSlot()
    {
        Item = null;
        Amount = 0;

        if (!isHotbar)
        {
            removeButton.interactable = false;
        }
    }

    /*
     * remove 1 of the item from the slot
     */
    public void onRemoveButton()
    {
        inventory.RemoveFromInventory(item, 1);
    }

    /*
     * drag and drop functions to move items in the inventory
     */
    public void OnDrag(BaseEventData data)
    {
        // select the first slot
        inventory.selectedSlot = this;
    }
    public virtual void OnDrop(BaseEventData data)
    {
        // select second slot and switch the two items in the slots
        inventory.SwitchItems(this);
    }
}
