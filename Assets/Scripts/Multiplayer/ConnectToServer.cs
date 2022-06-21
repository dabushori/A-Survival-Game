using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

/*
 * A script for the first menu displayed where the player enters his name and connects to the server
 */
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // The player's name input field
    public TMPro.TMP_InputField nameInput;

    // The connect button's text object
    public TMPro.TMP_Text connectText;

    // The connect button
    public Button connectButton;

    // The main manu game object
    public GameObject mainMenu;

    /*
     * Validate the input name and connect to the server
     */
    public void OnConnect()
    {
        // Validate that the player entered his name
        if (nameInput.text.Length >= 1)
        {
            // Set the local player's nickname as the enterred name
            PhotonNetwork.NickName = nameInput.text;

            // Change the button's text to "Connecting..."
            connectText.text = "Connecting...";

            // Make the button un-interactable 
            connectButton.interactable = false;
            
            // Connect to the game server
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /*
     * A Photon callback that will be called once connected to the server
     */
    public override void OnConnectedToMaster()
    {
        // Make the connect button interactable again (for future uses)
        connectButton.interactable = true;

        // Hide the current menu
        gameObject.SetActive(false);

        // Display the main menu
        mainMenu.SetActive(true);
    }
}
