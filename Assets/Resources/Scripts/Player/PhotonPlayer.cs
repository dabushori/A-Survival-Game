using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    GameObject myFirstPersonPlayer;
    PhotonView myPhotonView;

    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        if (myPhotonView.IsMine)
        {
            var position = Random.insideUnitCircle * 20;
            myFirstPersonPlayer = PhotonNetwork.Instantiate("Prefabs/Player/FirstPersonPlayer", new Vector3(position.x + GameStateController.worldDepth / 2, 5, position.y + GameStateController.worldWidth / 2), Quaternion.identity);
        }
    }
}
