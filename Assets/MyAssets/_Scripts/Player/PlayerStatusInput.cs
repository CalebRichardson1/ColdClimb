using ColdClimb.Global;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    public class PlayerStatusInput : MonoBehaviour{
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
            switch (GameManager.CurrentState)
            {
                case GameState.MainGame: GameManager.UpdateGameState(GameState.StatusScreen);
                    break;
                case GameState.StatusScreen: GameManager.UpdateGameState(GameState.MainGame);
                    break;
                default: break;
            }
        }

        private void ContextAction(InputAction.CallbackContext context){
            switch(GameManager.CurrentState){
                case GameState.ContextScreen: GameManager.UpdateGameState(GameState.StatusScreen);
                break;
                case GameState.StatusScreen: GameManager.UpdateGameState(GameState.MainGame);
                break;
            } 
        }
    }
}
