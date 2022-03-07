using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public Item(string name, string image)
    {
        this.name = name;
        this.image = image;
    }

    public string name;
    public string image; // path to image 
}

public class Pickaxe : Item
{
    public Pickaxe() : base("Pickaxe", null) {}
}
