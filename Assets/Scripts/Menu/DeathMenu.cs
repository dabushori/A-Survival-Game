using Photon.Pun;
using System.Collections;

/*
 * The class in charge of the death menu
 */
public class DeathMenu : MonoBehaviourPunCallbacks
{
    PhotonView parentPV; // photon id of the player

    private void Awake()
    {
        parentPV = GetComponentInParent<PhotonView>();
    }

    public void ReturnToMenu()
    {
        GetComponentInParent<PlayerControls>().HoldItem(null); // remove item that is shown
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
     * revive the player
     */
    public void Revive()
    {
        GetComponentInParent<PlayerControls>().Revive();
    }
}
