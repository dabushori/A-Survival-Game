using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;

public class RoomRegistration : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_InputField worldSeedInput;
    [SerializeField]
    Toggle randomSeedInput;
    [SerializeField]
    TMP_InputField roomNameInput_create;


    [SerializeField]
    Button createButton;
    [SerializeField]
    TMP_Text failedToCreateText;
    public void OnClickCreate()
    {
        if (randomSeedInput.isOn)
        {
            if (roomNameInput_create.text.Length > 0)
            {
                failedToCreateText.text = "";
                createButton.interactable = false;
                CreateRoom(roomNameInput_create.text, Random.Range(-1000000, 1000000));
                return;
            }
        } 
        else
        {
            if (int.TryParse(worldSeedInput.text, out int seed) && roomNameInput_create.text.Length > 0)
            {
                failedToCreateText.text = "";
                createButton.interactable = false;
                CreateRoom(roomNameInput_create.text, seed);
                return;
            }
        }
        failedToCreateText.text = "Can't create the room";
    }

    void CreateRoom(string name, int worldSeed)
    {
        RoomOptions options = new RoomOptions();
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties["seed"] = worldSeed;
        properties["playersInGame"] = 0;
        properties["readyPlayers"] = 0;
        options.CustomRoomProperties = properties;
        PhotonNetwork.CreateRoom(name, options);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        failedToCreateText.text = "Can't create the room";
        createButton.interactable = true;
    }

    [SerializeField]
    TMP_Text roomName;
    [SerializeField]
    TMP_Text roomSeed;
    [SerializeField]
    GameObject roomMenu;
    [SerializeField]
    GameObject createMenu;
    [SerializeField]
    GameObject joinMenu;
    [SerializeField]
    GameObject openMenu;
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameObject startButton;
    public override void OnJoinedRoom()
    {
        // logic for room joining

        // activate the room menu
        joinMenu.SetActive(false);
        createMenu.SetActive(false);
        roomMenu.SetActive(true);
        
        // set name
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        // set seed
        roomSeed.text = PhotonNetwork.CurrentRoom.CustomProperties["seed"].ToString();
        UpdatePlayersList();

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    [SerializeField]
    Button leaveButton;
    public void OnClickLeave()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            leaveButton.interactable = false;
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        roomMenu.SetActive(false);
        mainMenu.SetActive(true);
        UpdatePlayersList();
        // make the buttons interactable again
        leaveButton.interactable = true;
        createButton.interactable = true;
        joinButton.interactable = true;
        startButtonNO.interactable = true;
    }


    public void JoinRoom(string name)
    {
        if (PhotonNetwork.CurrentRoom == null) PhotonNetwork.JoinRoom(name);
    }

    [SerializeField]
    TMP_Text playersAmount;
    [SerializeField]
    Transform contentTransform;
    
    List<PlayerItem> players = new List<PlayerItem>();
    [SerializeField]
    GameObject playerItemPrefab;

    public void UpdatePlayersList()
    {
        foreach (var player in players) Destroy(player.gameObject);
        players.Clear();
        if (PhotonNetwork.CurrentRoom == null)
        {
            playersAmount.text = "0 Players";
            return;
        }
        playersAmount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + " Players";
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem item = Instantiate(playerItemPrefab, contentTransform).GetComponent<PlayerItem>();
            item.SetNickName(player.Value.NickName);
            players.Add(item);
        }
    }

    [SerializeField]
    TMP_InputField roomNameInput_join;
    [SerializeField]
    Button joinButton;
    [SerializeField]
    TMP_Text failedToJoinText; 
    public void OnClickJoin()
    {
        if (roomNameInput_join.text.Length > 0)
        {
            failedToJoinText.text = "";
            joinButton.interactable = false;
            JoinRoom(roomNameInput_join.text);
            return;
        }
        failedToJoinText.text = "Can't connect to the room";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        failedToJoinText.text = "Can't connect to the room";
        joinButton.interactable = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayersList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayersList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            startButton.SetActive(true);
        }
    }

    [SerializeField]
    Button startButtonNO;
    public void OnStartClick()
    {
        startButtonNO.interactable = false;
        leaveButton.interactable = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        //PhotonNetwork.LoadLevel("World");
        //LevelManager.Instance.LoadScene();
        PhotonView.Get(this).RPC("StartWorld", RpcTarget.All);
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Cursor.lockState = CursorLockMode.None;
        if (PhotonNetwork.IsConnected)
        {
            mainMenu.SetActive(true);
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        } 
        else
        {
            openMenu.SetActive(true);
        }
    }

    [PunRPC]
    public void StartWorld()
    {
        LoadScene();
        //PhotonNetwork.LoadLevel("World");
    }
    
    [SerializeField]
    GameObject loadingPage;
    private void LoadScene()
    {
        var scene = SceneManager.LoadSceneAsync("World");
        scene.allowSceneActivation = false;
        roomMenu.SetActive(false);
        loadingPage.SetActive(true);
        scene.allowSceneActivation = true;
    }


        public void resetFailedMessages()
    {
        failedToJoinText.text = "";
        failedToCreateText.text = "";
    }
}
