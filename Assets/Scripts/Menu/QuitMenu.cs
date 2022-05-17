using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using Photon.Realtime;

public class QuitMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Button leaveButton, stayButton;
    
    PhotonView parentPV;
    private void Start()
    {
        parentPV = GetComponentInParent<PhotonView>();
    }

    public void Quit()
    {
        stayButton.interactable = false;
        leaveButton.interactable = false;
        if (PhotonNetwork.IsMasterClient)
        {
            parentPV.RPC(nameof(PlayerLeaveRoom.ReturnAllToMenu), RpcTarget.Others);
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
