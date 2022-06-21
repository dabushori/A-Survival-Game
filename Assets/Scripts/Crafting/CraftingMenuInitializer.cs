using UnityEngine;

/*
* A class that inializes the crafting menu based on the object we used to view the menu
*/
public class CraftingMenuInitializer : MonoBehaviour
{
    [SerializeField]
    GameObject recipeCraftingMenuItemPrefab;
    void Start()
    {
        // Initialize all the hand recipes by default
        foreach (Recipe r in RecipesDatabase.handRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    public void OnCraftingTable()
    {
        // Initialize all the crafting table recipes
        foreach (Recipe r in RecipesDatabase.craftingTableRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    public void OnFurnace()
    {
        // Initialize all the furnace recipes
        foreach (Recipe r in RecipesDatabase.furnaceRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    public void OnAnvil()
    {
        // Initialize all the anvil recipes
        foreach (Recipe r in RecipesDatabase.anvilRecipes)
        {
            RecipeCraftingMenuItem recipeMenuItem = Instantiate(recipeCraftingMenuItemPrefab, transform).GetComponent<RecipeCraftingMenuItem>();
            recipeMenuItem.CreateRecipe(r);
        }
    }

    // Restart the crafting menu
    public void restartCraftingMenu()
    {
        // Remove all the existing recipes
        foreach (Transform transformChild in transform)
        {
            Destroy(transformChild.gameObject);
        }
        // Initialize the hand recipes by default
        Start();
    }
}
