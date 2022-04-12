using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item"; // item name

    [SerializeField]
    private int job; // what the item can be used for
    public bool IsSuitableForJob(Jobs wantedJob)
    {
        int jobValue = (int)wantedJob;
        return (job & (1 << jobValue)) != 0;
    }
    
    // mining
    public int breakDamage; // amount of damage it deals when breaking
    public BreakLevel breakLevel;

    public bool CanBreak(BreakLevel levelNeededToBreak)
    {
        return breakLevel >= levelNeededToBreak;
    }

    // fighting
    public int hitDamage; // amount of damage it deals when hitting enemies
    public HitLevel hitLevel;

    // armor
    public BodyPart bodyPart; // body part of the armor
    public float defenseLevel; // level of defense (between 0-1, every component will reduce 25% of the damage)

    // food
    public float hpBonus;

    public Sprite icon; // path to image (icon)
}

public enum Jobs { MINING, FIGHTING, FOOD, ARMOR }
public enum BreakLevel
{
    WOOD, STONE, IRON, GOLD, DIAMOND
}
public enum HitLevel
{
    // to fill
}
public enum BodyPart { HEAD, CHEST, LEGS, FEET }
