using System;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data based on the strucs defined. DataHolder is the base from all other dataholder inherit from.
/// </summary>
namespace SaveLoadSystem{
    public abstract class DataHolder : ScriptableObject{
        public abstract event Action LoadValuesCallback;

        // Subscribe to the on save callback and on load callback to write/read the values.
        private void OnEnable() {
            GameDataHandler.OnSaveCallback += OnSave;
            GameDataHandler.OnLoadCallback += OnLoad;
            GameDataHandler.OnNewGameCallback += OnNewGame;
        }

        private void OnDisable() {
            GameDataHandler.OnSaveCallback -= OnSave;
            GameDataHandler.OnLoadCallback -= OnLoad;
            GameDataHandler.OnNewGameCallback -= OnNewGame;
        }

        protected virtual void OnSave(){

        }
        protected virtual void OnLoad(){

        }
        protected virtual void OnNewGame(){

        }
    }
}

