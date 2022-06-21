using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    PhotonView pv; // player photon view

    public void Awake()
    {
        pv = PhotonView.Get(this);
    }

    /*
     * Return everyone to the menu
     */
    [PunRPC]
    public void ReturnAllToMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    /*
     * Deal damage to the player and send sound of being hit
     */
    [PunRPC]
    public void DealDamage(int damage)
    {
        PlayTakingDamageSound();
        GetComponentInChildren<PlayerHealth>().DealDamage(damage);
    }

    [SerializeField]
    AudioClip takingDamageSound; // taking damage sound

    /*
     * play the damage sound to everyone
     */
    [PunRPC]
    public void PlayTakingDamageSound()
    {
        if (pv.IsMine)
        {
            // send the same rpc to everyone else
            pv.RPC(nameof(PlayTakingDamageSound), RpcTarget.Others);
        }
        // play the sound
        SFXManager.Instance.PlaySound(takingDamageSound, transform.position, 1.5f);
    }

    [SerializeField]
    Transform itemPlaceHolder; // the place where the item will be shown

    /*
     * shows the item being held to everyone
     */
    [PunRPC]
    public void HoldItem(string name)
    {
        if (pv.IsMine)
        {
            // send the rpc to everyone else
            pv.RPC(nameof(HoldItem), RpcTarget.Others, name);
        }
        //destroy last item that was there
        foreach (Transform childTransform in itemPlaceHolder.GetComponentInChildren<Transform>())
        {
            Destroy(childTransform.gameObject);
        }
        // put the new item in the placeHolder
        if (name != null) Instantiate(Resources.Load(GameStateController.itemsToHoldPath + name, typeof(GameObject)), itemPlaceHolder);
    }

    /*
     * When leaving the app
     */
    private void OnApplicationQuit()
    {
        // if you are the master
        if (PhotonNetwork.IsMasterClient)
        {
            // return everyone to the menu
            pv.RPC(nameof(ReturnAllToMenu), RpcTarget.Others);
            // send all commands in the server that did not arrive yet to not get errors
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [SerializeField]
    AudioClip eatingSound; // eating sound

    /*
     * play the eating sound
     */
    [PunRPC]
    public void PlayEatingSound()
    {
        if (pv.IsMine)
        {
            // send the same rpc to everyone
            pv.RPC(nameof(PlayEatingSound), RpcTarget.Others);
        }
        // play the sound
        SFXManager.Instance.PlaySound(eatingSound, transform.position, 0.8f);
    }

    /*
     * The function is called when player finished spawning
     * The function save how many players finished spawning and notify them for each new player.
     */
    [PunRPC]
    public void SpawnedPlayer()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // let the master handle it
            pv.RPC(nameof(SpawnedPlayer), RpcTarget.MasterClient);
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            // the master save in  properties["playersInGame"] the amount of players that finished loading the game
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            properties["playersInGame"] = (int)properties["playersInGame"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            // notify all that another player finished loading with another rpc
            pv.RPC(nameof(AddSpawnedPlayers), RpcTarget.All);
            return;
        }
    }

    /*
     * Call to notify everyone that another player finished spawning
     */
    [PunRPC]
    public void AddSpawnedPlayers()
    {
        WorldGeneration.Instance.SpawnedPlayer();
    }


    /*
     * being called when a player gets that everyone finished loading (that means they are ready)
     * The function count how many people are ready and then call to start the game
     */
    [PunRPC]
    public void ReadyPlayer()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // let the master handle it
            pv.RPC(nameof(ReadyPlayer), RpcTarget.MasterClient);
        } 
        else
        {
            // the master save in  properties["readyPlayers"] the amount of players that are ready to play
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            properties["readyPlayers"] = (int)properties["readyPlayers"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            // of the amount of players that are ready is the amount of the players in the room: start playing is being called to everyone
            if ((int)properties["readyPlayers"] == PhotonNetwork.CurrentRoom.PlayerCount) pv.RPC(nameof(StartGame), RpcTarget.All);
        }
    }

    /*
     * call a function in the world generation that start the game
     */
    [PunRPC]
    public void StartGame()
    {
        WorldGeneration.Instance.StartGame();
    }

    /**
     * revive the player
     */
    public void Revive()
    {
        HoldItem(null); // remove the item being held
        StartCoroutine("TeleportSpawn"); // revivng the player
    }

    private IEnumerator TeleportSpawn()
    {
        // teleport the player
        gameObject.transform.position = new Vector3(GameStateController.worldDepth / 2, 5, GameStateController.worldWidth / 2);
        yield return new WaitForSeconds(0.3f); // wait in order to not allow the player to move immediately and stuck the teleport
        // add the health back
        PlayerHealth p = GetComponentInChildren<PlayerHealth>();
        p.AddHealth(p.MaxHealth);
        // clear the inventory
        Inventory.Instance.ClearInventory();
        // lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
    }
}
