using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{

    public AudioMixer audioMixer;

    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDD;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDD.ClearOptions();
        // resolution drop down only get lists
        List<string> resolutionsList = new List<string>();
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string resolution = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            resolutionsList.Add(resolution);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

        }
        resolutionDD.AddOptions(resolutionsList);
        resolutionDD.value = currentResolutionIndex;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex]; 
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetMusic(float music)
    {
        audioMixer.SetFloat("music", music);
    }
}
