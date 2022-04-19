using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    public Item[] ingredients;
    public int[] amounts;
    public Item craftedItem;
    public int amountOfCraftedItem;
}
