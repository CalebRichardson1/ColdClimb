using System.Linq;
using ColdClimb.Global;
using ColdClimb.Global.LevelSystem;
using UnityEngine;

namespace ColdClimb.Audio{
    public class AmbienceDirector : MonoBehaviour{
        // members
        [SerializeField] private AudioAmbience[] ambiences;

        private AudioController AudioController => AudioController.instance;

        [System.Serializable]
        public class AudioAmbience{
            public SceneIndex sceneToPlay;
            public AudioType ambienceAudio;
            public bool includeFading;
            public float fadeTime = 1;
        }

        private AudioType previousAmbience = AudioType.None;

#region Unity Functions
        private void Awake() {
            Configure();
        }
        private void OnDisable() {
            Dispose();
        }
#endregion

#region Private Functions
        private void Configure(){
            SceneDirector.LoadedScene += PlayAmbience;
        }

        private void Dispose(){
            SceneDirector.LoadedScene -= PlayAmbience;
        }

        private void PlayAmbience(SceneIndex newSceneIndex){
            if(newSceneIndex == SceneIndex.MANAGER && previousAmbience != AudioType.None|| newSceneIndex == SceneIndex.MAIN_MENU && previousAmbience != AudioType.None){
                StopAmbience();
                return;
            }
            AudioAmbience ambienceToPlay = ambiences.FirstOrDefault(ambience => ambience.sceneToPlay == newSceneIndex && ambience.ambienceAudio != AudioType.None);
            if(ambienceToPlay == null && previousAmbience == AudioType.None){
                return;
            }

            if(ambienceToPlay.ambienceAudio == previousAmbience){
                return;
            }

            if(previousAmbience != AudioType.None){
                StopAmbience();
            }

            AudioController.PlayLoopingAudio(ambienceToPlay.ambienceAudio, ambienceToPlay.includeFading, ambienceToPlay.fadeTime);
            previousAmbience = ambienceToPlay.ambienceAudio;
        }

        private void StopAmbience(){
            AudioController.StopAudio(previousAmbience);
            previousAmbience = AudioType.None;
        }
#endregion
    }
}

