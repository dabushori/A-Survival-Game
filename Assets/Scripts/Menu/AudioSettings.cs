using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField]
    Slider volumeSlider;
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    Slider gameSlider;
    [SerializeField]
    Slider sfxSlider;

    private void Start()
    {
        audioMixer.GetFloat("volume", out float volume);
        volumeSlider.value = volume;
        audioMixer.GetFloat("music", out float music);
        musicSlider.value = music;
        audioMixer.GetFloat("game", out float game);
        gameSlider.value = game;
        audioMixer.GetFloat("sfx", out float sfx);
        sfxSlider.value = sfx;
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

    public void SetGame(float game)
    {
        audioMixer.SetFloat("game", game);
    }
    
    public void SetSFX(float sfx)
    {
        audioMixer.SetFloat("sfx", sfx);
    }
}
