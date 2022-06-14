using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class MobAI : MonoBehaviour
{
    public NavMeshAgent agent;

    GameObject player;

    public LayerMask WhatIsGround, WhatIsPlayer, WhatIsEnvironment;

    // stats
    public int damage; // projectile damage


    //Walking
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    //Ranges
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    // sound
    private bool alreadyPlayedSound;
    public AudioClip sound;
    public float timeBetweenSounds;


    LayerMask worldObjects = 1 << 7;

    Animator animator;
    bool hasMultipleAnimations;

    PhotonView mobPV;

    void AssureValidPosition()
    {
        if (Physics.OverlapSphere(transform.position, GetComponent<CapsuleCollider>().radius, worldObjects).Length > 0)
        {
            DestroyMob();
        }
    }

    [PunRPC]
    void DestroyMob()
    {
        if (mobPV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            mobPV.RPC(nameof(DestroyMob), mobPV.Owner);
        }
    }

    public void Awake()
    {
        mobPV = GetComponent<PhotonView>();
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
        {
            transform.position = hit.point;
        }
        else if (Physics.Raycast(transform.position, Vector3.up, out hit, float.MaxValue, (1 << 6)))
        {
            transform.position = hit.point;
        }

        AssureValidPosition();
        
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hasMultipleAnimations = animator.HasState(0, Animator.StringToHash("attack"));
    }
    public void Update()
    {
        if (!mobPV.IsMine || !agent.isOnNavMesh) return;
        if (GetComponent<Destructible>().HP <= 0) return;
        Collider[] sightPlayers = Physics.OverlapSphere(transform.position, sightRange, WhatIsPlayer).OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
        Collider[] attackPlayers = Physics.OverlapSphere(transform.position, attackRange, WhatIsPlayer);

        playerInSightRange = sightPlayers?.Length != 0;
        playerInAttackRange = attackPlayers?.Length != 0;
        player = playerInSightRange ? sightPlayers[0].transform.gameObject : null;
        if (!playerInSightRange && !playerInAttackRange)
        {
            if (hasMultipleAnimations) animator.SetBool("IsAttacking", false);
            Patroling();
        }
        // if (player == null || player.GetComponent<PhotonView>().ViewID != (int)PhotonNetwork.LocalPlayer.CustomProperties["local_player"]) return;
        if (playerInSightRange)
        {
            if (!playerInAttackRange && hasMultipleAnimations)
            {
                animator.SetBool("IsAttacking", false);
            }
            Chasing();
        }
        if (playerInAttackRange)
        {
            Attacking();
        }

        // sound
        Collider[] soundPlayers = Physics.OverlapSphere(transform.position, 30, WhatIsPlayer).OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
        bool playerInSoundRange = soundPlayers?.Length != 0;
        if (playerInSoundRange)
        {
            Talking();
        }
    }

    public void Talking()
    {
        if (!alreadyPlayedSound && sound != null)
        {
            Invoke(nameof(PlaySound), timeBetweenSounds / 2);

            alreadyPlayedSound = true;
            Invoke(nameof(ResetSound), timeBetweenSounds);
        }
    }

    public void Patroling()
    {
        if (!walkPointSet) FindWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;

    }

    private void FindWalkPoint()
    {
        float newX = Random.Range(-walkPointRange, walkPointRange);
        float newZ = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + newX, transform.position.y, transform.position.z + newZ);
        // checking if the poisnt is on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, WhatIsGround)) walkPointSet = true;
        if (Physics.OverlapSphere(walkPoint, 1f, WhatIsEnvironment).Length != 0) walkPointSet = false;
    }
    public void Chasing()
    {
        agent.SetDestination(player.transform.position);
    }
    public void Attacking()
    {
        // mob wont move while attacking
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform);

        if (!alreadyAttacked)
        {
            if (hasMultipleAnimations) animator.SetBool("IsAttacking", true);

            // attack
            Invoke(nameof(DealDamage), timeBetweenAttacks / 2);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void DealDamage()
    {
        if (player != null)
        {
            PhotonView.Get(player).RPC(nameof(PlayerControls.DealDamage), PhotonView.Get(player).Owner, damage);
            PlaySound();
        }
        if (hasMultipleAnimations) animator.SetBool("IsAttacking", false);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void PlaySound()
    {
        if (sound != null) PhotonView.Get(this).RPC(nameof(RPCPlaySound), RpcTarget.All);
    }

    [PunRPC]
    void RPCPlaySound()
    {
        SFXManager.Instance.PlaySound(sound, transform.position, 0.5f);
    }

    private void ResetSound()
    {
        alreadyPlayedSound = false;
    }
}