using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * The database of all the recipes in the game
 */
public class RecipesDatabase
{
    // recipes that can be used anytime
    public static List<Recipe> handRecipes = new List<Recipe>(Resources.LoadAll("Recipes/HandRecipes", typeof(Recipe)).Cast<Recipe>());
    
    // recipes that can be used only in the crafting table
    public static List<Recipe> craftingTableRecipes = new List<Recipe>(Resources.LoadAll("Recipes/CraftingTableRecipes", typeof(Recipe)).Cast<Recipe>());
    
    // recipes that can be used only in the anvil
    public static List<Recipe> anvilRecipes = new List<Recipe>(Resources.LoadAll("Recipes/AnvilRecipes", typeof(Recipe)).Cast<Recipe>());
    
    // recipes that can be used only in the furnace
    public static List<Recipe> furnaceRecipes = new List<Recipe>(Resources.LoadAll("Recipes/FurnaceRecipes", typeof(Recipe)).Cast<Recipe>());

    /*
     * Check if a user can craft an item using the given recipe and inventory
     */
    public static bool CanUserCraft(Inventory inventory, Recipe recipe)
    {
        // check that there is a place for the crafted item
        if (inventory.isFull()) return false;
        for (int i = 0; i < recipe.ingredients.Length; ++i)
        {
            // check that there is enough of the ingredient
            if (inventory.GetAmountOfItem(recipe.ingredients[i]) < recipe.amounts[i]) return false;
        }
        return true;
    }

    /*
     * Craft an item using the given recipe and inventory
     */
    public static bool Craft(Inventory inventory, Recipe recipe)
    {
        // check that the item can be crafted
        if (!CanUserCraft(inventory, recipe)) return false;
        for (int i = 0; i < recipe.ingredients.Length; ++i)
        {
            // remove the ingredients from the user's inventory
            inventory.RemoveFromInventory(recipe.ingredients[i], recipe.amounts[i]);
        }
        // add the crafted item to the user's inventory
        inventory.AddToInventory(recipe.craftedItem, recipe.amountOfCraftedItem);
        return true;
    }
}
