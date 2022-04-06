using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public  class Item : ScriptableObject
{
    new public string name = "New Item"; // item name
    public int breakDamage = 20; // amount of damage it deals by breaking
    public int hitDamage = 20; // amount of damage it deals by hitting enemies
    public Sprite icon = null; // path to image 
}
