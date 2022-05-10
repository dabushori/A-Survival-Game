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
    TMP_InputField roomNameInput;

    void OnClickCreate()
    {
        if (randomSeedInput.isOn)
        {
            if (roomNameInput.text.Length > 0) CreateRoom(roomNameInput.text, Random.Range(-1000000, 1000000));
        } 
        else
        {
            if (int.TryParse(worldSeedInput.text, out int seed) && roomNameInput.text.Length > 0) CreateRoom(roomNameInput.text, seed);
        }
        // change text to creating room ...
    }

    void CreateRoom(string name, int worldSeed)
    {
        RoomOptions options = new RoomOptions();
        options.CustomRoomProperties["seed"] = worldSeed;
        PhotonNetwork.CreateRoom(name, options);
    }

    [SerializeField]
    TMP_Text roomName;
    [SerializeField]
    TMP_Text roomSeed;
    public override void OnJoinedRoom()
    {
        // logic for room joining

        // activate the room menu
        
        // set name
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        // set seed
        roomSeed.text = PhotonNetwork.CurrentRoom.CustomProperties["seed"].ToString();
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
}
