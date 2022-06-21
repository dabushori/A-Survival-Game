using UnityEngine;

/*
 * Class for local game constants
 */
public class GameStateController : MonoBehaviour
{
    // Player's mouse sensitivity
    public static float SensitivityX = 7f;
    public static float SensitivityY = 7f;

    // Display quality
    public static int Quality = 2;
    
    // Current world dimensions
    public static int worldDepth, worldWidth;

    // The time controller object of the current world (for easy access)
    public static TimeController timeController;

    // Constant paths to prefabs
    public static string mobsPath = "Prefabs/Mobs/";
    public static string treesPath = "World/Trees/";
    public static string mushroomPath = "World/Mushroom/";
    public static string rocksPath = "World/Rocks/";
    public static string ironsPath = "World/Iron/";
    public static string goldsPath = "World/Gold/";
    public static string diamondsPath = "World/Diamond/";
    public static string furniturePath = "Prefabs/Furniture/";
    public static string itemsToHoldPath = "Prefabs/3DItems/";

    // Layer masks of different object types
    public static LayerMask surfaceLayer = 1 << 6;
    public static LayerMask worldObjectsLayer = 1 << 7;
    public static LayerMask groundLayers = 1 << 6 | 1 << 7;
    public static LayerMask playersLayer = 1 << 8;
    public static LayerMask hostileMobsLayer = 1 << 9;
    public static LayerMask friendlyMobsLayer = 1 << 10;
    public static LayerMask mobsLayer = 1 << 9 | 1 << 10;
}
