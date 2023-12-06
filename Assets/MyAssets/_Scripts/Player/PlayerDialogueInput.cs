using ColdClimb.Global;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    public class PlayerDialogueInput : MonoBehaviour{
        private InputManager InputManager => ResourceLoader.InputManager;

        private bool validInput = false;

        private void Start(){
            GameManager.OnGameStateChange += (state) => validInput = state == GameState.DialogueScreen;
            InputManager.ReturnInteractAction().started += ContinueDialouge;
        }

        private void OnDestroy(){
            GameManager.OnGameStateChange -= (state) => validInput = state == GameState.DialogueScreen;
            InputManager.ReturnInteractAction().started -= ContinueDialouge;
        }

        private void ContinueDialouge(InputAction.CallbackContext context){
            if(!validInput) return;
            GlobalUIReference.DialogueController.DisplayNextSentence();
        }
    }
}
