using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private int hp;

    [SerializeField]
    public GameObject floatingPointsPrefab;

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
            PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
        }
        if (hp <= 0)
        {
            Destroy(gameObject);
            // GiveObjectToUser(); // give user whatever
        }
    }
}
