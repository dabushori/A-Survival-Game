using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Destructable : MonoBehaviour
{
    [SerializeField]
    private int hp;

    [SerializeField]
    private GameObject item;

    public bool IsUserInRange()
    {
        return true;
    }

    bool isDestructable = false;
/*
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("enter");
        isDestructable = true;
    }

    private void OnCollisionExit(Collision col)
    {
        Debug.Log("exit");
        isDestructable = false;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.isPressed)
        {
            Debug.Log("clicked");
            if (IsUserInRange()) // user is close enough
            {
                int damage = 50; // the damage the user has done
                hp -= damage;
                if (hp < 0)
                {
                    Destroy(gameObject); // destroy object
                    // GiveObjectToUser(); // give user whatever
                }
            }
        }
    }*/
}
