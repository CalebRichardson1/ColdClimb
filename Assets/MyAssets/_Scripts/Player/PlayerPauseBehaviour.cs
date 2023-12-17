using ColdClimb.Global;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    public class PlayerPauseBehaviour : MonoBehaviour{
        [SerializeField] private RectTransform defaultButton;
        [SerializeField] private RectTransform optionDefaultButton;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private GameObject optionsScreen;

        private bool pauseCooldown = false;
        private bool canPause = true;
        private InputManager InputManager => ResourceLoader.InputManager;

        private void Awake(){
            InputManager.ReturnPauseAction().started += PauseGameAction;
            GameManager.OnGameStateChange += EvaluateGameState;
            pauseMenu.SetActive(false);
        }   

        private void OnDestroy(){
            InputManager.ReturnPauseAction().started -= PauseGameAction;
            GameManager.OnGameStateChange -= EvaluateGameState;
        }

        private void EvaluateGameState(GameState state){
            if(state == GameState.MainGame || state == GameState.PauseMenu){
                if(pauseCooldown){
                    Invoke(nameof(PauseValid), 0.25f);
                    pauseCooldown = false;
                    return;
                }

                PauseValid();
                return;
            }

            // See if the state is a note state, and next time we switch back to main game, lock the pause function for a couple of seconds.
            if(state == GameState.NoteScreen){
                pauseCooldown = true;
            }

            canPause = false;
        }

        private void PauseValid(){
            canPause = true;
        }

        private void PauseGameAction(InputAction.CallbackContext context){
            if(!canPause) return;

            switch (GameManager.CurrentState){
                case GameState.MainGame: GlobalUIReference.ScreenFader.FadeToAndFromBlack(0.15f, PauseGame);
                break;
                case GameState.PauseMenu: GlobalUIReference.ScreenFader.FadeToAndFromBlack(0.15f, ResumeGame);
                break;
            }
        }

        private void PauseGame(){
            pauseMenu.SetActive(true);
            ShowPauseScreen();
            GameManager.UpdateGameState(GameState.PauseMenu);
        }

        private void ShowPauseScreen(){
            pauseScreen.SetActive(true);
            optionsScreen.SetActive(false);
            MenuSelector.Instance.SetDefaultSelectedObject(defaultButton);
        }

        private void ResumeGame(){
            if(pauseMenu.activeSelf){
                pauseMenu.SetActive(false);
            }
            GameManager.UpdateGameState(GameState.MainGame);
        } 

        public void MainMenu(){
            GameManager.UpdateGameState(GameState.MainMenu);
        }

        public void GameTransition(){
            GlobalUIReference.ScreenFader.FadeToAndFromBlack(0.15f, ResumeGame);
        }

        public void OptionsTransition(){
            GlobalUIReference.ScreenFader.FadeToAndFromBlack(0.15f, OptionsMenu);
        }

        public void PauseScreenTransition(){
            GlobalUIReference.ScreenFader.FadeToAndFromBlack(0.15f, ShowPauseScreen);
        }

        private void OptionsMenu(){
            optionsScreen.SetActive(true);
            pauseScreen.SetActive(false);
            MenuSelector.Instance.SetDefaultSelectedObject(optionDefaultButton);
        }

        public void QuitGame(){
            Application.Quit();
        }
    }
}

