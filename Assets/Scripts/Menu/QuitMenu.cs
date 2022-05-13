using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class QuitMenu : MonoBehaviour
{
    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");
    }
}
