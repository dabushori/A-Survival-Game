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

    public int DEFAULT_BREAKING_DAMAGE = 20, DEFAULT_HITTING_DAMAGE = 20;

    public void Break(Inventory inventory)
    {
        Item chosenItem = inventory.ChosenItem;
        int damage;
        BreakLevel toolBreakLevel;
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.MINING) || (levelNeededToBreak == BreakLevel.WOOD && chosenItem.breakLevel != BreakLevel.WOOD))
        {
            damage = DEFAULT_BREAKING_DAMAGE;
            toolBreakLevel = BreakLevel.WOOD;
        } 
        else
        {
            damage = chosenItem.breakDamage;
            toolBreakLevel = chosenItem.breakLevel;
        }

        if (Item.CanBreak(toolBreakLevel, levelNeededToBreak))
        {
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

    public void Hit(Inventory inventory)
    {
        Item chosenItem = inventory.ChosenItem;
        int damage;
        // HitLevel toolHitLevel;
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.FIGHTING))
        {
            damage = DEFAULT_HITTING_DAMAGE;
            // toolHitLevel = HitLevel.WOOD;
        }
        else
        {
            damage = chosenItem.hitDamage;
            // toolHitLevel = chosenItem.hitLevel;
        }

        // if (Item.CanHit(...))
        // {
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
        // }
    }
}