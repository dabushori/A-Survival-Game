using Photon.Pun;
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
            if (hp > 0)
            {
                DealDamage(damage);
                BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y / 6);
                PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
            }
            if (hp <= 0)
            {
                if (animator == null)
                {
                    DestroyObject();
                    BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y / 6);
                    PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
                } else
                {
                    animator.SetBool("IsDead", true);
                    DestroyObject();
                }
                for (int i = 0; i < items.Length; ++i)
                {
                    inventory.AddToInventory(items[i], Random.Range(minItemsToGive[i], maxItemsToGive[i] + 1));
                }
            }
        }
    }

    public void Hit(Inventory inventory)
    {
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

        if (hp > 0)
        {
            DealDamage(damage);
            BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2);
            PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
        }
        if (hp <= 0)
        {
            DestroyObject();
            BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2);
            PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
            for (int i = 0; i < items.Length; ++i)
            {
                inventory.AddToInventory(items[i], Random.Range(minItemsToGive[i], maxItemsToGive[i] + 1));
            }
        }
    }

    [PunRPC]
    void DealDamage(int damage)
    {
        if (photonView.IsMine)
        {
            hp -= damage;
        }
        else
        {
            photonView.RPC(nameof(DealDamage), photonView.Owner, damage);
        }
    }

    [PunRPC]
    void DestroyObject()
    {
        if (photonView.IsMine)
        {
            if (animator != null)
            {
                animator.SetBool("IsDead", true);
                Invoke(nameof(DestroyMe), deathLength);
            }
            else
            {
                DestroyMe();
            }
        } 
        else
        {
            photonView.RPC(nameof(DestroyObject), photonView.Owner);
        }
    }

    void DestroyMe()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}