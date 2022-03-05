using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private int hp;

    public int HP
    {
        get
        {
            return hp;
        }
    }

    [SerializeField]
    private GameObject item;

    public void Hit(int damage)
    {
        if (hp > 0)
        {
            hp -= damage;
        }
        if (hp <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Destroyed");
            // GiveObjectToUser(); // give user whatever
        }
    }
}
