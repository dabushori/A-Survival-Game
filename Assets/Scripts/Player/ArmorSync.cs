using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

/*
 * Sync the worn armor between all players. This script will be added to every armor part of each player.
 */
public class ArmorSync : MonoBehaviour
{
    // Current photonView
    PhotonView photonView;

    // List of the possible materials for armor
    [SerializeField]
    List<Material> armorMats;
    
    // The index of the current material for the current armor icon
    int currentIndex = -1;

    // Show the current armor
    [PunRPC]
    public void EnableArmor()
    {
        if (gameObject.activeSelf) return;
        if (PhotonView.Get(gameObject).IsMine)
        {
            // Send an RPC to others saying to enable the armor on this player
            PhotonView.Get(gameObject).RPC(nameof(EnableArmor), RpcTarget.Others);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    // Hide the current armor
    [PunRPC]
    public void DisableArmor()
    {
        if (!gameObject.activeSelf) return;
        if (PhotonView.Get(gameObject).IsMine)
        {
            // Send an RPC to others saying to disable the armor on this player
            PhotonView.Get(gameObject).RPC(nameof(DisableArmor), RpcTarget.Others);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /*
     * Set the material of the current armor part to the material with the given index
     */
    [PunRPC]
    public void SetMaterialByIndex(int matIndex)
    {
        gameObject.SetActive(true);
        currentIndex = matIndex;
        GetComponent<SkinnedMeshRenderer>().material = armorMats[matIndex];
    }

    /*
     * Set the material of the current armor part to the given material
     */
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
