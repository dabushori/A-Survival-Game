using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.InputSystem.InputAction;

public class Testing : MonoBehaviour
{
    Inventory inventory;
    public List<Item> items;
    public PlayerHealth health;
    public List<GameObject> agents;
    void Update()
    {
        if(items != null)
        {
            inventory = Inventory.Instance;
            foreach (Item i in items)
            {
                inventory.AddToInventory(i, 100);
            }
            items = null;
            foreach (GameObject g in agents)
            {
                Instantiate(g, new Vector3(10, 10, 10), Quaternion.identity);
            }
        }
    }

    public void Fun(CallbackContext ctx)
    {
    }
}
