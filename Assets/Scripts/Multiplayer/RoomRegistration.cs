using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

/*
 * A class that is responsible for the registration to the different rooms
 */
public class RoomRegistration : MonoBehaviourPunCallbacks
{
    /*
     * Room creation objects
     */

    // The world seed input
    [SerializeField]
    TMP_InputField worldSeedInput;

    // The random seed option input
    [SerializeField]
    Toggle randomSeedInput;
    
    // The room name input
    [SerializeField]
    TMP_InputField roomNameInput_create;

    // The create button
    [SerializeField]
    Button createButton;
    
    // The error message's text
    [SerializeField]
    TMP_Text failedToCreateText;


    /*
     * Room joining objects
     */

    // The room name input
    [SerializeField]
    TMP_InputField roomNameInput_join;
    
    // The join button
    [SerializeField]
    Button joinButton;

    // The error message's text
    [SerializeField]
    TMP_Text failedToJoinText; 


    /*
     * Room menu objects
     */

    // Room's name
    [SerializeField]
    TMP_Text roomName;

    // Room's seed
    [SerializeField]
    TMP_Text roomSeed;

    // Start button (only displayed to the player who created the room)
    [SerializeField]
    Button startButton;

    // Leave room button 
    [SerializeField]
    Button leaveButton;


    /*
     * Menus
     */

    // Room menu (wait for a room to start)
    [SerializeField]
    GameObject roomMenu;

    // Create Room menu (enter a room's name and seed & create it)
    [SerializeField]
    GameObject createMenu;

    // Join Room menu (enter a room's name & join it)
    [SerializeField]
    GameObject joinMenu;

    // Opening Menu (enter a name & connect to server)
    [SerializeField]
    GameObject openMenu;

    // The Main Menu
    [SerializeField]
    GameObject mainMenu;

    // The loading screen
    [SerializeField]
    GameObject loadingPage;


    /*
     * Players list
     */

    // The amount of players currently in the room
    [SerializeField]
    TMP_Text playersAmount;

    // The transform that is the parent of the players' text objects
    [SerializeField]
    Transform contentTransform;
    
    // A list contains all the player's in the current room
    List<PlayerItem> players = new List<PlayerItem>();

    // The playerItem prefab
    [SerializeField]
    GameObject playerItemPrefab;


    /*
     * A function that is called when the create button is pressed. 
     * This function validates the input, and (if needed) generates a random seed for the world. 
     * Then it creates a room with the given input parameters.
     */
    public void OnClickCreate()
    {
        // If the player wants to generate a random seed
        if (randomSeedInput.isOn)
        {
            // Ensure that the room name is enterred
            if (roomNameInput_create.text.Length > 0)
            {
                failedToCreateText.text = "";
                createButton.interactable = false;

                // Create the room with a random seed
                CreateRoom(roomNameInput_create.text, Random.Range(-1000000, 1000000));
                return;
            }
        } 
        else
        {
            // Ensure that the seed is a valid number and that the room name is enterred
            if (int.TryParse(worldSeedInput.text, out int seed) && roomNameInput_create.text.Length > 0)
            {
                failedToCreateText.text = "";
                createButton.interactable = false;

                // Create the room with the given input parameters
                CreateRoom(roomNameInput_create.text, seed);
                return;
            }
        }

        // Display an error 
        failedToCreateText.text = "Can't create the room";
    }

    /*
     * Create a room with the given name and world seed and join it
     */
    void CreateRoom(string name, int worldSeed)
    {
        RoomOptions options = new RoomOptions();

        // Generate the custom properties for the new room
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties["seed"] = worldSeed;
        properties["playersInGame"] = 0;
        properties["readyPlayers"] = 0;
        options.CustomRoomProperties = properties;

        // Create the room (and join it) using the generated properties
        PhotonNetwork.CreateRoom(name, options);
    }

    /*
     * A function that is called if failed to connect to a room.
     * This functino displays an error in that case and makes the create button interactable again.
     */
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        failedToCreateText.text = "Can't create the room";
        createButton.interactable = true;
    }

    /*
     * A function that is called when joining a room.
     * This function displays the room's menu and hides the others, displays the current room parameters and updates the players list.
     */
    public override void OnJoinedRoom()
    {
        // Activate the room menu
        joinMenu.SetActive(false);
        createMenu.SetActive(false);
        roomMenu.SetActive(true);
        
        // Set the name and seed of the current room
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        roomSeed.text = PhotonNetwork.CurrentRoom.CustomProperties["seed"].ToString();

        // Update the players list
        UpdatePlayersList();

        // If the player is the master client (the one who created the room), display the start button
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    /*
     * A function that is called whenever a player clicks on the leave button.
     * This function Leaves the current room.
     */
    public void OnClickLeave()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            leaveButton.interactable = false;
            PhotonNetwork.LeaveRoom();
        }
    }

    /*
     * A function that is called whenever a player leaves a room.
     * This function displays the main menu and hides the others, updates the players list and makes all the buttons interactable again. 
     */
    public override void OnLeftRoom()
    {
        // Display the main menu and hide the others
        roomMenu.SetActive(false);
        mainMenu.SetActive(true);

        // Update the players list
        UpdatePlayersList();

        // Make the buttons interactable again
        leaveButton.interactable = true;
        createButton.interactable = true;
        joinButton.interactable = true;
        startButton.interactable = true;
    }

    /*
     * Join the given room.
     */
    public void JoinRoom(string name)
    {
        if (PhotonNetwork.CurrentRoom == null) PhotonNetwork.JoinRoom(name);
    }

    /*
     * Update the player list 
     */
    public void UpdatePlayersList()
    {
        // Destroy all the current text objects and clear the players list
        foreach (var player in players) Destroy(player.gameObject);
        players.Clear();
        // If the player is somehow not in a room, mark the amount as 0 and return
        if (PhotonNetwork.CurrentRoom == null)
        {
            playersAmount.text = "0 Players";
            return;
        }

        // Write the amount of players to the amount text object
        playersAmount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + " Players";

        // For every player in the room, instantiate a new player item with a text object containing the player's name and add it to the list
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem item = Instantiate(playerItemPrefab, contentTransform).GetComponent<PlayerItem>();
            item.SetNickName(player.Value.NickName);
            players.Add(item);
        }
    }

    /*
     * A function that is called when the join room button is called.
     * This function validates that the user entered a room's name and tries to join it.
     */
    public void OnClickJoin()
    {
        // Ensure that the room name is enterred
        if (roomNameInput_join.text.Length > 0)
        {
            failedToJoinText.text = "";
            joinButton.interactable = false;

            // Join the room with the given name
            JoinRoom(roomNameInput_join.text);
            return;
        }

        // Show an error if failed to join to the room
        failedToJoinText.text = "Can't connect to the room";
    }

    /*
     * This function is called if joining a room has failed.
     * It writes an error message to the text object and makes the join button interactable again.
     */
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Show an error if failed to join to the room
        failedToJoinText.text = "Can't connect to the room";

        // Make the join button interactable again
        joinButton.interactable = true;
    }

    /*
     * This function is called when a player joins the room.
     */
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update the players list
        UpdatePlayersList();
    }

    /*
     * This function is called when a player leaves the room.
     */
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Update the players list
        UpdatePlayersList();
    }

    /*
     * This function is called when the master client leaves the room and it switched to another player.
     * It enables the start button for the new master client.
     */
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    /*
     * A function that is called when the start button is pressed (by the master client of course).
     * It closes the room for new joins and moves all the players in the room to the world scene.
     */
    public void OnStartClick()
    {
        // Make the buttons un-interactable
        startButton.interactable = false;
        leaveButton.interactable = false;

        // Close the room for joining 
        PhotonNetwork.CurrentRoom.IsOpen = false;

        // Call the StartWorld RPC to move all the players to the world's scene
        PhotonView.Get(this).RPC("StartWorld", RpcTarget.All);
    }

    /*
     * An RPC to move the local player to the world scene
     */
    [PunRPC]
    public void StartWorld()
    {
        // Move to the world scene (asynchronously)
        var scene = SceneManager.LoadSceneAsync("World");

        // Don't move to the scene when it's ready
        scene.allowSceneActivation = false;
        
        // Hide the room menu and display the loading screen
        roomMenu.SetActive(false);
        loadingPage.SetActive(true);

        // Move to the scene when it's ready, now that everythign is prepared
        scene.allowSceneActivation = true;
    }
    

    private void Awake()
    {
        // Disable scene syncronization
        PhotonNetwork.AutomaticallySyncScene = false;

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        
        // If connected to the server, leave the current room (if there is one) and display the main menu
        if (PhotonNetwork.IsConnected)
        {
            mainMenu.SetActive(true);
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        // Otherwise, display the open menu (enter name & connect to server)
        else
        {
            openMenu.SetActive(true);
        }
    }
}
