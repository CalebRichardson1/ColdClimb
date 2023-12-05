using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColdClimb.UI{
    public class OptionsController : MonoBehaviour{
        [Header("Sound")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Video")]
        [SerializeField] private TMP_Text resolutionLabel;
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Gameplay")]
        [SerializeField] private Slider lookSensitivitySlider;

        private OptionsData OptionsData => SystemManager.Instance.OptionsData;

        private void Awake(){
            OptionsData.LoadValuesCallback += UpdateMenu;
        }

        private void OnDestroy(){
            OptionsData.LoadValuesCallback -= UpdateMenu;
        }

        public void UpdateMenu(){
            masterVolumeSlider.value = OptionsData.settings.masterVolume;
            musicVolumeSlider.value = OptionsData.settings.musicVolume;
            sfxVolumeSlider.value = OptionsData.settings.sfxVolume;
            lookSensitivitySlider.value = OptionsData.settings.lookSensitivity;
            fullscreenToggle.isOn = OptionsData.settings.isFullScreen;
            SetResolutionLabel(OptionsData.settings.resolution);
        }

        public void MasterVolumeChange(float amount){
            OptionsData.settings.masterVolume = amount;
            OptionsData.ApplySettings(false);
        }

        public void MusicVolumeChange(float amount){
            OptionsData.settings.musicVolume = amount;
            OptionsData.ApplySettings(false);
        }

        public void SFXVolumeChange(float amount){
            OptionsData.settings.sfxVolume = amount;
            OptionsData.ApplySettings(false);
        }

        public void ResLeft(){
            OptionsData.CycleResLeft();
            SetResolutionLabel(OptionsData.GetCurrentSelectedRes());
        }

        public void ResRight(){
            OptionsData.CycleResRight();
            SetResolutionLabel(OptionsData.GetCurrentSelectedRes());
        }

        public void SetFullscreen(bool state) => OptionsData.settings.isFullScreen = state;

        public void ApplyVideoSettings(){
            OptionsData.ApplySettings(true);
        }

        public void LookSensChange(float amount){
            OptionsData.settings.lookSensitivity = amount;
            OptionsData.ApplySettings(false);
        }

        private void SetResolutionLabel(VideoResolution resolution){
            resolutionLabel.text = $"{resolution.horizontal} x {resolution.vertical}";
        }
    }
}
