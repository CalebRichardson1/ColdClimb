using UnityEngine;

namespace ColdClimb.Audio{       
    public class ObjectAudio : MonoBehaviour{
        // members
        [SerializeField] private AudioType audioToPlay;
        [SerializeField] private bool loopAudio;

        private AudioController AudioController => AudioController.instance;

        private AudioSource objectAudioSource;
#region Unity Functions
        private void Awake() {
            TryGetComponent(out objectAudioSource);
        }

        private void Start() {
            if(objectAudioSource == null){
                return;
            }
            if(loopAudio){
                AudioController.PlayLoopingAudio(audioToPlay, false, 0, 0, objectAudioSource);
            }
            else{
                AudioController.PlayAudio(audioToPlay, false, 0, 0, objectAudioSource);
            }
        }
#endregion

#region Public Functions

#endregion

#region Private Functions

#endregion
    }
}
