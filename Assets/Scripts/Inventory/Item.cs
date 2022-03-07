using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item 
{
    public abstract bool CanBreak { get; }
    public abstract bool CanBeBuilt { get; }

    public string name;
    public string image; // path to image 
}

public class Pickaxe : Item
{
    public Pickaxe() {
        name = "Pickaxe";
        image = null;
    }

    public override bool CanBreak { 
        get
        {
            return true;
        }
    }
    public override bool CanBeBuilt { 
        get
        {
            return false;
        }
    }


}
