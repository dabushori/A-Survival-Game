using UnityEngine;

/*
* A recipe object
*/
[CreateAssetMenu(fileName = "New Recipe", menuName = "Game Data/Recipe")]
public class Recipe : ScriptableObject
{
    // The ingredients required for the recipe
    public Item[] ingredients;
    // The required amount of every ingredient
    public int[] amounts;
    // The crafted item
    public Item craftedItem;
    // The amount of the crafted item
    public int amountOfCraftedItem;
}
