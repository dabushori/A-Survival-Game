using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomRegistration : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_InputField worldSeedInput;
    [SerializeField]
    Toggle randomSeedInput;
    [SerializeField]
    TMP_InputField roomNameInput_create;

    public void OnClickCreate()
    {
        if (randomSeedInput.isOn)
        {
            if (roomNameInput_create.text.Length > 0) CreateRoom(roomNameInput_create.text, Random.Range(-1000000, 1000000));
        } 
        else
        {
            if (int.TryParse(worldSeedInput.text, out int seed) && roomNameInput_create.text.Length > 0) CreateRoom(roomNameInput_create.text, seed);
        }
    }

    void CreateRoom(string name, int worldSeed)
    {
        RoomOptions options = new RoomOptions();
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties["seed"] = worldSeed;
        options.CustomRoomProperties = properties;
        PhotonNetwork.CreateRoom(name, options);
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
    GameObject mainMenu;
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
    }

    public void OnClickLeave()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        roomMenu.SetActive(false);
        mainMenu.SetActive(true);
        UpdatePlayersList();
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
    public void OnClickJoin()
    {
        if (roomNameInput_join.text.Length > 0) JoinRoom(roomNameInput_join.text);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayersList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayersList();
    }
}
