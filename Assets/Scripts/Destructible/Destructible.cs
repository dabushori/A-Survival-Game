using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private int hp;

    [SerializeField]
    public GameObject floatingPointsPrefab;

    [SerializeField]
    public GameObject breakParticlesPrefab;

    [SerializeField]
    public AudioClip takeDamageSound;

    PhotonView photonView;

    public int HP
    {
        get
        {
            return hp;
        }
    }

    [SerializeField]
    private Item[] items;
    [SerializeField]
    private int[] minItemsToGive, maxItemsToGive;
    [SerializeField]
    private BreakLevel levelNeededToBreak;

    public int DEFAULT_BREAKING_DAMAGE = 20, DEFAULT_HITTING_DAMAGE = 20;

    Animator animator;
    float deathLength;

    [SerializeField]
    public float mobScaleParticle = 1f;
    private void Awake()
    {
        TryGetComponent<Animator>(out animator);
        photonView = GetComponent<PhotonView>();
        if (animator != null && !animator.HasState(0, Animator.StringToHash("death"))) animator = null;
            
        if (animator != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "anim_death")
                {
                    deathLength = clip.length + 0.3f;
                    break;
                }
            }
        }
    }

    public void Break(Inventory inventory)
    {
        if (hp <= 0) return;
        Item chosenItem = inventory.ChosenItem;
        int damage;
        BreakLevel toolBreakLevel;
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.MINING) || (levelNeededToBreak == BreakLevel.WOOD && chosenItem.breakLevel != BreakLevel.WOOD))
        {
            damage = DEFAULT_BREAKING_DAMAGE;
            toolBreakLevel = BreakLevel.WOOD;
        } 
        else
        {
            damage = chosenItem.breakDamage;
            toolBreakLevel = chosenItem.breakLevel;
        }

        if (Item.CanBreak(toolBreakLevel, levelNeededToBreak))
        {
            photonView.RPC(nameof(RPCDamageSound), RpcTarget.All);
            BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y / 6, transform);
            PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
            if (hp > 0)
            {
                DealDamage(damage, PhotonNetwork.LocalPlayer);
            }
        }
    }

    public void Hit(Inventory inventory)
    {
        if (hp <= 0) return;
        Item chosenItem = inventory.ChosenItem;
        int damage;
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.FIGHTING))
        {
            damage = DEFAULT_HITTING_DAMAGE;
        }
        else
        {
            damage = chosenItem.hitDamage;
        }
        photonView.RPC(nameof(RPCDamageSound), RpcTarget.All);

        BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y * mobScaleParticle, transform);
        PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
        if (hp > 0)
        {
            DealDamage(damage, PhotonNetwork.LocalPlayer);
        }
    }

    [PunRPC]
    void GiveItemsToUser()
    {
        // give items to the user
        for (int i = 0; i < items.Length; ++i)
        {
            Inventory.Instance.AddToInventory(items[i], Random.Range(minItemsToGive[i], maxItemsToGive[i] + 1));
        }
    }

    [PunRPC]
    void DealDamage(int damage, Player sender)
    {
        if (photonView.IsMine)
        {
            hp -= damage;
            if (hp <= 0)
            {
                DestroyObject(sender);
            }
        }
        else
        {
            photonView.RPC(nameof(DealDamage), photonView.Owner, damage, sender);
        }
    }

    [PunRPC]
    void RPCDamageSound()
    {
        SFXManager.Instance.PlaySound(takeDamageSound, transform.position, 2f);
    }

    void DestroyObject(Player destroyer)
    {
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            Invoke(nameof(DestroyMe), deathLength);
        }
        else
        {
            Invoke(nameof(DestroyMe), 0);
        }
        photonView.RPC(nameof(GiveItemsToUser), destroyer);
    }

    void DestroyMe()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void Update()
    {
        if (photonView.IsMine && HP <= 0)
        {
            DestroyMe();
        }
    }
}