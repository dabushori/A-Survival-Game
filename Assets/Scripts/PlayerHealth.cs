using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    Slider healthBar;

    public float Health
    {
        get
        {
            return healthBar.value;
        }
        set
        {
            healthBar.value = value; 
        }
    }
}
