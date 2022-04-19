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
        private set
        {
            healthBar.value = value;
        }
    }

    public void DealDamage(int damage)
    {
        float defenseLevel = Inventory.Instance.GetCurrentDefenseLevel();
        Health -= damage * (100 - defenseLevel) / 100;
    }


}
