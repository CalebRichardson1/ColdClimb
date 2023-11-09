using System;
using UnityEngine;

namespace SaveLoadSystem
{
    [CreateAssetMenu(menuName = "Save Data Holders/Game Data Holder", fileName = "NewGameDataHolder")] 
    public class GameData : DataHolder
    {
        public override event Action LoadValuesCallback;
        public event Action LoadValidFilesCallback;

        public ContinueGame continueGame = new ContinueGame();

        private void OnEnable() {
            GameDataHandler.OnLoadValidContinueGame += () => LoadValuesCallback?.Invoke();
            GameDataHandler.OnLoadValidSaveDataCallback += () => LoadValidFilesCallback?.Invoke();
        }

        private void OnDisable() {
            GameDataHandler.OnLoadValidContinueGame -= () => LoadValuesCallback?.Invoke();
            GameDataHandler.OnLoadValidSaveDataCallback -= () => LoadValidFilesCallback?.Invoke();
        }

        public void LoadContinueGame(){
            GameDataHandler.LoadContinueGame();
        }
    }
}

