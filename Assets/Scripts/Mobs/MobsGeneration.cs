using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobsGeneration : MonoBehaviour
{
    public class MobData
    {
        int groupSizeMin, groupSizeMax;
        GameObject mobObject;
        int weight;
        bool onlyAtNight;
    }

    public int MAX_MOBS_CAPACITY;
    public List<MobData> mobsList;

    public abstract void SpawnMob();
    public abstract bool CanSpawnMob();
}
