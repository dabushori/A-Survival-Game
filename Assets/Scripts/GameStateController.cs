using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public static int Seed;

    public static float SensitivityX = 5f;
    public static float SensitivityY = 10f;

    public static float VolumeMixer = -10f;
    public static float MusicMixer = -10f;

    public static int Quality = 2;




    public static int worldDepth, worldWidth;

    public static TimeController timeController;
}
