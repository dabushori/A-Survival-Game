using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeCraftingMenuItem : MonoBehaviour
{
    [SerializeField]
    GameObject slot, plusSign, arrow;
    [SerializeField]
    int FIXED_SLOT_SIZE;

    public void CreateRecipe(Recipe recipe)
    {
        SlotSetup s = Instantiate(slot, transform).GetComponent<SlotSetup>();
        s.SetUp(recipe.amounts[0], recipe.ingredients[0].icon);
        int i;
        for (i = 1; i < recipe.ingredients.Length; ++i)
        {
            Instantiate(plusSign, transform);
            s = Instantiate(slot, transform).GetComponent<SlotSetup>();
            s.SetUp(recipe.amounts[i], recipe.ingredients[i].icon);
        }
        Instantiate(arrow, transform);
        s = Instantiate(slot, transform).GetComponent<SlotSetup>();
        s.SetUp(recipe.amountOfCraftedItem, recipe.craftedItem.icon);
    }
}
