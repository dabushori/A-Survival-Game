using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipesDatabase
{
    public static List<Recipe> handRecipes = new List<Recipe>(Resources.LoadAll("Recipes/HandRecipes", typeof(Recipe)).Cast<Recipe>());
    public static List<Recipe> craftingTableRecipes = new List<Recipe>(Resources.LoadAll("Recipes/CraftingTableRecipes", typeof(Recipe)).Cast<Recipe>());
    public static List<Recipe> anvilRecipes = new List<Recipe>(Resources.LoadAll("Recipes/AnvilRecipes", typeof(Recipe)).Cast<Recipe>());
    public static List<Recipe> furnaceRecipes = new List<Recipe>(Resources.LoadAll("Recipes/FurnaceRecipes", typeof(Recipe)).Cast<Recipe>());

    public static bool CanUserCraft(Inventory inventory, Recipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; ++i)
        {
            if (inventory.GetAmountOfItem(recipe.ingredients[i]) < recipe.amounts[i]) return false;
            if (inventory.isFull()) return false;
        }
        return true;
    }

    public static bool Craft(Inventory inventory, Recipe recipe)
    {
        if (!CanUserCraft(inventory, recipe)) return false;
        for (int i = 0; i < recipe.ingredients.Length; ++i)
        {
            inventory.RemoveFromInventory(recipe.ingredients[i], recipe.amounts[i]);
        }
        inventory.AddToInventory(recipe.craftedItem, recipe.amountOfCraftedItem);
        return true;
    }
}
