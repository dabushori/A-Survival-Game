using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualSettings : MonoBehaviour
{
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDD;

    [SerializeField]
    TMPro.TMP_Dropdown quality;
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDD.ClearOptions();
        // resolution drop down only get lists
        List<string> resolutionsList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolution = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            resolutionsList.Add(resolution);
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }

        }
        resolutionDD.AddOptions(resolutionsList);
        resolutionDD.value = currentResolutionIndex;
        resolutionDD.RefreshShownValue();

        quality.value = GameStateController.Quality;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }
    public void SetQuality(int qualityIndex)
    {
        GameStateController.Quality = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
