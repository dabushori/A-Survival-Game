using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    float MINING_DISTANCE = 2f;
    int DISTRUCTABLE_LAYER = 1 << 7;

    bool isPressed = false;
    DateTime lastClick;
    TimeSpan miningTime = TimeSpan.FromSeconds(1);

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

    private void Update()
    {
        UpdatePressed();
        if (Pointer.current.press.isPressed && (lastClick + miningTime) < DateTime.Now)
        {
            lastClick = DateTime.Now;
            Vector3 pos = gameObject.transform.position;
            if (Physics.Raycast(pos, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTABLE_LAYER, QueryTriggerInteraction.Collide))
            {
                hit.transform.gameObject.GetComponent<Destructible>().Hit(50);
            }
        }
    }
}
