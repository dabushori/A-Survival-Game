using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SFXManager : MonoBehaviour
{

    public static SFXManager Instance;

    [SerializeField]
    AudioSource Audio;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySound(AudioClip clip)
    {
        Instance.Audio.PlayOneShot(clip);
    }
    
    public void PlaySound(AudioClip clip, float scale)
    {
        Instance.Audio.PlayOneShot(clip, scale);
    }

    public void PlaySound(AudioClip clip, Vector3 position, float scale)
    {
        AudioSource.PlayClipAtPoint(clip, position, scale);
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
}
