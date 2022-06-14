using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField]
    Slider volume;
    [SerializeField]
    Slider music;

    private void Start()
    {
        volume.value = GameStateController.VolumeMixer;
        music.value = GameStateController.MusicMixer;
    }

    public AudioMixer audioMixer;
    public void SetVolume(float volume)
    {
        GameStateController.VolumeMixer = volume;
        audioMixer.SetFloat("volume", volume);
    }

    public void SetMusic(float music)
    {
        GameStateController.MusicMixer = music;
        audioMixer.SetFloat("music", music);
    }
}
