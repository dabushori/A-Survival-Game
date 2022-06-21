using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    PhotonView pv;
    public void Awake()
    {
        pv = PhotonView.Get(this);
    }
    [PunRPC]
    public void ReturnAllToMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    [PunRPC]
    public void DealDamage(int damage)
    {
        PlayTakingDamageSound();
        GetComponentInChildren<PlayerHealth>().DealDamage(damage);
    }

    [SerializeField]
    AudioClip takingDamageSound;

    [PunRPC]
    public void PlayTakingDamageSound()
    {
        if (pv.IsMine)
        {
            pv.RPC(nameof(PlayTakingDamageSound), RpcTarget.Others);
        }
        SFXManager.Instance.PlaySound(takingDamageSound, transform.position, 1.5f);
    }

    [SerializeField]
    Transform itemPlaceHolder;

    [PunRPC]
    public void HoldItem(string name)
    {
        if (pv.IsMine)
        {
            pv.RPC(nameof(HoldItem), RpcTarget.Others, name);
        }
        foreach (Transform childTransform in itemPlaceHolder.GetComponentInChildren<Transform>())
        {
            Destroy(childTransform.gameObject);
        }
        if (name != null) Instantiate(Resources.Load(GameStateController.itemsToHoldPath + name, typeof(GameObject)), itemPlaceHolder);
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC(nameof(ReturnAllToMenu), RpcTarget.Others);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [SerializeField]
    AudioClip eatingSound;

    [PunRPC]
    public void PlayEatingSound()
    {
        if (pv.IsMine)
        {
            pv.RPC(nameof(PlayEatingSound), RpcTarget.Others);
        }
        SFXManager.Instance.PlaySound(eatingSound, transform.position, 0.8f);
    }


    [PunRPC]
    public void SpawnedPlayer()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            pv.RPC(nameof(SpawnedPlayer), RpcTarget.MasterClient);
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            properties["playersInGame"] = (int)properties["playersInGame"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            pv.RPC(nameof(AddSpawnedPlayers), RpcTarget.All);
            return;
        }
    }

    [PunRPC]
    public void AddSpawnedPlayers()
    {
        WorldGeneration.Instance.SpawnedPlayer();
    }



    [PunRPC]
    public void ReadyPlayer()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            pv.RPC(nameof(ReadyPlayer), RpcTarget.MasterClient);
        } 
        else
        {
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            properties["readyPlayers"] = (int)properties["readyPlayers"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            if ((int)properties["readyPlayers"] == PhotonNetwork.CurrentRoom.PlayerCount) pv.RPC(nameof(FadeScreen), RpcTarget.All);
        }
    }

    [PunRPC]
    public void FadeScreen()
    {
        WorldGeneration.Instance.FadeScreen();
    }

    public void Revive()
    {
        HoldItem(null);
        StartCoroutine("TeleportSpawn");
    }

    private IEnumerator TeleportSpawn()
    {
        gameObject.transform.position = new Vector3(GameStateController.worldDepth / 2, 5, GameStateController.worldWidth / 2);
        yield return new WaitForSeconds(0.3f);
        PlayerHealth p = GetComponentInChildren<PlayerHealth>();
        p.AddHealth(p.MaxHealth);
        Inventory.Instance.ClearInventory();
        Cursor.lockState = CursorLockMode.Locked;
    }
}
