using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask WhatIsGround, WhatIsPlayer;

    // stats
    public int health;
    // public Item dropItem;
    // public int damage; // projectile damage


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

    public void Awake()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, (1 << 6))) // 6 is the surface layer, so here we will only find hits with the surface layer
        {
            transform.position = hit.point;
        }
        player = GameObject.Find("FirstPersonPlayer").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    public void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

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
        agent.SetDestination(player.position);
    }
    public void Attacking()
    {
        // mob wont move while attacking
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //attack


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyMob), 0.5f);
    }
    private void DestroyMob()
    {
        // drop item
        Destroy(gameObject);
    }
}