using System;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Global.LevelSystem;
using UnityEngine;

namespace ColdClimb.Global{
    /// <summary>
    /// Handles game flow and game state, used to setup our other classes before our player is able to play the game
    /// </summary>

    public static class GameManager{
    public static GameState CurrentState {get; private set;}

    public static event Action<GameState> OnGameStateChange;

    public static void UpdateGameState(GameState newState){
        CurrentState = newState;

        ResourceLoader.GlobalLogger.Log("The game state is: " + newState);
            switch (newState){
                case GameState.MainMenu:
                    HandleMainMenu();
                    break;
                case GameState.MainGame:
                    HandleMainGame();
                    break;
                case GameState.StatusScreen:
                    HandleStatus();
                    break;
                case GameState.ContextScreen:
                    break;
                case GameState.Cutscene:
                    break;
                case GameState.GameOver:
                    break;
                case GameState.PuzzleMiniGame:
                    break;
                case GameState.PauseMenu: 
                    HandlePauseMenu();
                    break;
            }

            OnGameStateChange?.Invoke(newState);
        }

        private static void HandlePauseMenu(){
            Cursor.lockState = CursorLockMode.None;
        }

        private static void HandleStatus(){
            
        }

        private static void HandleMainGame(){
            Cursor.lockState = CursorLockMode.Locked;
        }

        private static void HandleMainMenu(){
            Cursor.lockState = CursorLockMode.None;
            
            SceneDirector.Instance.LoadScene(SceneIndex.MAIN_MENU, GameDataHandler.IntializeSaveValues);
        }
    }


    public enum GameState{
        MainMenu,
        MainGame,
        PauseMenu,
        StatusScreen,
        ContextScreen,
        Cutscene,
        GameOver,
        PuzzleMiniGame,
    }
}
