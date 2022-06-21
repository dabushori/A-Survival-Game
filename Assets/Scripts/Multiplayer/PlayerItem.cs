using TMPro;
using UnityEngine;

/*
 * This class represents a player in the room waiting menu
 */
public class PlayerItem : MonoBehaviour
{
    // The text object that will display the player's name
    [SerializeField]
    TMP_Text nameText;

    /*
     * Set the player's nickname
     */
    public void SetNickName(string name)
    {
        nameText.text = name;
    }
}
