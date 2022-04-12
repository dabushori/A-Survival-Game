using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovements : MonoBehaviour
{
    public GameObject playerBody; // player's body to move (the whole prefab)
    private Inventory inventory;

    private float xRotation;
    [SerializeField]
    private float mouseXSensitivity, mouseYSensitivity;
    public void Look(InputAction.CallbackContext ctx)
    {
        if (!isInInventory)
        {
            Vector2 mouse = ctx.ReadValue<Vector2>();
            mouse = new Vector2(mouse.x * mouseXSensitivity, mouse.y * mouseYSensitivity) * Time.deltaTime;

            xRotation -= mouse.y;
            xRotation = Mathf.Clamp(xRotation, -90, 90);
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            playerBody.transform.Rotate(Vector3.up * mouse.x);
        }
    }

    [SerializeField]
    private float speed;
    [SerializeField]
    private float sprintSpeed;
    public CharacterController controller;
    private Vector2 movingDirection;
    public void Move(InputAction.CallbackContext ctx)
    {
        movingDirection = ctx.ReadValue<Vector2>();
        // movingDirection = Camera.main.transform.forward * dir.y + Camera.main.transform.right * dir.x;
    }

    [SerializeField]
    private float gravity;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private LayerMask GROUND_LAYER;
    void UpdateGravity()
    {
        if (Physics.CheckSphere(playerBody.transform.position, 0.5f, GROUND_LAYER) && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        // Debug.Log(velocity * Time.deltaTime);
    }

    [SerializeField]
    float jumpPower;
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!isInInventory)
        {
            // if (ctx.performed) controller.Move(Vector3.up * jumpPower);
            if (ctx.performed && Physics.CheckSphere(playerBody.transform.position, 0.5f, GROUND_LAYER) && velocity.y < 0) velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }
    }

    private bool isSprinting = false;
    public void Sprint(InputAction.CallbackContext ctx)
    {
        isSprinting = !ctx.canceled;
    }

    [SerializeField]
    private LayerMask DISTRUCTABLE_LAYER;
    [SerializeField]
    private float MINING_DISTANCE;
    [SerializeField]
    private LayerMask MOBS_LAYER;
    [SerializeField]
    private float MOB_HITTING_DISTANCE;
    private bool isHitKeyPressed = false;
    private float hitStartTime;
    public void Hit(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isHitKeyPressed = true;
            hitStartTime = Time.time;
        }
        else if (ctx.canceled)
        {
            isHitKeyPressed = false;
            hitStartTime = 0;
        }
        // Debug.Log(Time.fixedTimeAsDouble);
    }

    [SerializeField]
    private float MINING_TIME, HITTING_TIME;
    private bool isHit;
    private void ResetHit()
    {
        isHit = false;
    }

    public int DEFAULT_BREAKING_DAMAGE;
    public void Hit()
    {
        // Hit Logic
        if (
            (Time.time - hitStartTime) > MINING_TIME && // will use item mining speed
            Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTABLE_LAYER, QueryTriggerInteraction.Collide))
        {
            hit.transform.gameObject.GetComponent<Destructible>().Break(inventory);
            // hit.transform.gameObject.GetComponent<Destructible>().Hit(50); // use item damage - currently for testing
            hitStartTime = Time.time;
        }


        if (!isHit && // will use item mining speed
            Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out hit, MINING_DISTANCE, MOBS_LAYER, QueryTriggerInteraction.Collide))
        {
            isHit = true;
            hit.transform.gameObject.GetComponent<Destructible>().Hit(inventory);
            Invoke(nameof(ResetHit), HITTING_TIME);
        }
    }

    private bool isUse = false;
    private float useStartTime;

    public void Use(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isUse = true;
            useStartTime = Time.time;
        }
        else if (ctx.canceled)
        {
            isUse = false;
            useStartTime = 0;
        }
    }

    public void Use()
    {
        // use logic
        Debug.Log("use");
        // if (inventory.chosenItem.usable) inventory.chosenItem.Use();
        // something like currentItem.use
    }

    public void ChooseItem(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            string digit = ((KeyControl)ctx.control).keyCode.ToString();
            if (int.TryParse(digit[digit.Length - 1].ToString(), out int digitPressed)) inventory.ChooseItem(digitPressed - 1);
            Debug.Log(inventory.ChosenItem?.name);
        }
        // ctx.action.GetBindingIndex();
    }

    private void Update()
    {
        // Gravity 
        UpdateGravity();

        if (!isInInventory)
        {
            controller.Move(Time.deltaTime * (isSprinting ? sprintSpeed : speed) * (Camera.main.transform.forward * movingDirection.y + Camera.main.transform.right * movingDirection.x)); // moving
            // Hit Logic
            if (isHitKeyPressed) Hit();
            // Use Logic
            if (isUse) Use();
        }

    }

    bool isInInventory = false;
    public GameObject inventoryObject;

    public void ToggleInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isInInventory)
            {
                Cursor.lockState = CursorLockMode.Locked;
                isInInventory = false;
                inventoryObject.SetActive(false);
            } 
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInInventory = true;
                inventoryObject.SetActive(true);
            }
        }
    }

    public void CloseMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // pause game
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inventory = Inventory.Instance;
        mouseXSensitivity = GameStateController.SensitivityX;
        mouseYSensitivity = GameStateController.SensitivityY;
        inventoryObject.SetActive(false);
    }
}
