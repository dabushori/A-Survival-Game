using UnityEngine;

/*
 * A class to set up the parameters of the result slot in a recipe
 */
public class ResultSlotSetup : SlotSetup
{
    Recipe recipe;

    /*
     * Set up the parameters of the slot
     */
    public void SetUp(int amount, Sprite icon, Recipe recipe)
    {
        SetUp(amount, icon);
        this.recipe = recipe;
    }

    /*
     * Craft the item using the user's inventory and the recipe
     */
    public void CraftItem()
    {
        RecipesDatabase.Craft(Inventory.Instance, recipe);
    }
}
