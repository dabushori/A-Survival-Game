using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenuInitializer : MonoBehaviour
{
    [SerializeField]
    GameObject recipeCraftingMenuItemPrefab;
    void Start()
    {
        foreach (Recipe r in RecipesDatabase.handRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    public void OnCraftingTable()
    {
        foreach (Recipe r in RecipesDatabase.craftingTableRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    public void OnFurnace()
    {
        foreach (Recipe r in RecipesDatabase.furnaceRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    public void OnAnvil()
    {
        foreach (Recipe r in RecipesDatabase.anvilRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }


    public void restartCraftingMenu()
    {
        foreach (Transform transformChild in transform)
        {
            Destroy(transformChild.gameObject);
        }
        Start();
    }
}
