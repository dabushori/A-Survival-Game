using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField]
    Slider volumeSlider;
    [SerializeField]
    Slider musicSlider;

    private void Start()
    {
        audioMixer.GetFloat("volume", out float volume);
        volumeSlider.value = volume;
        audioMixer.GetFloat("music", out float music);
        musicSlider.value = music;
    }

    public AudioMixer audioMixer;
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetMusic(float music)
    {
        audioMixer.SetFloat("music", music);
    }
}
