using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DeathMenu : MonoBehaviourPunCallbacks
{
    PhotonView parentPV;

    private void Awake()
    {
        parentPV = GetComponentInParent<PhotonView>();
    }

    public void ReturnToMenu()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            parentPV.RPC(nameof(PlayerControls.ReturnAllToMenu), RpcTarget.Others);
            StartCoroutine(AssureAllPlayersExited());
        }
        else PhotonNetwork.LoadLevel("Menu");
    }

    private IEnumerator AssureAllPlayersExited()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount > 1) yield return null;
        PhotonNetwork.LoadLevel("Menu");
    }
}
