using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Testing : MonoBehaviour
{
    Inventory inventory;
    public List<Item> items;
    public PlayerHealth health;

    void Update()
    {
        if(items != null)
        {
            inventory = Inventory.Instance;
            foreach (Item i in items)
            {
                inventory.AddToInventory(i, 5);
            }
            items = null;
        }
    }
}
