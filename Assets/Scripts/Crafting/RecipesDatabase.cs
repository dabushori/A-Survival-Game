using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipesDatabase
{
    public static List<Recipe> recipes = new List<Recipe>(Resources.LoadAll("Recipes", typeof(Recipe)).Cast<Recipe>());
    public static List<Recipe> handRecipes = new List<Recipe>(Resources.LoadAll("Recipes", typeof(Recipe)).Cast<Recipe>());

    public static bool CanUserCraft(Inventory inventory, Recipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; ++i)
        {
            if (inventory.GetAmountOfItem(recipe.ingredients[i]) < recipe.amounts[i]) return false;
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
