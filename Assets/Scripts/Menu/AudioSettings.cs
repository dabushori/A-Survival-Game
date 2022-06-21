using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*
 * The class is in charge of the audio settings
 */
public class AudioSettings : MonoBehaviour
{
    [SerializeField]
    Slider volumeSlider; // volume slider
    [SerializeField]
    Slider musicSlider; // music slider
    [SerializeField]
    Slider gameSlider; // game slider
    [SerializeField]
    Slider sfxSlider; // sfx slider

    private void Start()
    {
        // sync the sliders to the current value of the mixers
        audioMixer.GetFloat("volume", out float volume);
        if (volume == -80f) volumeSlider.value = -50f;
        else volumeSlider.value = volume;

        audioMixer.GetFloat("music", out float music);
        if (music == -80f) musicSlider.value = -50f;
        else musicSlider.value = music;

        audioMixer.GetFloat("game", out float game);
        if (game == -80f) gameSlider.value = -50f;
        else gameSlider.value = game;

        audioMixer.GetFloat("sfx", out float sfx);
        if (sfx == -80f) sfxSlider.value = -50f;
        else sfxSlider.value = sfx;
    }

    [SerializeField]
    AudioMixer audioMixer; // the audio mixer we use in teh game

    /*
     * change the volume level
     */
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        if (volume == -50f) audioMixer.SetFloat("volume", -80f);
    }

    /*
     * change the music level
     */
    public void SetMusic(float music)
    {
        audioMixer.SetFloat("music", music);
        if (music == -50f) audioMixer.SetFloat("music", -80f);
    }

    /*
     * change the game sound level
     */
    public void SetGame(float game)
    {
        audioMixer.SetFloat("game", game);
        if (game == -50f) audioMixer.SetFloat("game", -80f);
    }

    /*
     * change the sfx sound level
     */
    public void SetSFX(float sfx)
    {
        audioMixer.SetFloat("sfx", sfx);
        if (sfx == -50f) audioMixer.SetFloat("sfx", -80f);
    }
}
