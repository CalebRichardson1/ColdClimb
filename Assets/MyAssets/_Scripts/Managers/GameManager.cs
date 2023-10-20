using System;
using SaveLoadSystem;
using UnityEngine;

/// <summary>
/// Handles game flow and game state, used to setup our other classes before our player is able to play the game
/// </summary>

public static class GameManager
{
   public static GameState CurrentState {get; private set;}

   public static event Action<GameState> OnGameStateChange;

   public static void UpdateGameState(GameState newState){
    if(CurrentState == newState) return;
    CurrentState = newState;

    ResourceLoader.GlobalLogger.Log("The game state is: " + newState);
        switch (newState)
        {
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
            case GameState.SavingGame:
                HandleSavingGame();
                break;
            case GameState.LoadingGame:
                HandleLoadingGame();
                break;
            case GameState.NewGame:
                HandleNewGame();
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }

    private static void HandleLoadingGame(){
        GameDataHandler.LoadGame();
    }

    private static void HandleSavingGame(){
        GameDataHandler.SaveGame();
    }

    private static void HandleStatus(){
        
    }

    private static void HandleMainGame(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    private static void HandleMainMenu(){
        Cursor.lockState = CursorLockMode.None;
    }

    public static void HandleNewGame(){
        GameDataHandler.NewGame();
    }
}


public enum GameState{
    NewGame,
    SavingGame,
    LoadingGame,
    MainMenu,
    MainGame,
    StatusScreen,
    ContextScreen,
    Cutscene,
    GameOver,
    PuzzleMiniGame,
}
