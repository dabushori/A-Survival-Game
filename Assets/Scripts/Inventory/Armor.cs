using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor")]
public class Armor : Item
{
    public BodyPart bodyPart; // body part of the armor
    public int defenseLevel; // level of defense
}

public enum BodyPart { Head, Chest, Legs, Feet }
