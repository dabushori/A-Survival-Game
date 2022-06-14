using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSync : MonoBehaviour
{
    PhotonView photonView;

    [PunRPC]
    public void EnableArmor()
    {
        if (PhotonView.Get(gameObject).IsMine)
        {
            PhotonView.Get(gameObject).RPC(nameof(EnableArmor), RpcTarget.Others);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void DisableArmor()
    {
        if (PhotonView.Get(gameObject).IsMine)
        {
            PhotonView.Get(gameObject).RPC(nameof(DisableArmor), RpcTarget.Others);
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
        gameObject.SetActive(true);
        GetComponent<SkinnedMeshRenderer>().material = armorMats[matIndex];
    }


    public void SetMaterial(Material m) 
    {
        if (GetComponent<SkinnedMeshRenderer>().material != m) PhotonView.Get(gameObject).RPC(nameof(SetMaterialByIndex), RpcTarget.All, armorMats.IndexOf(m));
    }
}
