using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item"; // item name

    
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }
    }
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }

    
    [EnumFlagsAttribute]
    public Jobs job; // what the item can be used for
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

    public static bool CanBreak(BreakLevel toolBreakLevel , BreakLevel levelNeededToBreak)
    {
        return BreakLevel.WOOD >= levelNeededToBreak;
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

public enum Jobs { MINING = 1, FIGHTING = 2, FOOD = 4, ARMOR = 8 }
public enum BreakLevel
{
    WOOD, STONE, IRON, GOLD, DIAMOND
}
public enum HitLevel
{
    // to fill
}
public enum BodyPart { HEAD, CHEST, LEGS, FEET }
