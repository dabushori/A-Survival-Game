using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/*
 * A destructible object is an object that can be destroyed by the players
 * hit = deal damage to mobs, break = deal damage to world objects
 */
public class Destructible : MonoBehaviour
{
    // The health of the object
    [SerializeField] 
    int hp;

    public int HP
    {
        get
        {
            return hp;
        }
    }

    // The floating points prefab
    [SerializeField]
    GameObject floatingPointsPrefab;

    // The particles prefab
    [SerializeField]
    GameObject breakParticlesPrefab;

    // The sound played when dealing damage to the object
    [SerializeField]
    AudioClip takeDamageSound;

    // A photon view to sync the object on all the players' machines
    PhotonView photonView;

    // The items that can be received after destroying the object
    [SerializeField]
    Item[] items;
    // The minimum and maximum amounts of the items that can be received
    [SerializeField]
    int[] minItemsToGive, maxItemsToGive;
   
    // The tool level that is needed in order to break the current object
    [SerializeField]
    BreakLevel levelNeededToBreak;

    // The damage that will be dealt if a user hits the object with an item that can't be used for breaking (and the object allows it)
    [SerializeField]
    int DEFAULT_BREAKING_DAMAGE = 20, DEFAULT_HITTING_DAMAGE = 20;

    // The animator (if exists) of the object
    Animator animator;
    // The length of the death animation clip
    float deathLength;

    // A scale for the mobs particles
    [SerializeField]
    float mobScaleParticle = 1f;

    private void Awake()
    {
        // Get the photon view and the animator components of the object (if exists) 
        TryGetComponent<Animator>(out animator);
        photonView = GetComponent<PhotonView>();

        // Set the animator as null if it doesn't have any death animation
        if (animator != null && !animator.HasState(0, Animator.StringToHash("death"))) animator = null;
        
        // Find the death animation length
        if (animator != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "anim_death")
                {
                    deathLength = clip.length * 2;
                    break;
                }
            }
        }
    }

    /*
    * A function to break the current object. This function:
    * 1. Checkes that the object can be broken by the tool that the player holds.
    * 2. Play breaking sound on all the players' machines
    * 3. Create the break particles and floating point message
    * 4. Deal the damage to the object on all the players' machines
    */
    public void Break(Inventory inventory)
    {
        // If the object is already destroyed - don't do anything (In case that a player hits it before the updated HP arrives)
        if (hp <= 0) return;

        Item chosenItem = inventory.ChosenItem;
        int damage;
        BreakLevel toolBreakLevel;
        
        // If the levelNeededToBreak is WOOD, it means that the object can be broken by any item. 
        // In that case, if the player doesn't hold an item that can break, assign the default damage to the current damage
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.MINING) || (levelNeededToBreak == BreakLevel.WOOD && chosenItem.breakLevel != BreakLevel.WOOD))
        {
            damage = DEFAULT_BREAKING_DAMAGE;
            toolBreakLevel = BreakLevel.WOOD;
        } 
        // Otherwise, assign the damage that the current item of the player deals
        else
        {
            damage = chosenItem.breakDamage;
            toolBreakLevel = chosenItem.breakLevel;
        }

        // Check if the current item can break the current object
        if (Item.CanBreak(toolBreakLevel, levelNeededToBreak))
        {
            // Play the damage sound effect on all the players' machines
            photonView.RPC(nameof(RPCDamageSound), RpcTarget.All);
        
            // Create the particles and floating points
            if (breakParticlesPrefab != null) BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y / 6, transform);
            PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
            
            // Deal damage to the current object
            if (hp > 0)
            {
                DealDamage(damage, PhotonNetwork.LocalPlayer);
            }
        }
    }

    /*
    * A function to break the current object. This function:
    * 1. Checkes that the object can be broken by the tool that the player holds.
    * 2. Play breaking sound on all the players' machines
    * 3. Create the break particles and floating point message
    * 4. Deal the damage to the object on all the players' machines
    */
    public void Hit(Inventory inventory)
    {
        // If the object is already destroyed - don't do anything (In case that a player hits it before the updated HP arrives)
        if (hp <= 0) return;
        Item chosenItem = inventory.ChosenItem;
        int damage;
        
        // If the player doesn't hold an item that can hit, assign the default damage to the current damage
        if (chosenItem == null || !chosenItem.IsSuitableForJob(Jobs.FIGHTING))
        {
            damage = DEFAULT_HITTING_DAMAGE;
        }
        else
        {
            damage = chosenItem.hitDamage;
        }
        
        // Play the damage sound effect on all the players' machines
        photonView.RPC(nameof(RPCDamageSound), RpcTarget.All);

        // Create the particles and floating points
        if (breakParticlesPrefab != null) BreakParticles.CreateBreakParticles(breakParticlesPrefab, transform.position + Vector3.up * transform.lossyScale.y * mobScaleParticle, transform);
        PointsHandler.CreateFloatingPoints(floatingPointsPrefab, transform.position + Vector3.up * transform.lossyScale.y / 2, "-" + damage.ToString());
        
        // Deal damage to the current object
        if (hp > 0)
        {
            DealDamage(damage, PhotonNetwork.LocalPlayer);
        }
    }

    /*
    * An RPC to give the items to the local user (it exists because all the damage logic is done on the master client's machine)
    */
    [PunRPC]
    void GiveItemsToUser()
    {
        // Give the items to the user (the amount is chosen randomally using the given range)
        for (int i = 0; i < items.Length; ++i)
        {
            Inventory.Instance.AddToInventory(items[i], Random.Range(minItemsToGive[i], maxItemsToGive[i] + 1));
        }
    }

    /*
    * An RPC to deal damage to the current object if you are the master client, or notify the master client by sending this RPC to him otherwise 
    */
    [PunRPC]
    void DealDamage(int damage, Player sender)
    {
        if (photonView.IsMine)
        {
            // Deal damage to the object
            hp -= damage;

            // Destroy the object if its hp is less than 0
            if (hp <= 0)
            {
                DestroyObject(sender);
            }
        }
        else
        {
            // Tell the master client that damage has been dealt to the object
            photonView.RPC(nameof(DealDamage), photonView.Owner, damage, sender);
        }
    }

    /*
    * An RPC to play the damage sound on all the players' machines
    */
    [PunRPC]
    void RPCDamageSound()
    {
        SFXManager.Instance.PlaySound(takeDamageSound, transform.position, 2f);
    }

    /*
    * Destroy the current object and syncronize it with the death animation (if exists). This function will be called on the master client's machine.
    */
    void DestroyObject(Player destroyer)
    {
        // If animator is not null, which means that the death animation exists, destory the object after the animation is played
        if (animator != null)
        {
            Debug.Log("Animation Length: " + deathLength);
            animator.SetBool("IsDead", true);
            Invoke(nameof(DestroyMe), deathLength);
        }
        // Otherwise, destroy it now
        else
        {
            Invoke(nameof(DestroyMe), 0);
        }

        // Give the items to the user who destroyed the object by sending him the GiveItemsToUser RPC
        photonView.RPC(nameof(GiveItemsToUser), destroyer);
    }

    /*
    * Destroy the current object all over the network
    */
    void DestroyMe()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}