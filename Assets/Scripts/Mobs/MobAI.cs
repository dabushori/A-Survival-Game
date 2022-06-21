using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
/*
 * A class to handle the entire mob AI
 */
public class MobAI : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent; // the nav mesh agent of the mob

    GameObject player; // the player the mob targets

    [SerializeField]
    bool onlyAtNight; // can the mob be alive only at night

    // stats
    [SerializeField]
    int damage; // projectile damage


    //Walking
    Vector3 walkPoint; // where the mob goes to
    bool walkPointSet; // does the mob have a walkpoint
    [SerializeField]
    float walkPointRange; // how far can the mob walk (for one point)

    //Attacking
    [SerializeField]
    float timeBetweenAttacks;  // the time between each attack
    bool alreadyAttacked; // bool used to have attack per time

    //Ranges
    [SerializeField]
    float sightRange; // ranges of sight by the mob
    [SerializeField]
    float attackRange; // ranges of attack by the mob
    bool playerInSightRange, playerInAttackRange; // bools that hold if there are any players in the sights

    // sound
    bool alreadyPlayedSound; // bool used to play sound per time
    [SerializeField]
    AudioClip sound; // clip of mob sound
    [SerializeField]
    float timeBetweenSounds; // the time between each sound

    Animator animator; // animator of the mob
    bool hasMultipleAnimations; // bool that know if the mob has multiple animations

    PhotonView mobPV; // photon view of the mob (for rpcs)

    /*
     * Assure that the mob is in a valid position, meaning he is not hiting any objects
     */
    void AssureValidPosition()
    {
        if (Physics.OverlapSphere(transform.position, GetComponent<CapsuleCollider>().radius, GameStateController.worldObjectsLayer).Length > 0)
        {
            // if not in valid position: destroy the mob
            DestroyMob();
        }
    }

    /*
     * Destory the mob
     */
    void DestroyMob()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public void Awake()
    {
        mobPV = GetComponent<PhotonView>();
        // here we will only find hits with the surface layer
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, GameStateController.surfaceLayer))
        {
            transform.position = hit.point;
        }
        else if (Physics.Raycast(transform.position, Vector3.up, out hit, float.MaxValue, GameStateController.surfaceLayer))
        {
            transform.position = hit.point;
        }
        // checks if the position is valid
        AssureValidPosition();

        // set variables
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        // check if the mob has multiple animations
        hasMultipleAnimations = animator.HasState(0, Animator.StringToHash("attack"));
    }

    public void Update()
    {
        // if the mob lives only at night and it is day kill the mob
        if (onlyAtNight && GameStateController.timeController != null && !GameStateController.timeController.IsNight())
        {
            DestroyMob();
            return;
        }
        // the mob will move only in one player game (his owner) and then sync through the server
        if (!mobPV.IsMine || !agent.isOnNavMesh) return;
        // if the mob is dead
        if (GetComponent<Destructible>().HP <= 0) return;

        // checking for any players in the ranges
        Collider[] sightPlayers = Physics.OverlapSphere(transform.position, sightRange, GameStateController.playersLayer).OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
        Collider[] attackPlayers = Physics.OverlapSphere(transform.position, attackRange, GameStateController.playersLayer);

        playerInSightRange = sightPlayers?.Length != 0;
        playerInAttackRange = attackPlayers?.Length != 0;
        // save the closest player as the taget
        player = playerInSightRange ? sightPlayers[0].transform.gameObject : null;
        // if there is no player in any range do patroling
        if (!playerInSightRange && !playerInAttackRange)
        {
            if (hasMultipleAnimations) animator.SetBool("IsAttacking", false);
            Patroling();
        }
        // if there is a player in sight range do chasing
        if (playerInSightRange)
        {
            if (!playerInAttackRange && hasMultipleAnimations)
            {
                animator.SetBool("IsAttacking", false);
            }
            Chasing();
        }
        // if there is a player in attack range do attacking
        if (playerInAttackRange)
        {
            Attacking();
        }

        // play sound if there is a player in sight range
        Collider[] soundPlayers = Physics.OverlapSphere(transform.position, sightRange, GameStateController.playersLayer).OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
        bool playerInSoundRange = soundPlayers?.Length != 0;
        if (playerInSoundRange)
        {
            Talking();
        }
    }

    /*
     * The mob play his sound
     */
    public void Talking()
    {
        // calls the PlaySound once and it will be able to call it again only after timeBetweenSounds
        if (!alreadyPlayedSound && sound != null)
        {
            // play sound
            Invoke(nameof(PlaySound), timeBetweenSounds / 2);

            alreadyPlayedSound = true;
            // reset sound
            Invoke(nameof(ResetSound), timeBetweenSounds);
        }
    }

    /*
     * The function calls a rpc function play sound if there is a sound
     */
    private void PlaySound()
    {
        if (sound != null) PhotonView.Get(this).RPC(nameof(RPCPlaySound), RpcTarget.All);
    }

    /*
     * The function play the sound
     */
    [PunRPC]
    void RPCPlaySound()
    {
        SFXManager.Instance.PlaySound(sound, transform.position, 0.5f);
    }

    /*
     * The function reset the already played sound so we will be able to play the sound again
     */
    private void ResetSound()
    {
        alreadyPlayedSound = false;
    }

    /*
     * The function finds a place to walk to and sends the mob there.
     */
    public void Patroling()
    {
        // find walk point
        if (!walkPointSet) FindWalkPoint();

        // the mob walk to the walk point
        if (walkPointSet) agent.SetDestination(walkPoint);

        // check if walkpoint reached
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;

    }

    /*
     * The function finds a walk point in the range of the walk point
     */
    private void FindWalkPoint()
    {
        // random walk point in range
        float newX = Random.Range(-walkPointRange, walkPointRange);
        float newZ = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + newX, transform.position.y, transform.position.z + newZ);

        // find the height of the target point by its x and y
        walkPointSet = true;
        if (Physics.Raycast(walkPoint, Vector3.down, out RaycastHit hit, float.MaxValue, GameStateController.surfaceLayer))
        {
            walkPoint.y = hit.point.y;
        }
        else if (Physics.Raycast(walkPoint, Vector3.up, out hit, float.MaxValue, GameStateController.surfaceLayer))
        {
            walkPoint.y = hit.point.y;
        }
        else
        {
            walkPointSet = false;
            return;
        }
        // if the mob will hit an object we cancel the walk point
        if (Physics.OverlapSphere(walkPoint, 1f, GameStateController.worldObjectsLayer).Length != 0) walkPointSet = false;
    }

    /*
     * The function makes the mob chase the chosen player
     */
    public void Chasing()
    {
        agent.SetDestination(player.transform.position);
    }

    /*
     * The function makes the mob attack the chosen player
     */
    public void Attacking()
    {
        // mob wont move while attacking
        agent.SetDestination(transform.position);
        // look at the player
        transform.LookAt(player.transform);
        // attack once until reset after timeBetweenAttacks
        if (!alreadyAttacked)
        {
            if (hasMultipleAnimations) animator.SetBool("IsAttacking", true);
            // attack
            Invoke(nameof(DealDamage), 0.2f);
            alreadyAttacked = true;
            // reset attack
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    /*
     * The function is being called 0.2 seconds after the mob attack and checks if the player is still in range to take damage
     */
    void DealDamage()
    {
        if (player != null)
        {
            // play sound when attacking
            PlaySound();
            // check if the player is still in damage range
            Collider[] inDamageRangePlayer = Physics.OverlapSphere(transform.position, attackRange, GameStateController.playersLayer);
            // if in damage range send rpc to the player owner to deal him damage
            if (inDamageRangePlayer?.Length != 0)
            {
                PhotonView.Get(player).RPC(nameof(PlayerControls.DealDamage), PhotonView.Get(player).Owner, damage);
            }
        }
        if (hasMultipleAnimations) animator.SetBool("IsAttacking", false);
    }

    /*
     * The function reset the alreadyAttacked and the mob will be able to attack again
     */
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

}