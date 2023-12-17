using System;
using System.Collections;
using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.Events;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Generic{
    public class Cutscene : MonoBehaviour{
        public string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }
        [Header("Dialogue Settings")]
        [SerializeField] private Dialogue beforeActionDialogue;
        [SerializeField] private Dialogue afterActionDialogue;

        [Header("Transition Settings")]
        [SerializeField] private float fadeTime = 1f;
        [SerializeField] private float waitTime = 3f;

        [Header("Unity Event Settings")]
        [SerializeField] private bool repeatableCutscene;
        [SerializeField] private UnityEvent duringBlackScreenAction;

        [Header("Audio Settings")]
        [SerializeField] private AudioType audioToPlayDuringBlackScreen;
        [SerializeField] private bool loopAudio;
        private ScenesData ScenesData => ResourceLoader.ScenesData;

        private WaitForSecondsRealtime blackScreenWaitTime;

        private bool playedCutscene;

        private void Awake() {
            blackScreenWaitTime = new WaitForSecondsRealtime(waitTime);
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;
        }

        private void SaveState(){
            if(ScenesData.CurrentSceneData.CutscenesActivated.ContainsKey(id)){
                ScenesData.CurrentSceneData.CutscenesActivated.Remove(id);
            }
            ScenesData.CurrentSceneData.CutscenesActivated.Add(id, playedCutscene);
        }

        private void LoadData(){
             ScenesData.CurrentSceneData.CutscenesActivated.TryGetValue(id, out playedCutscene);
            if(playedCutscene && !repeatableCutscene){
                //Do the action from the cutscene and disable the cutscene component
                duringBlackScreenAction?.Invoke();
                enabled = false;
            }
        }

        private void OnDestroy(){
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }

        public void StartCutscene(){
            if(playedCutscene && !repeatableCutscene) return;
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

            playedCutscene = true;
        }

        private IEnumerator DuringBlackScreenCoroutine(){
            yield return blackScreenWaitTime;
            GlobalUIReference.ScreenFader.FadeFromBlack(fadeTime, AfterBlackScreen);
        }
    }
}
