using UnityEngine;

namespace ColdClimb.Audio{
    public class PlaySoundOnTrigger : MonoBehaviour{
        [Header("Audio Types")]
        [SerializeField] private AudioType[] muteAudioTypes;
        [SerializeField] private AudioType[] audioToPlay;
        [Header("Audio Options")]
        [SerializeField] private bool includeFade;
        [SerializeField] private bool stopAudioOnExit;
        [SerializeField] private bool loopAudio;
        [SerializeField] private float fadeAmount;

        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("Player")) return;

            if(muteAudioTypes.Length != 0){
                // Stop each audio type in mute Audio
                foreach (AudioType audioType in muteAudioTypes){
                    AudioController.instance.StopAudio(audioType, includeFade, fadeAmount);
                }
            }

            if(audioToPlay.Length != 0){
                // Play each audio type in the audio to play
                foreach (AudioType audio in audioToPlay){
                    switch(loopAudio){
                        case true: AudioController.instance.PlayLoopingAudio(audio, includeFade, fadeAmount);
                        break;
                        case false: AudioController.instance.PlayAudio(audio, includeFade, fadeAmount);
                        break;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other){
            if(!other.CompareTag("Player")) return;

            if(!stopAudioOnExit) return;

            if(audioToPlay.Length != 0){
                // Stop each audio type played
                foreach (AudioType audioType in audioToPlay){
                    AudioController.instance.StopAudio(audioType, includeFade, fadeAmount);
                }
            }
        }
    }
}
