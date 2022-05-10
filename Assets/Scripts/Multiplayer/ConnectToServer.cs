using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    public TMPro.TMP_InputField nameInput;
    public TMPro.TMP_Text connectText;
    public Button connectButton;

    public GameObject mainMenu;

    public void OnConnect()
    {
        if (nameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = nameInput.text;
            connectText.text = "Connecting...";
            connectButton.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
