using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipesDatabase
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
    public class Recipe : ScriptableObject
    {
        public Item[] ingredients;
        public int[] amounts;
        public Item craftedItem;
        public int amountOfCraftedItem;
    }

    public static List<Recipe> recipes = new List<Recipe>(Resources.LoadAll("Recipes", typeof(Recipe)).Cast<Recipe>());

    public bool CanUserCraft(Inventory inventory, Recipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; ++i)
        {
            if (inventory.GetAmountOfItem(recipe.ingredients[i]) < recipe.amounts[i]) return false;
        }
        return true;
    }

    public bool Craft(Inventory inventory, Recipe recipe)
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
