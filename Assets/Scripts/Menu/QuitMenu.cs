using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using Photon.Realtime;

/*
 * The class is in charge of quiting menu options
 */
public class QuitMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Button leaveButton, stayButton; // leave and stay button
    
    PhotonView parentPV; // player photon view
    private void Start()
    {
        parentPV = GetComponentInParent<PhotonView>(); // save the photon view
    }

    public void Quit()
    {
        // make the buttons uninteractable
        stayButton.interactable = false;
        leaveButton.interactable = false;
        // if it is the master: kick everyone to the main menu
        if (PhotonNetwork.IsMasterClient)
        {
            parentPV.RPC(nameof(PlayerControls.ReturnAllToMenu), RpcTarget.Others); // send all the players to the menu (with rpc)
            StartCoroutine(AssureAllPlayersExited()); // wait until everyone left and then send the master too
        }
        // if it is not the master just return to the main menu
        else PhotonNetwork.LoadLevel("Menu");
    }

    /*
     * leave to main menu after all the other players left
     */
    private IEnumerator AssureAllPlayersExited()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount > 1) yield return null; // wait until only the master is in the game
        PhotonNetwork.LoadLevel("Menu");
    }

    /*
     * if left the application call quit
     */
    private void OnApplicationQuit()
    {
        Quit();
    }
}
