using UnityEngine;

[CreateAssetMenu(fileName = "New MobsData", menuName = "Game Data/MobData")]
public class MobData : ScriptableObject
{
    public int groupSizeMin, groupSizeMax;
    public GameObject mobObject;
    public int weight;
    public bool onlyAtNight;
}