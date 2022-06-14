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
        if (gameObject.activeSelf) return;
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
        if (!gameObject.activeSelf) return;
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
    int currentIndex = -1;

    [PunRPC]
    public void SetMaterialByIndex(int matIndex)
    {
        gameObject.SetActive(true);
        currentIndex = matIndex;
        GetComponent<SkinnedMeshRenderer>().material = armorMats[matIndex];
    }


    public void SetMaterial(Material m) 
    {
        int matIndex = armorMats.IndexOf(m);
        if (currentIndex != matIndex)
        {
            SetMaterialByIndex(matIndex);
            PhotonView.Get(gameObject).RPC(nameof(SetMaterialByIndex), RpcTarget.Others, matIndex);
        }
    }
}
