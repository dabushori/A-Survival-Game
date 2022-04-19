using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCraftingMenuItem : MonoBehaviour
{
    [SerializeField]
    GameObject ingredientSlot, resultSlot, plusSign, arrow;
    [SerializeField]
    int FIXED_SLOT_SIZE;

    public void CreateRecipe(Recipe recipe)
    {
        SlotSetup s = Instantiate(ingredientSlot, transform).GetComponent<SlotSetup>();
        s.SetUp(recipe.amounts[0], recipe.ingredients[0].icon);
        int i;
        for (i = 1; i < recipe.ingredients.Length; ++i)
        {
            Instantiate(plusSign, transform);
            s = Instantiate(ingredientSlot, transform).GetComponent<SlotSetup>();
            s.SetUp(recipe.amounts[i], recipe.ingredients[i].icon);
        }
        Instantiate(arrow, transform);
        ResultSlotSetup rs = Instantiate(resultSlot, transform).GetComponent<ResultSlotSetup>();
        rs.SetUp(recipe.amountOfCraftedItem, recipe.craftedItem.icon, recipe);
    }
}
