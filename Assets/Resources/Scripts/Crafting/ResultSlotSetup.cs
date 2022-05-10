using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSlotSetup : SlotSetup
{
    Recipe recipe;

    public void SetUp(int amount, Sprite icon, Recipe recipe)
    {
        SetUp(amount, icon);
        this.recipe = recipe;
    }

    public void CraftItem()
    {
        RecipesDatabase.Craft(Inventory.Instance, recipe);
    }
}
