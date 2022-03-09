using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public  class Item : ScriptableObject
{
    new public string name = "New Item"; // item name
    public Sprite icon = null; // path to image 
    public int breakDamage = 50; // damage done to destructible objects
    public bool usable = true; // can be used in the world (eat / build / something)
}
