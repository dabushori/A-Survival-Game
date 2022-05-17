using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Slider healthBar;

    [SerializeField]
    GameObject endScreen;

    public int Health
    {
        get
        {
            return (int) healthBar.value;
        }
        private set
        {
            healthBar.value = Mathf.Min(value, healthBar.maxValue);
            if (healthBar.value <= 0)
            {
                endScreen.SetActive(true);
            }
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
