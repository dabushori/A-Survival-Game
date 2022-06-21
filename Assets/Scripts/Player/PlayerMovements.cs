using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovements : MonoBehaviour
{
    // The player's body to move (the whole object)
    [SerializeField]
    GameObject playerBody; 

    // The player's health object
    [SerializeField]
    PlayerHealth playerHealth;

    // The player's inventory
    Inventory inventory;

    // The current camera's rotation around the x axis
    float xRotation;

    // Player's speed when not sprinting
    [SerializeField]
    float speed;

    // Player's speed when sprinting
    [SerializeField]
    float sprintSpeed;
    
    // Player's character controller (used to move the player)
    [SerializeField]
    CharacterController controller;
    
    // The current direction that the player will move to
    Vector2 movingDirection;
    
    // The gravity constant
    [SerializeField]
    float gravity;

    // The current velocity of the player
    Vector3 velocity = Vector3.zero;

    // The force of the player's jump
    [SerializeField]
    float jumpPower;

    // Booleans indicating wether the player is in a specific menu
    public bool isInLoadingScreen = true;
    bool isInInventory = false;
    bool isInStopMenu = false;
    
    // Booleans indicating wether the player is doing some actions
    bool isSprinting = false;
    bool isHitKeyPressed = false;
    bool isHit = false;
    bool isUse = false;
    bool canEat = true;
    bool canPlace = true;
    bool canWear = true;
    bool canChooseItem = true;

    // Constants that indicates the distance that the player can mine, hit and place objects
    [SerializeField]
    float MINING_DISTANCE, MOB_HITTING_DISTANCE;
    [SerializeField]
    int MIN_PLACING_DISTANCE, MAX_PLACING_DISTANCE;

    // A constant that indicates the time that the player needs to hold the key pressed when mining
    [SerializeField]
    float MINING_TIME;

    // Constants that indicate the time that the player needs to wait between hitting/eating/placing/wearing another object
    [SerializeField]
    float HITTING_TIME, EATING_TIME, PLACING_TIME, WEARING_TIME, CHOOSE_ITEM_TIME;

    // The time of the start of the last hit
    float breakStartTime;

    // The CraftingMenuInitializer object 
    [SerializeField]
    CraftingMenuInitializer craftingMenuInitializer;

    // Health to add as the eating is delayed
    int healthToAdd = 0;

    // The root of the character's skeleton stracture
    [SerializeField]
    Transform characterBoneRoot;

    // The armor game objects of the character
    [SerializeField]
    GameObject[] armor;

    // The PlayerControls object of this player (mostly used to send RPCs)
    [SerializeField]
    PlayerControls playerControls;

    // The current item that the player holds (an actual 3D item)
    GameObject currentItem;

    // The object that will view the player's coordinates
    [SerializeField]
    TMP_Text coordinates;

    // Some objects that the script needs to enable and disable
    [SerializeField]
    GameObject stopMenuObject, inventoryObject, crosserObject;

    // The character's meshes that are needed to be hidden when hiding the character
    [SerializeField]
    GameObject[] playerMeshes;

    // The current photon view
    [SerializeField]
    PhotonView photonView;

    // The local player logic (which will be destroyed on non-local characters)
    [SerializeField]
    GameObject[] localPlayerLogic;

    // The character's animator
    [SerializeField]
    Animator animator;

    /*
     * Update the camera's rotation based on mouse input and face the player to where the camera is facing
     */
    public void Look(InputAction.CallbackContext ctx)
    {
        if (!isInLoadingScreen && !isInInventory && playerHealth.Health > 0 && !isInStopMenu)
        {
            // Read the mouse input values
            Vector2 mouse = ctx.ReadValue<Vector2>();

            // Consider the passed time and the mouse sensitivity
            mouse = new Vector2(mouse.x * GameStateController.SensitivityX, mouse.y * GameStateController.SensitivityY) * Time.deltaTime;

            // Rotate the camera around the x axis based on the given input
            xRotation -= mouse.y;
            
            // Clamp the angle to the range [-90, 50] to avoid full circle rotation
            xRotation = Mathf.Clamp(xRotation, -90, 50);

            // Rotate the camera
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            // Rotate the player around the y axis based on mouse input
            playerBody.transform.Rotate(Vector3.up * mouse.x);
        }
    }

    /*
     * Update the movingDirection variable based on the keyboard input
     */
    public void Move(InputAction.CallbackContext ctx)
    {
        movingDirection = ctx.ReadValue<Vector2>();
    }

    /*
     * Update the velocity based on the gravity
     */
    void UpdateGravity()
    {
        // If the player is on the ground, reset the velocity in the y axis
        if (Physics.CheckSphere(playerBody.transform.position, 0.5f, GameStateController.surfaceLayer) && velocity.y < 0)
        {
            velocity.y = -2f;
            // End the jumping animation
            animator.SetBool("IsJumping", false);
        }

        // Increase the velocity in the y axis considering the time passed
        velocity.y += gravity * Time.deltaTime;
        // Move the player on the y axis (delta y = 0.5 * g * t^2)
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    /*
     * Register a jump by the player
     */
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && !isInStopMenu)
        {
            // Check that the player is on the ground and jumps
            if (ctx.performed && Physics.CheckSphere(playerBody.transform.position, 0.5f, GameStateController.groundLayers) && velocity.y < 0)
            {
                // v = sqrt(-2 * F * g)
                velocity.y = Mathf.Sqrt(-2f * jumpPower * gravity);
                // Start the jumping animation
                animator.SetBool("IsJumping", true);
            }
        }
    }

    /*
     * Register that the player is sprinting
     */
    public void Sprint(InputAction.CallbackContext ctx)
    {
        isSprinting = !ctx.canceled;
    }

    /*
     * Register a hit by the player
     */
    public void Hit(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // Not relevant if in the inpventory or stop menu
            if (isInInventory || isInStopMenu) return;
            isHitKeyPressed = true;
            // Start the hit animation
            animator.SetBool("IsHitting", true);
            // Record the hit time
            breakStartTime = Time.time;
        }
        else if (ctx.canceled)
        {
            isHitKeyPressed = false;
            // End the hit animation
            animator.SetBool("IsHitting", false);
            // Reset the hit time
            breakStartTime = 0;
        }
    }

    /*
     * Mark that the player can hit again
     */
    void ResetHit()
    {
        isHit = false;
    }

    /*
     * Hit or break the object the player is looking at
     */
    public void Hit()
    {
        // Break Logic - check that the key is pressed enough time and the player is looking at a breakable object
        if (
            (Time.time - breakStartTime) > MINING_TIME &&
            Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, GameStateController.worldObjectsLayer, QueryTriggerInteraction.Collide))
        {
            // Break the object
            hit.transform.gameObject.GetComponent<Destructible>().Break(inventory);
            // Reset the hit time to now
            breakStartTime = Time.time;
        } 
        
        // Hit Logic - check that enough time has passed from the last hit and that the player is looking at a hit-able object
        if (!isHit &&
            Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out hit, MINING_DISTANCE, GameStateController.mobsLayer, QueryTriggerInteraction.Collide))
        {
            // Mark the hit and reset it in HITTING_TIME seconds
            isHit = true;
            Invoke(nameof(ResetHit), HITTING_TIME);
            // Hit the object
            hit.transform.gameObject.GetComponent<Destructible>().Hit(inventory);
        }
    }

    /*
     * Register a use by the player
     */
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

    /*
     * Use the object that the player currently holds or looks at
     */
    public void Use()
    {
        if (Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, GameStateController.worldObjectsLayer, QueryTriggerInteraction.Collide)) {
            // Check if the player looks at an object that contains a RecipeCraftingMenuItem object
            // If so, update the crafting menu and open the inventory
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
                return;
            }
        }

        // Use logic
        Item currentItem = inventory.ChosenItem;
        if (currentItem == null) return;

        // If the current object is placeable - place it if possible
        if (currentItem.placeable)
        {
            // Verify that enough time has passed from the last placement
            if (canPlace)
            {
                // Check that the player is looking at the surface from the right distance
                if (Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out hit, MAX_PLACING_DISTANCE, GameStateController.surfaceLayer, QueryTriggerInteraction.Collide) && hit.distance > MIN_PLACING_DISTANCE)
                {
                    // Mark the placement and reset it in PLACING_TIME seconds
                    canPlace = false;
                    Invoke(nameof(ResetCanPlace), PLACING_TIME);

                    // Place the item facing the player
                    Vector3 cameraForward = Camera.main.transform.forward;
                    cameraForward.y = 0;
                    PhotonNetwork.Instantiate(GameStateController.furniturePath + currentItem.name, hit.point, Quaternion.LookRotation(cameraForward));
                    
                    // Remove the item from the player's inventory
                    inventory.RemoveFromInventory(currentItem, 1);
                }
            }
        }
        else
        {
            // If the item is an armor, wear it
            if (currentItem.job == Jobs.ARMOR)
            {
                if (canWear)
                {
                    // Mark the wearing and reset it in WEARING_TIME seconds
                    canWear = false;
                    Invoke(nameof(ResetCanWear), WEARING_TIME);

                    // Wear the current held armor
                    inventory.WearArmor();
                    WearCurrentArmor();
                }
            }
            // If the item is a food, eat it
            else if (currentItem.job == Jobs.FOOD)
            {
                if (canEat && playerHealth.Health != playerHealth.MaxHealth)
                {
                    // Mark the eating and eat and reset it in EATING_TIME seconds
                    canEat = false;
                    healthToAdd = currentItem.hpBonus;
                    Invoke(nameof(DelayEating), EATING_TIME);

                    // Remove the item from the inventory
                    inventory.RemoveFromInventory(currentItem, 1);

                    // Start the eating animation
                    animator.SetTrigger("IsEating");
                    
                    // Play the eating sound
                    playerControls.PlayEatingSound();
                }
            }
        }
    }

    /*
     * Eat and mark that the player can eat again
     */
    private void DelayEating()
    {
        if (playerHealth.Health > 0) playerHealth.AddHealth(healthToAdd);
        ResetCanEat();
    }

    /*
     * Update the armor's material to the current worn's one
     */
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
                    // Enable the armor and set its material
                    armorSync.EnableArmor();
                    Material m = slot.Item.itemToHold.GetComponent<Renderer>().sharedMaterial;
                    armorSync.SetMaterial(m);
                }
            }
            else
            {
                // Disable the armor
                armorSync.DisableArmor();
            }
        }
    }

    /*
     * Mark that the player can eat again
     */
    void ResetCanEat()
    {
        canEat = true;
    }

    /*
     * Mark that the player can wear armor again
     */
    void ResetCanWear()
    {
        canWear = true;
    }

    /*
     * Mark that the player can place objects again
     */
    void ResetCanPlace()
    {
        canPlace = true;
    }


    /*
     * Choose the current item based on keyboard input (digits 1 to 8)
     */
    public void ChooseItem(InputAction.CallbackContext ctx)
    {
        if (canChooseItem && !isInLoadingScreen && !isInInventory && !isInStopMenu && ctx.performed)
        {
            // Mark the choice and reset it in 
            canChooseItem = false;
            Invoke(nameof(ResetCanChooseItem), CHOOSE_ITEM_TIME);

            // Get the digit and choose the right item
            string digit = ((KeyControl)ctx.control).keyCode.ToString();
            if (int.TryParse(digit[digit.Length - 1].ToString(), out int digitPressed))
            {
                inventory.ChooseItem(digitPressed - 1);
            }
        }
    }

    /*
     * Change the item based on the scroller input
     */
    public void ChangeItem(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && !isInLoadingScreen && !isInStopMenu && ctx.performed)
        {
            float scrollY = ctx.ReadValue<float>();

            // If the scroller is moved up, choose the next item
            if (scrollY > 0) inventory.ChooseNextItem();
            // Otherwise, choose the previous item
            else if (scrollY < 0) inventory.ChoosePrevItem();
        }
    }

    /*
     * Hold the actual 3D item that is chosen in the inventory
     */
    public void HoldCurrentItem()
    {
        // If holding an item, remove the current 3D item that the charatcer is holding and hold the new one
        if (inventory.ChosenItem != null && inventory.ChosenItem.itemToHold != null)
        {
            if (currentItem == inventory.ChosenItem.itemToHold) return;
            currentItem = inventory.ChosenItem.itemToHold;
            playerControls.HoldItem(inventory.ChosenItem.itemToHold.name);
        }
        // Otherwise, only remove the current 3D item that the charatcer is holding
        else
        {
            if (currentItem == null) return;
            currentItem = null;
            playerControls.HoldItem(null);
        }
    }

    /*
     * Mark that the player can choose a new item again
     */
    void ResetCanChooseItem()
    {
        canChooseItem = true;
    }

    /*
     * Update the viewed coordinates to the player's coordinates
     */
    void UpdateCoordinates()
    {
        Vector3 pos = playerBody.transform.position;
        coordinates.text = String.Format("X: {0} Y: {1} Z: {2}", Math.Round(pos.x, 0), Math.Round(pos.y, 0), Math.Round(pos.z, 0));
    }



    /*
     * Toggle the inventory menu by the user's input
     */
    public void ToggleInventory(InputAction.CallbackContext ctx)
    {
        if (!isInStopMenu && !isInLoadingScreen && ctx.performed)
        {
            // If trying to get out of the inventory
            if (isInInventory)
            {
                // Restart the crafting menu
                craftingMenuInitializer.restartCraftingMenu();

                // Lock the cursor
                Cursor.lockState = CursorLockMode.Locked;

                // Mark that the player is not in the inventory
                isInInventory = false;
                
                // Hide the inventory and display the crosses
                inventoryObject.SetActive(false);
                crosserObject.SetActive(true);
            } 
            // Otherwise
            else
            {
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;

                // Mark that the player is in the inventory
                isInInventory = true;

                // Display the inventory and hide the crosses
                inventoryObject.SetActive(true);
                crosserObject.SetActive(false);
            }

            // Reset all the animations
            ResetAnimations();
        }
    }

    /*
     * Reset all the animations (in case of enterring a menu)
     */
    void ResetAnimations()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsSprinting", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsHitting", false);
    }

    /*
     * Toggle the inventory menu by other reasons (crafting menus)
     */
    public void ToggleInventory()
    {
        if (!isInStopMenu && !isInLoadingScreen)
        {
            // If trying to get out of the inventory
            if (isInInventory)
            {
                // Restart the crafting menu
                craftingMenuInitializer.restartCraftingMenu();

                // Lock the cursor
                Cursor.lockState = CursorLockMode.Locked;

                // Mark that the player is not in the inventory
                isInInventory = false;

                // Hide the inventory and display the crosses
                inventoryObject.SetActive(false);
                crosserObject.SetActive(true);
            }
            // Otherwise
            else
            {
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;

                // Mark that the player is in the inventory
                isInInventory = true;

                // Display the inventory and hide the crosses
                inventoryObject.SetActive(true);
                crosserObject.SetActive(false);
            }

            // Reset all the animations
            ResetAnimations();
        }
    }

    /*
     * Toggle the inventory menu by user's input
     */
    public void ToggleStopMenu(InputAction.CallbackContext ctx)
    {
        if (!isInInventory && !isInLoadingScreen && ctx.performed)
        {
            // If trying to get out of the stop menu
            if (isInStopMenu)
            {
                // Lock the cursor
                Cursor.lockState = CursorLockMode.Locked;

                // Mark that the player is not in the inventory
                isInStopMenu = false;

                // Hide the stop menu and display the crosses
                stopMenuObject.SetActive(false);
                crosserObject.SetActive(true);
            }
            else
            {
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;
                
                // Mark that the player is in the inventory
                isInStopMenu = true;

                // Display the stop menu and Hide the crosses
                stopMenuObject.SetActive(true);
                crosserObject.SetActive(false);
            }

            // Reset all the animations
            ResetAnimations();
        }
    }

    /*
     * Toggle the inventory menu by other reasons
     */
    public void ToggleStopMenu()
    {
        if (!isInInventory && !isInLoadingScreen)
        {
            // If trying to get out of the stop menu
            if (isInStopMenu)
            {
                // Lock the cursor
                Cursor.lockState = CursorLockMode.Locked;

                // Mark that the player is not in the inventory
                isInStopMenu = false;

                // Hide the stop menu and display the crosses
                stopMenuObject.SetActive(false);
                crosserObject.SetActive(true);
            }
            else
            {
                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;

                // Mark that the player is in the inventory
                isInStopMenu = true;

                // Display the stop menu and Hide the crosses
                stopMenuObject.SetActive(true);
                crosserObject.SetActive(false);
            }

            // Reset all the animations
            ResetAnimations();
        }
    }

    /*
     * Close a menu by the ESC button
     */
    public void CloseMenuByButton(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        // If the player is in the inventory, close it (by toggling it)
        if (isInInventory)
        {
            ToggleInventory();
        }
        // Otherwise, toggle the stop menu
        else
        {
            ToggleStopMenu();
        }
    }

    /*
     * Make the character visible or invisible
     */
    public void SetCharacterVisible(bool isVisible)
    {
        foreach (var i in playerMeshes) i.SetActive(isVisible);
    }

    void Awake()
    {
        // Destroy the player's logic if the current character is not local
        if (!photonView.IsMine)
        {
            foreach (GameObject obj in localPlayerLogic)
            {
                Destroy(obj);
            }
            Destroy(gameObject);
            return;
        }

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Set the inventory instance
        inventory = Inventory.Instance;
        
        // Hide the inventory and stop menus
        inventoryObject.SetActive(false);
        stopMenuObject.SetActive(false);

        // Make the character invisible for the local player
        SetCharacterVisible(false);
    }

    void Update()
    {
        if (isInLoadingScreen) return;
        // Unlock the cursor if the player is dead
        if (playerHealth.Health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        // Update the player's gravity and coordinates
        UpdateGravity();
        UpdateCoordinates();

        if (!isInInventory && !isInStopMenu)
        {
            // Move the character using the last player's input (movingDirection)
            Vector3 movingVec = Vector3.zero;
            movingVec = (Camera.main.transform.forward * movingDirection.y + Camera.main.transform.right * movingDirection.x);
            movingVec.y = 0;

            // Set the speed and animation based on wether the player is sprinting or not
            if (!isSprinting)
            {
                animator.SetBool("IsSprinting", false);
                if (movingVec.x == 0 && movingVec.z == 0) animator.SetBool("IsWalking", false);
                else animator.SetBool("IsWalking", true);
            } 
            else
            {
                animator.SetBool("IsWalking", false);
                if (movingVec.x == 0 && movingVec.z == 0) animator.SetBool("IsSprinting", false);
                else animator.SetBool("IsSprinting", true);
            }
            // Move the characters
            controller.Move(Time.deltaTime * (isSprinting ? sprintSpeed : speed) * movingVec);

            // Hit Logic
            if (isHitKeyPressed) Hit();
            // Use Logic
            if (isUse) Use();

        }
        if (inventory != null)
        {
            // Wear the current worn armor and hold the current held item
            WearCurrentArmor();
            HoldCurrentItem();
        }

    }
}
