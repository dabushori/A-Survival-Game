using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{

    public static float SensitivityX = 7f;
    public static float SensitivityY = 7f;

    public static int Quality = 2;

    public static int worldDepth, worldWidth;

    public static TimeController timeController;

    public static string mobsPath = "Prefabs/Mobs/";
    public static string treesPath = "World/Trees/";
    public static string rocksPath = "World/Rocks/";
    public static string ironsPath = "World/Iron/";
    public static string goldsPath = "World/Gold/";
    public static string diamondsPath = "World/Diamond/";
    public static string furniturePath = "Prefabs/Furniture/";
    public static string itemsToHoldPath = "Prefabs/3DItems/";
}
