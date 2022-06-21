using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobsGeneration : MonoBehaviour
{
    [SerializeField]
    int MAX_HOSTILE_MOBS_CAPACITY; // the max amount of mobs the player can have around him
    [SerializeField]
    int MAX_FRIENDLY_MOBS_CAPACITY; // the max amount of mobs the player can have around him
    List<MobData> hostileMobsList; // data of all the hostile mobs
    List<MobData> friendlyMobsList; // data of all the friendly mobs

    int sumOfHostileWeights, sumOfFriendlyWeights; // the sum of all the weights of all the mobs (used for chosing who to spawn)

    [SerializeField]
    int spawnRadiusMax; // max radius of spawn
    [SerializeField]
    int spawnRadiusMin; // min radius of spawn

    /*
     * The function spawn the mob (hostile or not depending on isHostile)
     */
    public void SpawnMob(bool isHotile)
    {
        // select random mob
        MobData mob = SelectRandomMob(isHotile);
        // choose the amount of the mob to spawn
        int numOfMobs = Random.Range(mob.groupSizeMin, mob.groupSizeMax + 1);

        // choose a random spawn location for each mob
        for (int i = 0; i < numOfMobs; ++i)
        {
            // random location around the player (adding to transform.position)
            Vector2 location = Random.insideUnitCircle * spawnRadiusMax;
            Vector3 mobPosition = new Vector3(location.x, 0, location.y) + transform.position;
            // if the mob is closer than the minRadius we dont spawn him and move to the next mob
            if (Vector3.Distance(mobPosition, new Vector3(transform.position.x, 0, transform.position.z)) < spawnRadiusMin)
            {
                continue;
            }
        
            // check spawn rules
            if (CanSpawnMob(mob, isHotile, mobPosition))
            {
                PhotonNetwork.InstantiateRoomObject(GameStateController.mobsPath + mob.mobName, mobPosition, Quaternion.identity);
            }
        }
    }

    /*
     * The function select a random mob to spawn depending on weights (hostile or not depending on isHostile)
     */
    MobData SelectRandomMob(bool isHotile)
    {
        // put the mobs and sumofwights according to if hostile or not
        int sumOfWeights = isHotile ? sumOfHostileWeights : sumOfFriendlyWeights;
        List<MobData> mobsList = isHotile ? hostileMobsList : friendlyMobsList;
        // random number for choosing mob
        int rnd = Random.Range(0, sumOfWeights + 1);
        foreach (MobData md in mobsList)
        {
            // if the mob weight is more than rnd choose the mob
            if (rnd <= md.weight)
            {
                return md;
            }
            // remove from rnd the mob wiight and move to the next mob
            rnd -= md.weight;
        }
        // if no mob was chosen just choose the last mob
        return mobsList[mobsList.Count - 1];
    }

    /*
     * The function checks if we will be able to spawn the mob according to his data, position and if he is hostile.
     */
    public bool CanSpawnMob(MobData mobData, bool isHostile, Vector3 position)
    {
        if (GameStateController.timeController == null) return false;
        // if the mob is to close to a game object
        if (Physics.OverlapSphere(position, 2f, GameStateController.worldObjectsLayer).Length != 0) return false;
        // if the mob can only spawn at night
        if (mobData.onlyAtNight)
        {
            // check if the mob is in the map, if it's night and if the mob pass the capacity around the player
            return position.x > 0 && position.z > 0 && position.x < GameStateController.worldWidth && position.z < GameStateController.worldDepth &&
                GameStateController.timeController.IsNight() && (isHostile ?
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, GameStateController.hostileMobsLayer).Length < MAX_HOSTILE_MOBS_CAPACITY) :
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, GameStateController.friendlyMobsLayer).Length < MAX_FRIENDLY_MOBS_CAPACITY));
        }
        // check if the mob is in the map and if the mob pass the capacity around the player
        return position.x > 0 && position.z > 0 && position.x < GameStateController.worldWidth && position.z < GameStateController.worldDepth &&
            (isHostile ?
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, GameStateController.hostileMobsLayer).Length < MAX_HOSTILE_MOBS_CAPACITY) :
            (Physics.OverlapSphere(transform.position, spawnRadiusMax, GameStateController.friendlyMobsLayer).Length < MAX_FRIENDLY_MOBS_CAPACITY));
    }
    


    float previousHostileSpawnTime = -Mathf.Infinity, previousFriendlySpawnTime = -Mathf.Infinity; // last spawn time
    [SerializeField]
    float HOSTILE_MOB_SPAWN_CYCLE_TIME;// in how much time the mobs spawn
    [SerializeField]
    float FRIENDLY_MOB_SPAWN_CYCLE_TIME; // in how much time the mobs spawn

    public void Update()
    {
        // spawn mobs every HOSTILE/FRIENDLY_MOB_SPAWN_CYCLE_TIME
        if (Time.time - previousHostileSpawnTime > HOSTILE_MOB_SPAWN_CYCLE_TIME)
        {
            if (GameStateController.timeController != null)
            {
                // spawn hostile mobs 5 times faster on average at night
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
        // update the difficulty according to the day it is
        UpdateDifficulty();
    }

    int lastDay;
    private void UpdateDifficulty()
    {
        // if already changed difficulty today return
        if (GameStateController.timeController == null) return;
        int day = GameStateController.timeController.GetDay();
        if (day == lastDay) return;
        // change difficulty according to the day
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
        // save the mobdata
        hostileMobsList = new List<MobData>(Resources.LoadAll("Mobs/Hostile Mobs", typeof(MobData)).Cast<MobData>());
        friendlyMobsList = new List<MobData>(Resources.LoadAll("Mobs/Friendly Mobs", typeof(MobData)).Cast<MobData>());
        // sum the weights of the mobs for when we want to spawn them randomaly
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
