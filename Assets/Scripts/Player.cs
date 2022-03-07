using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    // mining variables
    float MINING_DISTANCE = 2f;
    int DISTRUCTABLE_LAYER = 1 << 7;

    bool isPressed = false;
    DateTime lastClick;
    TimeSpan miningTime = TimeSpan.FromSeconds(1);

    // user inventory
    Inventory userInventory = new Inventory(10);

    public Item CurrentItem
    {
        get
        {
            return userInventory.ChosenItem;
        }
    }


    private void UpdatePressed()
    {
        if (!isPressed && Pointer.current.press.isPressed)
        {
            isPressed = true;
            lastClick = DateTime.Now;
        }
        else if (isPressed && !Pointer.current.press.isPressed)
        {
            isPressed = false;
        }
    }

    private void Start()
    {
        Keyboard.current.onTextInput += OnTextInput;
    }

    private void Update()
    {
        UpdatePressed();
        if (Pointer.current.press.isPressed && (lastClick + miningTime) < DateTime.Now)
        {
            lastClick = DateTime.Now;
            if (CurrentItem != null && CurrentItem.CanBreak && Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTABLE_LAYER, QueryTriggerInteraction.Collide))
            {
                hit.transform.gameObject.GetComponent<Destructible>().Hit(50);
            }
        }
    }

    private void OnTextInput(char c)
    {
        int inventorySlot = int.TryParse(c.ToString(), out int res) ? res : -1;
        if (inventorySlot != -1)
        {
            inventorySlot = (inventorySlot + 9) % 10;
            userInventory.ChooseItem(inventorySlot);
            Debug.Log(CurrentItem?.name);
        }
    }
}
