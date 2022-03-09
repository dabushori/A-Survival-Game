using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    Inventory inventory;
    public List<Item> items;

    private void Start()
    {
        for (int i = 0; i < 20; ++i)
        {
            Item it = new Item();
            it.name = "Item number " + i.ToString();
            items.Add(it);
        }
    }

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
