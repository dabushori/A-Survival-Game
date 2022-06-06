using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance;


    [SerializeField]
    AudioMixerGroup game;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioClip clip, Vector3 position, float scale)
    {
        PlayAtPoint(clip, position, scale, game);
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        PlayAtPoint(clip, position, 1, game);
    }

    private void PlayAtPoint(AudioClip clip, Vector3 position, float volume = 1.0f, AudioMixerGroup group = null)
    {
        if (clip == null) return;
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        if (group != null)
            audioSource.outputAudioMixerGroup = group;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
}
