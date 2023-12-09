using System;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Global.LevelSystem;
using UnityEngine;
using ColdClimb.Audio;
using AudioType = ColdClimb.Audio.AudioType;

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
                    break;
                case GameState.StatusScreen:
                    break;
                case GameState.ContextScreen:
                    break;
                case GameState.Cutscene:
                    break;
                case GameState.GameOver:
                    break;
                case GameState.CombineItemScreen:
                    break;
                case GameState.PauseMenu:
                    break;
                case GameState.Devscene:
                    HandleDevScene();
                    break;
                case GameState.NoteScreen:
                    break;
                case GameState.LoadingScene:
                    break;
                case GameState.DialogueScreen:
                    break;
            }

            OnGameStateChange?.Invoke(newState);
        }

        private static void HandleDevScene(){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneDirector.Instance.LoadScene(SceneIndex.DEVPLAYGROUND, GameDataHandler.TriggerNewGameValues);
        }

        private static void HandleMainMenu(){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            AudioController.instance.PlayLoopingAudio(AudioType.SOUNDTRACK_01);
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
        NoteScreen,
        CombineItemScreen,
        LoadingScene,
        DialogueScreen,
        QuestionScreen,
        KeypadScreen,
        Devscene
    }
}
