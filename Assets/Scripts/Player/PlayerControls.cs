using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [PunRPC]
    public void ReturnAllToMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    [PunRPC]
    public void DealDamage(int damage)
    {
        GetComponentInChildren<PlayerHealth>().DealDamage(damage);
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView.Get(this).RPC(nameof(PlayerControls.ReturnAllToMenu), RpcTarget.Others);
            StartCoroutine(AssureAllPlayersExited());
        }
    }

    private IEnumerator AssureAllPlayersExited()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount > 1) yield return null;
        PhotonNetwork.LoadLevel("Menu");
    }
}
