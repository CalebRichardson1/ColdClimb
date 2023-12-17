using System;
using System.Collections.Generic;
using ColdClimb.Global.LevelSystem;
using UnityEngine;

namespace  ColdClimb.Global.SaveSystem{
    [CreateAssetMenu(menuName = "Save Data Holders/Scene Data Holder", fileName = "NewSceneDataHolder")]
    public class ScenesData : DataHolder{
        public override event Action LoadValuesCallback;
        public SceneData CurrentSceneData;
        public GameScenesData gameScenesData;

        public override void Intialize(){
            GameDataHandler.OnSaveCallback += OnSave;
            GameDataHandler.OnLoadCallback += OnLoad;
            GameDataHandler.OnNewGameCallback += OnNewGame;
            SceneDirector.LoadedScene += OnSceneLoaded;
        }

        private void OnDisable() {
            GameDataHandler.OnSaveCallback -= OnSave;
            GameDataHandler.OnLoadCallback -= OnLoad;
            GameDataHandler.OnNewGameCallback -= OnNewGame;
            SceneDirector.LoadedScene -= OnSceneLoaded;
        }

        private void OnSceneLoaded(SceneIndex scene){
            if(scene == SceneIndex.MANAGER || scene == SceneIndex.MAIN_MENU || scene == SceneIndex.DEVPLAYGROUND) return;

                //See if we have a scene data with the current scene index
                foreach (SceneData sceneData in gameScenesData.SceneDatasModified){
                    if(sceneData.associatedSceneIndex == (int)scene){
                        //found it!
                        CurrentSceneData = sceneData;
                        return;
                    }
                }

            SceneData incomingSceneData = new()
            {
                //Create one if one doesn't exists
                associatedSceneIndex = (int)scene,

                //Initailize the Dictionaries
                DialogueTriggersActivated = new(),
                InspectObjectsUpdated = new(),
                ItemsPickuped = new(),
                ItemLocksUnlocked = new(),
                DoorsUnlocked = new(),
                DoorsOpened = new(),
                EnemiesDied = new(),
                ReadNotes = new(),
                KeyPadsUnlocked = new(),
                CutscenesActivated = new()
            };

            //Set the current scene data
            CurrentSceneData = incomingSceneData;
            gameScenesData.SceneDatasModified.Add(incomingSceneData);
            Debug.Log(gameScenesData.SceneDatasModified.Count);
        }

        protected override void OnLoad(){
            gameScenesData = GameDataHandler.CurrentSaveData.CurrentScenesData;

            LoadValuesCallback?.Invoke();
        }

        protected override void OnSave(){
            //Update the file values
            GameDataHandler.CurrentSaveData.CurrentScenesData = gameScenesData;
        }

        protected override void OnNewGame(){
            gameScenesData.SceneDatasModified.Clear();
        }
    }
}
