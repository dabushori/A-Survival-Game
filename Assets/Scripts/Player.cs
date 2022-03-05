using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    float MINING_DISTANCE = 2f;
    int DISTRUCTABLE_LAYER = 1 << 7;

    DateTime lastClick;
    TimeSpan delay = TimeSpan.FromSeconds(1);

    private void Update()
    {
        if (Pointer.current.press.isPressed && (lastClick + delay) < DateTime.Now)
        {
            lastClick = DateTime.Now;
            Vector3 pos = gameObject.transform.position;
            // if (Physics.Raycast(Camera.main.ScreenPointToRay(), out RaycastHit hit))
            if (Physics.Raycast(pos, Camera.main.transform.forward, out RaycastHit hit, MINING_DISTANCE, DISTRUCTABLE_LAYER, QueryTriggerInteraction.Collide))
            {
                Debug.Log("Hit! HP = " + hit.transform.gameObject.GetComponent<Destructible>().HP);
                Debug.Log(hit.transform.gameObject.GetComponent<Destructible>());
                hit.transform.gameObject.GetComponent<Destructible>().Hit(50);
            }
        }
    }

    private void Start()
    {
        // Camera.main.transform.forward = Vector3.forward;
    }
}
