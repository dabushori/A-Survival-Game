using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeaveRoom : MonoBehaviour
{
    [PunRPC]
    public void ReturnAllToMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }
}
