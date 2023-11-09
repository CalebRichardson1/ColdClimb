using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    public AudioMixerGroup audioMixer;

    public bool isLooping = false;
    [Range(0f, 1f)]
    public float clipVolume;
    [Range(0.1f, 3f)]
    public float clipPitch = 1f;

    [HideInInspector]
    public AudioSource source;
}
