using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MobsGeneration : MonoBehaviour
{
    public int MAX_HOSTILE_MOBS_CAPACITY, MAX_FRIENDLY_MOBS_CAPACITY;
    public List<MobData> hostileMobsList;
    public List<MobData> friendlyMobsList;

    int sumOfHostileWeights, sumOfFriendlyWeights;

    public int spawnRadiusMax, spawnRadiusMin;

    public LayerMask hostileMobsLayer, friendlyMobsLayer;

    public void SpawnMob(bool isHotile)
    {
        // select random mob
        MobData mob = SelectRandomMob(isHotile);

        int numOfMobs = Random.Range(mob.groupSizeMin, mob.groupSizeMax + 1);

        for (int i = 0; i < numOfMobs; ++i)
        {
            // choose a random spawn location
            Vector2 location = Random.insideUnitCircle * spawnRadiusMax;
            if (Vector2.Distance(location, new Vector2(transform.position.x, transform.position.z)) < spawnRadiusMin)
            {
                continue;
            }
            Vector3 mobPosition = new Vector3(location.x, 0, location.y) + transform.position;
        
            // check spawn rules
            if (CanSpawnMob(mob, isHotile, mobPosition))
            {
                Instantiate(mob.mobObject, mobPosition, Quaternion.identity);
            }
        }
    }

    MobData SelectRandomMob(bool isHotile)
    {
        int sumOfWeights = isHotile ? sumOfHostileWeights : sumOfFriendlyWeights;
        List<MobData> mobsList = isHotile ? hostileMobsList : friendlyMobsList;
        int rnd = Random.Range(0, sumOfWeights + 1);
        foreach (MobData md in mobsList)
        {
            if (rnd <= md.weight)
            {
                return md;
            }
            rnd -= md.weight;
        }
        return mobsList[mobsList.Count - 1];
    }

    public bool CanSpawnMob(MobData mobData, bool isHostile, Vector3 position)
    {
        if (mobData.onlyAtNight)
        {
            return position.x > 0 && position.z > 0 && position.x < GameStateController.worldWidth && position.z < GameStateController.worldDepth &&
                GameStateController.timeController.IsNight() && (isHostile ?
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, hostileMobsLayer).Length < MAX_HOSTILE_MOBS_CAPACITY) :
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, friendlyMobsLayer).Length < MAX_FRIENDLY_MOBS_CAPACITY));
        }
        return position.x > 0 && position.z > 0 && position.x < GameStateController.worldWidth && position.z < GameStateController.worldDepth &&
            (isHostile ?
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, hostileMobsLayer).Length < MAX_HOSTILE_MOBS_CAPACITY) : 
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, friendlyMobsLayer).Length < MAX_FRIENDLY_MOBS_CAPACITY));
    }


    float previousHostileSpawnTime = -Mathf.Infinity, previousFriendlySpawnTime = -Mathf.Infinity;
    public float HOSTILE_MOB_SPAWN_CYCLE_TIME, FRIENDLY_MOB_SPAWN_CYCLE_TIME;
    public void Update()
    {
        if (Time.time - previousHostileSpawnTime > HOSTILE_MOB_SPAWN_CYCLE_TIME)
        {
            previousHostileSpawnTime = Time.time;
            SpawnMob(true);
        }
        if (Time.time - previousFriendlySpawnTime > HOSTILE_MOB_SPAWN_CYCLE_TIME)
        {
            previousFriendlySpawnTime = Time.time;
            SpawnMob(false);
        }
    }

    private void Awake()
    {
        hostileMobsList = new List<MobData>(Resources.LoadAll("Mobs/Hostile Mobs", typeof(MobData)).Cast<MobData>());
        friendlyMobsList = new List<MobData>(Resources.LoadAll("Mobs/Friendly Mobs", typeof(MobData)).Cast<MobData>());

        sumOfHostileWeights = 0;
        foreach(MobData md in hostileMobsList)
        {
            sumOfHostileWeights += md.weight;
        }
        sumOfFriendlyWeights = 0;
        foreach (MobData md in friendlyMobsList)
        {
            sumOfFriendlyWeights += md.weight;
        }
    }
}
