using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance; // singleton


    [SerializeField]
    AudioMixerGroup game; // the mixer that control the sound level

    void Awake()
    {
        Instance = this;
    }

    /*
     * play the sound at point with scale
     */
    public void PlaySound(AudioClip clip, Vector3 position, float scale)
    {
        PlayAtPoint(clip, position, scale, game);
    }

    /*
     * The function creates an original play at point sound object and destroys it
     */
    private void PlayAtPoint(AudioClip clip, Vector3 position, float volume = 1.0f, AudioMixerGroup group = null)
    {
        if (clip == null) return; // no clip

        // set position and create the object
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        // cast the object to an audiosource object
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        if (group != null) // no mixer
            audioSource.outputAudioMixerGroup = group;
        // set variables in the audiosource
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play(); // play
        Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale)); // destroy the object after it finishes playing
    }
}
