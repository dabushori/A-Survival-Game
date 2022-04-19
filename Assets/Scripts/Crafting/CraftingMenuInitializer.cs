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
}
