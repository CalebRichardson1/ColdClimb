using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class TMP_AudioManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private List<Sound> Sounds;
    public static TMP_AudioManager Instance;

    [Header("Sound Settings")]
    [SerializeField] private AudioMixer mainMixer;

    private const string MASTER_VOLUME = "MasterVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";


    private void Awake() {
        if(Instance == null){
            Instance = this;
        }
        
        else{
            Destroy(gameObject);
            return;
        }

        foreach (Sound sound in Sounds){
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.clipVolume;
            sound.source.pitch = sound.clipPitch;
            sound.source.loop = sound.isLooping;
            sound.source.outputAudioMixerGroup = sound.audioMixer;
        }

        MainMenuController.OnButtonPress += () => PlaySound("ButtonPress");
        GameManager.OnGameStateChange += OnGameState;
    }

    private void OnDestroy() {
        MainMenuController.OnButtonPress -= () => PlaySound("ButtonPress");
        GameManager.OnGameStateChange -= OnGameState;
    }

    private void OnGameState(GameState state){
        if(state != GameState.MainMenu){
            GetSound("MainMenuMusic").source.Stop();
        }
    }

    public void PlayMenuMusic(){
        PlaySound("MainMenuMusic");
    }

    public void PlaySound(string soundName){
        Sound validSound = Sounds.FirstOrDefault(name => name.name == soundName);
        if(validSound == null) return;

        validSound.source.Play();
    }

    public Sound GetSound(string soundName){
        Sound validSound = Sounds.FirstOrDefault(name => name.name == soundName);
        return validSound;
    }

    public void UpdateMixerVolume(){

    }
}
