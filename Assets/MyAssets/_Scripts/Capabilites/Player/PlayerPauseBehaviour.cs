using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPauseBehaviour : MonoBehaviour{
    [SerializeField] private GameObject pauseMenu;
    private InputManager InputManager => ResourceLoader.InputManager;

    private void Awake() {
        InputManager.ReturnPauseAction().started += PauseGame;
        pauseMenu.SetActive(false);
    }   

    private void OnDestroy() {
        InputManager.ReturnPauseAction().started -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context){
        if(GameManager.CurrentState != GameState.PauseMenu){
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            GameManager.UpdateGameState(GameState.PauseMenu);
        }
        else{
            ResumeGame();
        }
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        GameManager.UpdateGameState(GameState.MainGame);
    } 

    public void MainMenu(){
        Time.timeScale = 1;
        GameManager.UpdateGameState(GameState.MainMenu);
    }

    public void QuitGame(){
        Application.Quit();
    }
}

