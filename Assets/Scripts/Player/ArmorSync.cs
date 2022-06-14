using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSync : MonoBehaviour
{
    PhotonView photonView;

    [PunRPC]
    private void OnEnable()
    {
        if (PhotonView.Get(gameObject).IsMine)
        {
            PhotonView.Get(gameObject).RPC(nameof(OnEnable), RpcTarget.Others);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    [PunRPC]
    private void OnDisable()
    {
        if (PhotonView.Get(gameObject).IsMine)
        {
            PhotonView.Get(gameObject).RPC(nameof(OnDisable), RpcTarget.Others);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    [SerializeField]
    List<Material> armorMats;

    [PunRPC]
    public void SetMaterialByIndex(int matIndex)
    {
        GetComponent<SkinnedMeshRenderer>().material = armorMats[matIndex];
    }


    public void SetMaterial(Material m) 
    {
        if (GetComponent<SkinnedMeshRenderer>().material != m) PhotonView.Get(gameObject).RPC(nameof(SetMaterialByIndex), RpcTarget.All, armorMats.IndexOf(m));
    }
}
