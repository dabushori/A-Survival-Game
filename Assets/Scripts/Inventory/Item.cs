using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item", menuName = "Game Data/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item"; // item name
    

    public Sprite icon; // image of the item
    
    public Jobs job; // what the item can be used for

    /*
     * checking if the item can be used for this job
     */
    public bool IsSuitableForJob(Jobs wantedJob)
    {
        return (job & wantedJob) != 0;
    }
    
    // mining
    [SerializeField]
    public int breakDamage; // amount of damage it deals when breaking
    public BreakLevel breakLevel; // break level of the item

    /*
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
    public int hpBonus; // the health we heal from the item

    //placeable
    public bool placeable; // is the item placeable
    public GameObject placedObject; // the object that will be placed if the item is placeable

    public GameObject itemToHold; // the item that is shown in the player hand
}

// Jobs are being used to decide what is the usage of the item
public enum Jobs { MINING = 1, FIGHTING = 2, FOOD = 4, ARMOR = 8 }

// Break level is used to determine what can the item break
public enum BreakLevel
{
    WOOD, STONE, GOLD, IRON, DIAMOND
}

// BodyPart determine what is the body part of the armor
public enum BodyPart { HEAD = 0, CHEST = 1, LEGS = 2, FEET = 3}
