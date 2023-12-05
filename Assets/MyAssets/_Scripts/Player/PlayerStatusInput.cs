using System;
using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Player{
    public class PlayerStatusInput : MonoBehaviour{
        [Header("Sounds")]
        [SerializeField] private AudioType cancelAudio;

        private InputManager InputManager => ResourceLoader.InputManager;

        private void Start(){
            InputManager.ReturnStatusAction().started += StatusAction;
            InputManager.ReturnCancelAction().started += ContextAction;
        } 
        
        private void OnDestroy(){
            InputManager.ReturnStatusAction().started -= StatusAction;
            InputManager.ReturnCancelAction().started -= ContextAction;
        } 

        private void StatusAction(InputAction.CallbackContext context){
            switch (GameManager.CurrentState){
                case GameState.MainGame: GlobalUIReference.ScreenFader.FadeFromAndToBlack(0.3f, SwitchGameState);
                    break;
                case GameState.StatusScreen: GlobalUIReference.ScreenFader.FadeFromAndToBlack(0.3f, SwitchGameState);
                    break;
                case GameState.CombineItemScreen: GameManager.UpdateGameState(GameState.StatusScreen);
                    AudioController.instance.PlayAudio(cancelAudio);
                    break;
                default: break;
            }
        }

        private void SwitchGameState(){
            switch(GameManager.CurrentState){
                case GameState.MainGame: GameManager.UpdateGameState(GameState.StatusScreen);
                break;
                case GameState.StatusScreen: GameManager.UpdateGameState(GameState.MainGame);
                break;
            }

        }

        private void ContextAction(InputAction.CallbackContext context){
            switch(GameManager.CurrentState){
                case GameState.ContextScreen: GameManager.UpdateGameState(GameState.StatusScreen);
                    AudioController.instance.PlayAudio(cancelAudio);
                    break;
                case GameState.StatusScreen: GlobalUIReference.ScreenFader.FadeFromAndToBlack(0.3f, SwitchGameState);
                    break;
                case GameState.CombineItemScreen: GameManager.UpdateGameState(GameState.StatusScreen);
                    AudioController.instance.PlayAudio(cancelAudio);
                    break;
            } 
        }
    }
}
