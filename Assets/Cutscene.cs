using System.Collections;
using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.Events;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Generic{
    public class Cutscene : MonoBehaviour{
        [Header("Dialogue Settings")]
        [SerializeField] private Dialogue beforeActionDialogue;
        [SerializeField] private Dialogue afterActionDialogue;

        [Header("Transition Settings")]
        [SerializeField] private float fadeTime = 1f;
        [SerializeField] private float waitTime = 3f;

        [Header("Unity Event Settings")]
        [SerializeField] private UnityEvent duringBlackScreenAction;

        [Header("Audio Settings")]
        [SerializeField] private AudioType audioToPlayDuringBlackScreen;
        [SerializeField] private bool loopAudio;

        private WaitForSecondsRealtime blackScreenWaitTime;

        private void Awake() {
            blackScreenWaitTime = new WaitForSecondsRealtime(waitTime);
        }

        public void StartCutscene(){
            if(beforeActionDialogue.sentences.Length != 0){
                GlobalUIReference.DialogueController.StartDialogue(beforeActionDialogue);
                return;
            }
            else{
                GlobalUIReference.ScreenFader.FadeToBlack(fadeTime, DuringBlackScreen);
            }
        }

        public void DuringBlackScreen(){
            GameManager.UpdateGameState(GameState.Cutscene);

            duringBlackScreenAction?.Invoke();
            if(audioToPlayDuringBlackScreen != AudioType.None){
                if(loopAudio){
                    AudioController.instance.PlayLoopingAudio(audioToPlayDuringBlackScreen);
                }
                else{
                    AudioController.instance.PlayAudio(audioToPlayDuringBlackScreen);
                }
            }

            StartCoroutine(DuringBlackScreenCoroutine());
        }

        private void AfterBlackScreen(){
            if(afterActionDialogue.sentences.Length != 0){
                GlobalUIReference.DialogueController.StartDialogue(afterActionDialogue);
                return;
            }

            GameManager.UpdateGameState(GameState.MainGame);
        }

        private IEnumerator DuringBlackScreenCoroutine(){
            yield return blackScreenWaitTime;
            GlobalUIReference.ScreenFader.FadeFromBlack(fadeTime, AfterBlackScreen);
        }
    }
}
