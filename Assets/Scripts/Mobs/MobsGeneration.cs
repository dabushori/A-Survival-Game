using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobsGeneration : MonoBehaviour
{
    public int MAX_HOSTILE_MOBS_CAPACITY, MAX_FRIENDLY_MOBS_CAPACITY;
    public List<MobData> hostileMobsList;
    public List<MobData> friendlyMobsList;

    int sumOfHostileWeights, sumOfFriendlyWeights;

    public int spawnRadiusMax, spawnRadiusMin;

    public LayerMask hostileMobsLayer, friendlyMobsLayer, whatIsEnvironment;

    public void SpawnMob(bool isHotile)
    {
        // select random mob
        MobData mob = SelectRandomMob(isHotile);

        int numOfMobs = Random.Range(mob.groupSizeMin, mob.groupSizeMax + 1);

        for (int i = 0; i < numOfMobs; ++i)
        {
            // choose a random spawn location
            Vector2 location = Random.insideUnitCircle * spawnRadiusMax;
            Vector3 mobPosition = new Vector3(location.x, 0, location.y) + transform.position;
            if (Vector3.Distance(mobPosition, new Vector3(transform.position.x, 0, transform.position.z)) < spawnRadiusMin)
            {
                continue;
            }
        
            // check spawn rules
            if (CanSpawnMob(mob, isHotile, mobPosition))
            {
                PhotonNetwork.InstantiateRoomObject(GameStateController.mobsPath + mob.mobName, mobPosition, Quaternion.identity);
                //Instantiate(mob.mobObject, mobPosition, Quaternion.identity);
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
        if (GameStateController.timeController == null) return false;
        if (Physics.OverlapSphere(position, 2f, whatIsEnvironment).Length != 0) return false;
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
            if (GameStateController.timeController != null)
            {
                if (GameStateController.timeController.IsNight())
                {
                    previousHostileSpawnTime = Time.time;
                    SpawnMob(true);
                } 
                else
                {
                    // spawn hostile one in five times when at night it is one to one
                    int rand = Random.Range(0, 5);
                    if (rand == 0)
                    {
                        previousHostileSpawnTime = Time.time;
                        SpawnMob(true);
                    }
                }

            } 
        }
        if (Time.time - previousFriendlySpawnTime > FRIENDLY_MOB_SPAWN_CYCLE_TIME)
        {
            previousFriendlySpawnTime = Time.time;
            SpawnMob(false);
        }
        UpdateDifficulty();
    }

    private int lastDay;
    private void UpdateDifficulty()
    {
        if (GameStateController.timeController == null) return;
        int day = GameStateController.timeController.GetDay();
        if (day == lastDay) return;
        switch(day)
        {
            case 3:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 12;
                break;
            case 5:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 10;
                break;
            case 7:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 9;
                break;
            case 9:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 8;
                break;
            case 11:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 7;
                break;
            case 13:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 5;
                break;
            case 15:
                MAX_HOSTILE_MOBS_CAPACITY += 5;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 3;
                break;
            case 20:
                MAX_HOSTILE_MOBS_CAPACITY += 20;
                HOSTILE_MOB_SPAWN_CYCLE_TIME = 2;
                break;
        }
        lastDay = day;
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
