using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    public GameObject playerBody; // player's body to move (the whole prefab)

    private float xRotation;
    private float mouseSensitivity = 20f;
    public void Look(InputAction.CallbackContext ctx)
    {
        Vector2 mouse = ctx.ReadValue<Vector2>();
        mouse = mouse * Time.deltaTime * mouseSensitivity;

        xRotation -= mouse.y;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerBody.transform.Rotate(Vector3.up * mouse.x);
    }

    private float speed = 12f;
    private float sprintSpeed = 20f;
    public CharacterController controller;
    private Vector2 movingDirection;
    public void Move(InputAction.CallbackContext ctx)
    {
        movingDirection = ctx.ReadValue<Vector2>();
        // movingDirection = Camera.main.transform.forward * dir.y + Camera.main.transform.right * dir.x;
    }

    [SerializeField]
    private float gravity = -9.81f * 2;
    private Vector3 velocity = Vector3.zero;

    public LayerMask GROUND_LAYER;
    void UpdateGravity()
    {
        if (Physics.CheckSphere(playerBody.transform.position, 0.5f, GROUND_LAYER) && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        // Debug.Log(velocity * Time.deltaTime);
    }

    float jumpPower = 3f;
    public void Jump(InputAction.CallbackContext ctx)
    {
        // if (ctx.performed) controller.Move(Vector3.up * jumpPower);
        if (ctx.performed && Physics.CheckSphere(playerBody.transform.position, 0.5f, GROUND_LAYER) && velocity.y < 0) velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
    }

    private bool isSprinting = false;
    public void Sprint(InputAction.CallbackContext ctx)
    {
        isSprinting = !ctx.canceled;
    }

    /*
    private LayerMask DISTRUCTABLE_LAYER;
    private float MINING_DISTANCE = 2f;
    */
    private bool isHit = false;
    private double hitStartTime;
    public void Hit(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isHit = true;
            hitStartTime = ctx.startTime;
        } else if (ctx.canceled)
        {
            isHit = false;
            hitStartTime = 0;
        }
    }

    public void Hit()
    {
        Debug.Log("hit");
        // Hit Logic
        /*
        if (
            CurrentItem != null && CurrentItem.CanBreak && 
            Physics.Raycast(gameObject.transform.position, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTABLE_LAYER, QueryTriggerInteraction.Collide))
        {
            // hit logic
            hit.transform.gameObject.GetComponent<Destructible>().Hit(50);
        }
        */
    }

    private bool isUse = false;
    private double useStartTime;

    public void Use(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isUse = true;
            useStartTime = ctx.startTime;
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
    }

    private void Update()
    {
        controller.Move(Time.deltaTime * (isSprinting ? sprintSpeed : speed) * (Camera.main.transform.forward * movingDirection.y + Camera.main.transform.right * movingDirection.x)); // moving
        // Gravity 
        UpdateGravity();

        // Hit Logic
        if (isHit) Hit();
        // Use Logic
        if (isUse) Use();
        
    }


}
