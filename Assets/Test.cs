using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Init), 3);
    }

    private void Init()
    {
        foreach (var r in RecipesDatabase.furnaceRecipes) Inventory.Instance.AddToInventory(r.craftedItem, 10);
        foreach (var r in RecipesDatabase.anvilRecipes) Inventory.Instance.AddToInventory(r.craftedItem, 10);
    }
}
