using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public  class Item : ScriptableObject
{
    new public string name = "New Item"; // item name
    public int breakDamage = 50; // damage done to destructible objects
    public int hitDamage = 50; // damage done to mobs
    public bool usable = true; // can be used in the world (eat / build / something)
    public Sprite icon = null; // path to image 
}
