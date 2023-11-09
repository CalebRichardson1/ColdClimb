using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SaveLoadSystem{
    [CreateAssetMenu(menuName = "Save Data Holders/Options Data Holder", fileName = "NewOptionsDataHolder")] 
    public class OptionsData : DataHolder
    {
        public Options settings = new();

        public override event Action LoadValuesCallback;

        [Header("Audio")]
        [SerializeField] private AudioMixer mainMixer;
        [Header("Video")]
        [SerializeField] private List<VideoResolution> videoResolutions = new();

        private int selectedResolution;

        private void OnEnable() {
            GameDataHandler.OnSaveOptionsCallback += OnSaveSettings;
            GameDataHandler.OnLoadOptionsCallback += OnLoadSettings;
            GameDataHandler.OnNewOptionsCallback += SetDefaultOptions;
        }

        private void OnDisable() {
            GameDataHandler.OnSaveOptionsCallback -= OnSaveSettings;
            GameDataHandler.OnLoadOptionsCallback -= OnLoadSettings;
            GameDataHandler.OnNewOptionsCallback -= SetDefaultOptions;
        }


        private void OnSaveSettings(){
            GameDataHandler.CurrentOptions.CurrentSettings = settings;
        }

        private void OnLoadSettings(){
            settings = GameDataHandler.CurrentOptions.CurrentSettings;

            LoadValuesCallback?.Invoke();
            // Set the correct selected res.
            selectedResolution = videoResolutions.FindIndex(item => item.Equals(settings.resolution));
            ApplySettings(true);
        }

        private void SetDefaultOptions(){
            // Setting the default values.
            settings.masterVolume = 1;
            settings.musicVolume = 1;
            settings.sfxVolume = 1;

            if(videoResolutions.Any(res => Screen.width == res.horizontal 
                                    && Screen.height == res.vertical)){
                                        var res = videoResolutions.FirstOrDefault(res => Screen.width == res.horizontal 
                                        && Screen.height == res.vertical);
                                        var index = videoResolutions.FindIndex(item => item.Equals(res));
                                        if(index != -1){
                                            selectedResolution = index;
                                        }
                                    }
            else{
                var defaultIndex = videoResolutions.FindIndex(res => res.horizontal == 1920 && res.vertical == 1080);
                 selectedResolution = defaultIndex;
            } 
            settings.isFullScreen = true;
            LoadValuesCallback?.Invoke();
            ApplySettings(true);
        }

        public void CycleResLeft(){
            selectedResolution--;
            if(selectedResolution < 0) selectedResolution = 0;
        }

        public void CycleResRight(){
            selectedResolution++;
            if(selectedResolution > videoResolutions.Count - 1) selectedResolution = videoResolutions.Count - 1;
        }

        public VideoResolution GetCurrentSelectedRes(){
            return videoResolutions[selectedResolution];
        }

        public void ApplySettings(bool applyVideo){
            if(applyVideo){
                var res = videoResolutions[selectedResolution];
                settings.resolution.SetValues(res.horizontal, res.vertical);

                Screen.SetResolution(settings.resolution.horizontal,
                                 settings.resolution.vertical, settings.isFullScreen);
            }
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(settings.masterVolume) * 20);
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(settings.musicVolume) * 20);
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(settings.sfxVolume) * 20);

            GameDataHandler.SaveSettings();
        }
    }
}
