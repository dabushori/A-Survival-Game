using Photon.Pun;
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
        if (!isInLoadingScreen && !isInInventory && playerHealth.Health > 0 && !isInStopMenu)
        {
            Vector2 mouse = ctx.ReadValue<Vector2>();
            mouse = new Vector2(mouse.x * GameStateController.SensitivityX, mouse.y * GameStateController.SensitivityY) * Time.deltaTime;

            xRotation -= mouse.y;
            xRotation = Mathf.Clamp(xRotation, -90, 50);
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
    }

    [SerializeField]
    private float gravity;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private LayerMask GROUND_LAYER;
    void UpdateGravity()
    {
        if (Physics.CheckSphere(playerBody.transform.position, 0.5f, GROUND_LAYER) && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("IsJumping", false);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    [SerializeField]
    float jumpPower;
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && !isInStopMenu)
        {
            if (ctx.performed && Physics.CheckSphere(playerBody.transform.position, 0.5f, GROUND_LAYER) && velocity.y < 0)
            {
                velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
                animator.SetBool("IsJumping", true);
            }
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
        if (isInInventory || isInStopMenu) return;
        if (ctx.started)
        {
            isHitKeyPressed = true;
            animator.SetBool("IsHitting", true);
            hitStartTime = Time.time;
        }
        else if (ctx.canceled)
        {
            isHitKeyPressed = false;
            animator.SetBool("IsHitting", false);
            hitStartTime = 0;
        }
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
            if (isInInventory || isInStopMenu) return;
            isUse = true;
        }
        else if (ctx.canceled)
        {
            isUse = false;
        }
    }

    bool canEat = true;
    bool canPlace = true;
    bool canWear = true;
    [SerializeField]
    float EATING_TIME, PLACING_TIME, WEARING_TIME; // time between eatings
    [SerializeField]
    private LayerMask SURFACE_LAYER;
    [SerializeField]
    int MIN_PLACING_DISTANCE, MAX_PLACING_DISTANCE;

    public CraftingMenuInitializer craftingMenuInitializer;
    public void Use()
    {
        if (Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTIBLE_LAYER, QueryTriggerInteraction.Collide)) {
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
        }
        // use logic
        Item currentItem = inventory.ChosenItem;
        if (currentItem == null) return;
        if (currentItem.placeable)
        {
            if (canPlace)
            {
                // place
                if (Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out hit, MAX_PLACING_DISTANCE, SURFACE_LAYER, QueryTriggerInteraction.Collide) && hit.distance > MIN_PLACING_DISTANCE)
                {
                    canPlace = false;
                    Vector3 cameraForward = Camera.main.transform.forward;
                    cameraForward.y = 0;
                    PhotonNetwork.InstantiateRoomObject(GameStateController.furniturePath + currentItem.name, hit.point, Quaternion.LookRotation(cameraForward));
                    inventory.RemoveFromInventory(currentItem, 1);
                    Invoke(nameof(ResetCanPlace), PLACING_TIME);
                }
            }
        }
        else
        {
            if (currentItem.job == Jobs.ARMOR)
            {
                if (canWear)
                {
                    canWear = false;
                    inventory.WearArmor();
                    WearCurrentArmor();
                    Invoke(nameof(ResetCanWear), WEARING_TIME);
                }
            }
            else if (currentItem.job == Jobs.FOOD)
            {
                if (canEat)
                {
                    if (playerHealth.Health != playerHealth.MaxHealth)
                    {
                        inventory.RemoveFromInventory(currentItem, 1);
                        canEat = false;
                        animator.SetTrigger("IsEating");
                        Invoke(nameof(DelayEating), EATING_TIME);
                        healthToAdd = currentItem.hpBonus;
                        playerControls.PlayEatingSound();
                    }
                }
            }
        }
    }
    private int healthToAdd = 0;
    private void DelayEating()
    {
        if (playerHealth.Health > 0) playerHealth.AddHealth(healthToAdd);
        ResetCanEat();
    }

    [SerializeField]
    Transform characterBoneRoot;

    [SerializeField]
    GameObject[] armor;

    public void WearCurrentArmor()
    {
        var armorSlots = inventory.GetArmorSlots();
        if (armorSlots == null) return;
        foreach (var slot in armorSlots)
        {
            GameObject currArmor = armor[(int)((ArmorSlot)slot).bodyPart];
            ArmorSync armorSync = currArmor.GetComponent<ArmorSync>();
            if (slot.Amount > 0)
            {
                if (armorSync != null)
                {
                    armorSync.EnableArmor();
                    Material m = slot.Item.itemToHold.GetComponent<Renderer>().sharedMaterial;
                    armorSync.SetMaterial(m);
                }
            }
            else
            {
                armorSync.DisableArmor();
            }
        }
    }

    void ResetCanEat()
    {
        canEat = true;
    }
    void ResetCanWear()
    {
        canWear = true;
    }
    void ResetCanPlace()
    {
        canPlace = true;
    }

    bool canChooseItem = true;

    public void ChooseItem(InputAction.CallbackContext ctx)
    {
        if (canChooseItem && !isInLoadingScreen && !isInInventory && !isInStopMenu && ctx.performed)
        {
            canChooseItem = false;
            Invoke(nameof(ResetCanChooseItem), 0.2f);
            string digit = ((KeyControl)ctx.control).keyCode.ToString();
            if (int.TryParse(digit[digit.Length - 1].ToString(), out int digitPressed))
            {
                inventory.ChooseItem(digitPressed - 1);
            }
        }
    }

    public void ChangeItem(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && !isInLoadingScreen && !isInStopMenu && ctx.performed)
        {
            float scrollY = ctx.ReadValue<float>();
            if (scrollY > 0) inventory.ChooseNextItem();
            else if (scrollY < 0) inventory.ChoosePrevItem();
        }
    }

    [SerializeField]
    PlayerControls playerControls;
    GameObject currentItem;
    public void HoldCurrentItem()
    {
        if (inventory.ChosenItem != null && inventory.ChosenItem.itemToHold != null)
        {
            if (currentItem == inventory.ChosenItem.itemToHold) return;
            currentItem = inventory.ChosenItem.itemToHold;
            playerControls.HoldItem(inventory.ChosenItem.itemToHold.name);
        }
        else
        {
            if (currentItem == null) return;
            currentItem = null;
            playerControls.HoldItem(null);
        }
    }

    void ResetCanChooseItem()
    {
        canChooseItem = true;
    }

    public bool isInLoadingScreen = true;

    private void Update()
    {
        if (isInLoadingScreen) return;
        if (playerHealth.Health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        // Gravity 
        UpdateGravity();

        if (!isInInventory && !isInStopMenu)
        {
            Vector3 movingVec = Vector3.zero;
            movingVec = (Camera.main.transform.forward * movingDirection.y + Camera.main.transform.right * movingDirection.x);
            movingVec.y = 0;
            // Hit Logic
            if (isHitKeyPressed) Hit();
            // Use Logic
            if (isUse) Use();

            if (!isSprinting)
            {
                animator.SetBool("IsSprinting", false);
                if (movingVec.x == 0 && movingVec.z == 0) animator.SetBool("IsWalking", false);
                else animator.SetBool("IsWalking", true);
            } else
            {
                animator.SetBool("IsWalking", false);
                if (movingVec.x == 0 && movingVec.z == 0) animator.SetBool("IsSprinting", false);
                else animator.SetBool("IsSprinting", true);
            }
            controller.Move(Time.deltaTime * (isSprinting ? sprintSpeed : speed) * movingVec);

        }
        if (inventory != null)
        {
            WearCurrentArmor();
            HoldCurrentItem();
        }

    }

    bool isInInventory = false;
    public GameObject inventoryObject, crosserObject;

    public void ToggleInventory(InputAction.CallbackContext ctx)
    {
        if (!isInStopMenu && !isInLoadingScreen && ctx.performed)
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
        if (!isInLoadingScreen && !isInStopMenu)
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
        if (!isInInventory && !isInLoadingScreen && ctx.performed)
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

    [SerializeField]
    GameObject[] playerMeshes;
    public void SetCharacterVisible(bool isVisible)
    {
        foreach (var i in playerMeshes) i.SetActive(isVisible);
    }

    [SerializeField]
    PhotonView photonView;
    [SerializeField]
    GameObject[] localPlayerLogic;
    [SerializeField]
    Animator animator;
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            foreach (GameObject obj in localPlayerLogic)
            {
                Destroy(obj);
            }
            Destroy(gameObject);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        inventory = Inventory.Instance;
        inventoryObject.SetActive(false);
        stopMenuObject.SetActive(false);
        SetCharacterVisible(false);

        // testing
        Invoke(nameof(Init), 3);
    }

    private void Init()
    {
        foreach (var r in RecipesDatabase.furnaceRecipes) Inventory.Instance.AddToInventory(r.craftedItem, 10);
        foreach (var r in RecipesDatabase.anvilRecipes) Inventory.Instance.AddToInventory(r.craftedItem, 10);
        playerHealth.DealDamage(90);
    }
}
