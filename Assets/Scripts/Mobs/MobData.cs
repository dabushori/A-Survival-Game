using UnityEngine;
/*
 * The class saves the mob data (for the generation)
 */
[CreateAssetMenu(fileName = "New MobsData", menuName = "Game Data/MobData")]
public class MobData : ScriptableObject
{
    // The name of the mob
    public string mobName;

    // The minimum and maximum sizes of the group that can spawn in a single spawn cycle
    public int groupSizeMin, groupSizeMax;

    // The mob's prefab that will be generated
    public GameObject mobObject;

    // The probability that the mob will be spawned is weight / total weight of all the mobs
    public int weight;

    // Can the mob spawn only at night
    public bool onlyAtNight;
}