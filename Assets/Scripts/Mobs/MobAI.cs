using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MobAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject player;

    public LayerMask WhatIsGround, WhatIsPlayer;

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

    LayerMask worldObjects = 1 << 7;

    void AssureValidPosition()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        var direction = new Vector3 { [col.direction] = 1 };
        var offset = col.height / 2 - col.radius;
        var localPoint0 = col.center - direction * offset;
        var localPoint1 = col.center + direction * offset;
        var point0 = transform.TransformPoint(localPoint0);
        var point1 = transform.TransformPoint(localPoint1);
        var r = transform.TransformVector(col.radius, col.radius, col.radius);
        var radius = Enumerable.Range(0, 3).Select(xyz => xyz == col.direction ? 0 : r[xyz])
            .Select(Mathf.Abs).Max();
        if (Physics.OverlapCapsule(point0, point1, radius, worldObjects).Length > 0)
        {
            Destroy(this);
        }
    }

    public void Awake()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
        {
            transform.position = hit.point;
        }
        else if (Physics.Raycast(transform.position, Vector3.up, out hit, float.MaxValue, (1 << 6)))
        {
            transform.position = hit.point;
        }

        AssureValidPosition();
        
        // player = GameObject.Find("FirstPersonPlayer");
        agent = GetComponent<NavMeshAgent>();
    }
    public void Update()
    {
        Collider[] sightPlayers = Physics.OverlapSphere(transform.position, sightRange, WhatIsPlayer);
        Collider[] attackPlayers = Physics.OverlapSphere(transform.position, attackRange, WhatIsPlayer);

        playerInSightRange = sightPlayers?.Length != 0;
        playerInAttackRange = attackPlayers?.Length != 0;

        player = playerInSightRange ? sightPlayers[0].gameObject : null;

        // playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        // playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange) Chasing();
        if (playerInAttackRange) Attacking();
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
        if (Physics.Raycast(walkPoint, -transform.up, 2f, WhatIsGround))
            walkPointSet = true;
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
            //attack
            PlayerHealth health = player.GetComponentInChildren<PlayerHealth>();
            health.DealDamage(damage);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}