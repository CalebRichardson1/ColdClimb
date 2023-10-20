using System;
using System.IO;
using UnityEngine;

namespace SaveLoadSystem
{
    /// <summary>
    /// Handles creating external files and writing/reading those files.
    /// </summary>
    public static class GameDataHandler
    {
        // OnSaveInjection is used to update our DataHolders values from various scripts.
        public static event Action OnSaveInjectionCallback; 
        // OnSaveAction applies the changes from DataHolders to the CurrentSaveData.
        public static event Action OnSaveCallback;
        // OnLoadAction notifies our DataHolders to extract the information from the CurrentSaveData once it's loaded.
        public static event Action OnLoadCallback;
        // OnNewGameCallback sets whatever our default values set in the inspector as the values we use.
        public static event Action OnNewGameCallback;
    
        // The current save data, if no save data is found when loading, then we use the default save data
        public static SaveData CurrentSaveData = new SaveData();

        public const string SAVEDIRECTORY = "/SavedGameData/";
        public const string FILENAME = "SavedGameData.sav";

        // Defines a path that we write a JSON file to, and writes our CurrentSaveData to that file.
        public static bool SaveGame(){
            OnSaveInjectionCallback?.Invoke();
            OnSaveCallback?.Invoke();

            string dir = Application.dataPath + SAVEDIRECTORY;
            if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string json = JsonUtility.ToJson(CurrentSaveData, true);
            File.WriteAllText(dir + FILENAME, json);
            return true;
        }

        // Loads the SaveGameData.sav file from the directory, and if found, reads the file and applys it to our CurrentSaveData
        public static void LoadGame(){
            string fullPath = Application.dataPath + SAVEDIRECTORY + FILENAME;
            if(!File.Exists(fullPath)){
                ResourceLoader.GlobalLogger.Log("No save file found...");
                NewGame();
                return;
            }

            string json = File.ReadAllText(fullPath);

            SaveData tempData = JsonUtility.FromJson<SaveData>(json);
            CurrentSaveData = tempData;
            ResourceLoader.GlobalLogger.Log("Loaded File Successfully!");
            
            OnLoadCallback?.Invoke();
        }

        public static void NewGame(){
            OnNewGameCallback?.Invoke();
        }
    }
}

