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

    [SerializeField]
    Transform itemPlaceHolder;

    [PunRPC]
    public void HoldItem(string name)
    {
        if (PhotonView.Get(this).Owner == PhotonNetwork.LocalPlayer)
        {
            PhotonView.Get(this).RPC(nameof(HoldItem), RpcTarget.Others, name);
        }
        foreach (Transform childTransform in itemPlaceHolder.GetComponentInChildren<Transform>())
        {
            Destroy(childTransform.gameObject);
        }
        Instantiate(Resources.Load(GameStateController.itemsToHoldPath + name, typeof(GameObject)), itemPlaceHolder);
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

    [SerializeField]
    AudioClip eatingSound;

    [PunRPC]
    public void PlayEatingSound()
    {
        if (PhotonView.Get(this).Owner == PhotonNetwork.LocalPlayer)
        {
            PhotonView.Get(this).RPC(nameof(PlayEatingSound), RpcTarget.Others);
        }
        SFXManager.Instance.PlaySound(eatingSound, transform.position, 0.8f);
    }
}
