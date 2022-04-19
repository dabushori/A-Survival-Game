using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    Slider healthBar;

    public int Health
    {
        get
        {
            return (int) healthBar.value;
        }
        private set
        {
            healthBar.value = Mathf.Min(value, healthBar.maxValue);
            // death logic
            // if healthBar.value <= 0 { ... }
        }
    }

    public void DealDamage(int damage)
    {
        float defenseLevel = Inventory.Instance.GetCurrentDefenseLevel();
        Health -= (int) (damage * (100 - defenseLevel)) / 100;
    }

    public void AddHealth(int helathToAdd)
    {
        Health += helathToAdd;
    }
}
