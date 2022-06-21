using System.Collections.Generic;
using UnityEngine;

/*
 * The class is in charge of the visual settings
 */
public class VisualSettings : MonoBehaviour
{
    Resolution[] resolutions; // the resolutions
    [SerializeField]
    TMPro.TMP_Dropdown resolutionDD; // dropdown of all the resolutions

    [SerializeField]
    TMPro.TMP_Dropdown quality; // dropdown of all the graphics qualities
    void Start()
    {
        // get all the resolutions of the screen
        resolutions = Screen.resolutions;
        resolutionDD.ClearOptions();
        // resolution drop down only get lists
        List<string> resolutionsList = new List<string>();
        int currentResolutionIndex = 0; // current resolution
        // add all the resolutions to the list as strings
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolution = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz"; // syntax
            resolutionsList.Add(resolution);
            // is the current resolution
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }

        }
        // add the list to the dropdowns
        resolutionDD.AddOptions(resolutionsList);
        resolutionDD.value = currentResolutionIndex; // the current resolution
        resolutionDD.RefreshShownValue();
        // save the current quality
        quality.value = GameStateController.Quality;
    }

    /*
     * change resolution
     */
    public void SetResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    /*
     * change quality of the game
     */
    public void SetQuality(int qualityIndex)
    {
        GameStateController.Quality = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    /*
     * toggle the full screen
     */
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
