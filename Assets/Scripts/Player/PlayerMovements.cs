using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovements : MonoBehaviour
{
    public GameObject playerBody; // player's body to move (the whole prefab)
    private Inventory inventory;
    public PlayerHealth playerHealth;

    private float xRotation;

    public void Look(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && playerHealth.Health > 0 && !isInStopMenu)
        {
            Vector2 mouse = ctx.ReadValue<Vector2>();
            mouse = new Vector2(mouse.x * GameStateController.SensitivityX, mouse.y * GameStateController.SensitivityY) * Time.deltaTime;

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
        if (!isInInventory && !isInStopMenu)
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
    private LayerMask DISTRUCTIBLE_LAYER;
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
            Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTIBLE_LAYER, QueryTriggerInteraction.Collide))
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

    public void Use(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isUse = true;
        }
        else if (ctx.canceled)
        {
            isUse = false;
        }
    }

    bool canEat = true;
    bool canPlace = true;
    [SerializeField]
    float EATING_TIME, PLACING_TIME; // time between eatings
    [SerializeField]
    private LayerMask SURFACE_LAYER;
    [SerializeField]
    int MIN_PLACING_DISTANCE, MAX_PLACING_DISTANCE;

    public CraftingMenuInitializer craftingMenuInitializer;
    public void Use()
    {
        if (Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTIBLE_LAYER, QueryTriggerInteraction.Collide))
        {
            if (hit.transform.gameObject.TryGetComponent<RecipeCraftingMenuItem>(out RecipeCraftingMenuItem menuItem))
            {
                switch (menuItem.menuType)
                {
                    case MenuType.CRAFTING_TABLE:
                        craftingMenuInitializer.OnCraftingTable();
                        break;
                    case MenuType.FURNACE:
                        craftingMenuInitializer.OnFurnace();
                        break;
                    case MenuType.ANVIL:
                        craftingMenuInitializer.OnAnvil();
                        break;
                }
                ToggleInventory();
            }
            return;
        }
        // use logic
        Item currentItem = inventory.ChosenItem;
        if (currentItem == null) return;
        if (currentItem.placeable && canPlace)
        {
            // place
            if (Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out hit, MAX_PLACING_DISTANCE, SURFACE_LAYER, QueryTriggerInteraction.Collide) && hit.distance > MIN_PLACING_DISTANCE)
            {
                canPlace = false;
                Instantiate(currentItem.placedObject, hit.point, Quaternion.identity);
                inventory.RemoveFromInventory(currentItem, 1);
                Invoke(nameof(ResetCanPlace), PLACING_TIME);
            }
        }
        else
        {
            if (currentItem.job == Jobs.ARMOR)
            {
                inventory.WearArmor();
            }
            else if (currentItem.job == Jobs.FOOD && canEat)
            {
                canEat = false;
                if (!(playerHealth.Health == 100))
                {
                    playerHealth.AddHealth(currentItem.hpBonus);
                    inventory.RemoveFromInventory(currentItem, 1);
                }
                Invoke(nameof(ResetCanEat), EATING_TIME);
            }
        }
    }

    void ResetCanEat()
    {
        canEat = true;
    }
    void ResetCanPlace()
    {
        canPlace = true;
    }

    public void ChooseItem(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            string digit = ((KeyControl)ctx.control).keyCode.ToString();
            if (int.TryParse(digit[digit.Length - 1].ToString(), out int digitPressed)) inventory.ChooseItem(digitPressed - 1);
        }
        // ctx.action.GetBindingIndex();
    }

    public void ChangeItem(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            float scrollY = ctx.ReadValue<float>();
            if (scrollY > 0) inventory.ChooseNextItem();
            else if (scrollY < 0) inventory.ChoosePrevItem();
        }
    }

    private void Update()
    {
        if (playerHealth.Health <= 0)
        {
            DeathHold();
            return;
        }
        // Gravity 
        UpdateGravity();

        if (!isInInventory && !isInStopMenu)
        {
            controller.Move(Time.deltaTime * (isSprinting ? sprintSpeed : speed) * (Camera.main.transform.forward * movingDirection.y + Camera.main.transform.right * movingDirection.x)); // moving
            // Hit Logic
            if (isHitKeyPressed) Hit();
            // Use Logic
            if (isUse) Use();
        }

    }

    public void DeathHold()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    bool isInInventory = false;
    public GameObject inventoryObject, crosserObject;

    public void ToggleInventory(InputAction.CallbackContext ctx)
    {
        if (!isInStopMenu && ctx.performed)
        {
            if (isInInventory)
            {
                craftingMenuInitializer.restartCraftingMenu();
                Cursor.lockState = CursorLockMode.Locked;
                isInInventory = false;
                inventoryObject.SetActive(false);
                crosserObject.SetActive(true);
            } 
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInInventory = true;
                inventoryObject.SetActive(true);
                crosserObject.SetActive(false);

            }
        }
    }

    public void ToggleInventory()
    {
        if (!isInStopMenu)
        {
            if (isInInventory)
            {
                craftingMenuInitializer.restartCraftingMenu();
                Cursor.lockState = CursorLockMode.Locked;
                isInInventory = false;
                inventoryObject.SetActive(false);
                crosserObject.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInInventory = true;
                inventoryObject.SetActive(true);
                crosserObject.SetActive(false);
            }
        }
    }

    bool isInStopMenu = false;
    public GameObject stopMenuObject;
    public void ToggleStopMenu(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && ctx.performed)
        {
            if (isInStopMenu)
            {
                Cursor.lockState = CursorLockMode.Locked;
                isInStopMenu = false;
                stopMenuObject.SetActive(false);
                crosserObject.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInStopMenu = true;
                stopMenuObject.SetActive(true);
                crosserObject.SetActive(false);
            }
        }
    }

    public void CloseMenuByButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInStopMenu = false;
        stopMenuObject.SetActive(false);
        crosserObject.SetActive(true);
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inventory = Inventory.Instance;
        inventoryObject.SetActive(false);
        stopMenuObject.SetActive(false);
    }
}
