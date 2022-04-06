using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/Tools")]
public class Tools : Item
{
    public Job job; // job of the tool
    public int breakLevel; // level of breaking
}
public enum Job { Mining, Chopping, Fighting}
