using ColdClimb.Global;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    public class PlayerPauseBehaviour : MonoBehaviour{
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private RectTransform defaultButton;
        private InputManager InputManager => ResourceLoader.InputManager;

        private void Awake(){
            InputManager.ReturnPauseAction().started += PauseGameAction;
            pauseMenu.SetActive(false);
        }   

        private void OnDestroy(){
            InputManager.ReturnPauseAction().started -= PauseGameAction;
        }

        private void PauseGameAction(InputAction.CallbackContext context){
            switch (GameManager.CurrentState){
                case GameState.MainGame: PauseGame();
                break;
                case GameState.PauseMenu: ResumeGame();
                break;
                case GameState.NoteScreen: ResumeGame();
                break;
            }
        }

        private void PauseGame(){
                MenuSelector.Instance.SetDefaultSelectedObject(defaultButton);
                pauseMenu.SetActive(true);
                GameManager.UpdateGameState(GameState.PauseMenu);
        } 

        public void ResumeGame(){
            if(pauseMenu.activeSelf){
                pauseMenu.SetActive(false);
            }
            GameManager.UpdateGameState(GameState.MainGame);
        } 

        public void MainMenu(){
            GameManager.UpdateGameState(GameState.MainMenu);
        }

        public void QuitGame(){
            Application.Quit();
        }
    }
}

