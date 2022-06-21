using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/*
 * A class represents the player health, which is connected to the UI element that shows it on the screen
 */
public class PlayerHealth : MonoBehaviourPunCallbacks
{
    // The UI element that shows the player's health
    [SerializeField]
    Slider healthBar;

    // The end screen (death message) game object
    [SerializeField]
    GameObject endScreen;

    // A property to get and (privately) set the player's health
    public int Health
    {
        get
        {
            // Return the slider's value
            return (int) healthBar.value;
        }
        private set
        {
            // Set the sliders value
            healthBar.value = Mathf.Min(value, MaxHealth);
            // Show death message if needed
            if (healthBar.value <= 0)
            {
                endScreen.SetActive(true);
            }
            if (healthBar.value == MaxHealth)
            {
                endScreen.SetActive(false);
            }
        }
    }

    // A property to get the maximum player's health
    public int MaxHealth
    {
        get
        {
            return (int) healthBar.maxValue;
        }
    }

    /*
     * Deal damage to the player (considering the current worn armor)
     */
    public void DealDamage(int damage)
    {
        float defenseLevel = Inventory.Instance.GetCurrentDefenseLevel();
        Health -= (int) (damage * (100 - defenseLevel)) / 100;
    }

    /*
     * Add health to the player (used when eating)
     */
    public void AddHealth(int healthToAdd)
    {
        Health += healthToAdd;
    }
}
