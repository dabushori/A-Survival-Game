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
    private Item[] items;
    [SerializeField]
    private int[] minItemsToGive, maxItemsToGive;
    [SerializeField]
    private BreakLevel levelNeededToBreak;

    public void Hit(int damage, Inventory inventory)
    {
        Item chosenItem = inventory.ChosenItem;
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.MINING) || !chosenItem.CanBreak(levelNeededToBreak)) return;
        if (hp > 0)
        {
            hp -= damage;
            PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
        }
        if (hp <= 0)
        {
            Destroy(gameObject);
            for (int i = 0; i < items.Length; ++i)
            {
                inventory.AddToInventory(items[i], Random.Range(minItemsToGive[i], maxItemsToGive[i] + 1));
            }
        }
    }
}