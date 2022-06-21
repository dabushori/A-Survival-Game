using UnityEngine;
/*
 * The class saves the mob data (for the generation)
 */
[CreateAssetMenu(fileName = "New MobsData", menuName = "Game Data/MobData")]
public class MobData : ScriptableObject
{
    public string mobName;
    public int groupSizeMin, groupSizeMax;
    public GameObject mobObject;
    public int weight;
    public bool onlyAtNight;
}