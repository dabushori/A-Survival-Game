using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item", menuName = "Game Data/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item"; // item name
    

    public Sprite icon; // path to image (icon)
    
    public Jobs job; // what the item can be used for

    /**
     * checking if the item can be used for this job
     */
    public bool IsSuitableForJob(Jobs wantedJob)
    {
        int jobValue = (int)wantedJob;
        // return (job & (1 << jobValue)) != 0;
        return (job & wantedJob) != 0;
    }
    
    // mining
    [SerializeField]
    public int breakDamage; // amount of damage it deals when breaking
    public BreakLevel breakLevel;

    /**
     * checking if the item can break (match the break level)
     */
    public static bool CanBreak(BreakLevel toolBreakLevel , BreakLevel levelNeededToBreak)
    {
        return toolBreakLevel >= levelNeededToBreak;
    }

    // fighting
    public int hitDamage; // amount of damage it deals when hitting enemies

    // armor
    public BodyPart bodyPart; // body part of the armor
    public int defenseLevel; // level of defense (between 0-100, represent the protection precent of the damage, 10 means only 90% of the damage will be dealt)

    // food
    public int hpBonus;

    //placeable
    public bool placeable;
    public GameObject placedObject;

    public GameObject itemToHold;
}

public enum Jobs { MINING = 1, FIGHTING = 2, FOOD = 4, ARMOR = 8 }
public enum BreakLevel
{
    WOOD, STONE, GOLD, IRON, DIAMOND
}
public enum BodyPart { HEAD = 0, CHEST = 1, LEGS = 2, FEET = 3}
