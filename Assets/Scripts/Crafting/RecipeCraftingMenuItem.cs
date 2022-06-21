using UnityEngine;

/*
* A class to create the recipe's view in the crafting menu using a Recipe object
*/
public class RecipeCraftingMenuItem : MonoBehaviour
{
    [SerializeField]
    GameObject ingredientSlot, resultSlot, plusSign, arrow;
    [SerializeField]
    int FIXED_SLOT_SIZE;
    [SerializeField]
    public MenuType menuType;

    /*
     * Create the recipe view using the given recipe (ingredient 1 + ingredient 2 + ... + ingredient n -> result item)
     */
    public void CreateRecipe(Recipe recipe)
    {
        // Create an ingredientSlot for each ingredient and set up its parameters
        SlotSetup s = Instantiate(ingredientSlot, transform).GetComponent<SlotSetup>();
        s.SetUp(recipe.amounts[0], recipe.ingredients[0].icon);
        int i;
        for (i = 1; i < recipe.ingredients.Length; ++i)
        {
            // Create the plus sign (+)
            Instantiate(plusSign, transform);
            s = Instantiate(ingredientSlot, transform).GetComponent<SlotSetup>();
            s.SetUp(recipe.amounts[i], recipe.ingredients[i].icon);
        }
        // Create the arrow sign (->)
        Instantiate(arrow, transform);
        // Create the reault item's slot and set up its parameters
        ResultSlotSetup rs = Instantiate(resultSlot, transform).GetComponent<ResultSlotSetup>();
        rs.SetUp(recipe.amountOfCraftedItem, recipe.craftedItem.icon, recipe);
    }
}
public enum MenuType {CRAFTING_TABLE = 1, FURNACE, ANVIL }
