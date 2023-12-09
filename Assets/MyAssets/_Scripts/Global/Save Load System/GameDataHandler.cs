using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ColdClimb.Global.SaveSystem{
    /// <summary>
    /// Handles creating external files and writing/reading those files.
    /// </summary>
    public static class GameDataHandler{
        // OnSaveInjection is used to update our DataHolders values from various scripts.
        public static event Action OnSaveInjectionCallback; 
        // OnSaveAction applies the changes from DataHolders to the CurrentSaveData.
        public static event Action OnSaveCallback;
        // OnSaveOptionsCallback applies the changes from the Option data holder to the CurrentOptions.
        public static event Action OnSaveOptionsCallback;
        // The intital event that is used to load the game scene then load all our data.
        public static event Action<SceneIndex, Action> LoadGameScene;
        // OnLoadAction notifies our DataHolders to extract the information from the CurrentSaveData once it's loaded.
        public static event Action OnLoadCallback;
        // OnLoadOptions notifies any script listening and changing based on the options
        public static event Action OnLoadOptionsCallback;
        // OnLoadValidSaveData notifies any script that is listening to display loadable game data
        public static event Action OnLoadValidSaveDataCallback;
        // OnLoadValidContinueGame notifies that there is a valid continue game.
        public static event Action OnLoadValidContinueGame;
        // OnNewGameCallback sets whatever our default values set in the inspector as the values we use.
        public static event Action OnNewGameCallback;
        // OnNewOptionsCallback sets the default settings in the main menu.
        public static event Action OnNewOptionsCallback;
        // The current save data, if no save data is found when loading, then we use the default save data.
        public static SaveData CurrentSaveData = new SaveData();
        // The current saved options, if no save data is found when starting the game, then set the values to default settings.
        public static SavedOptions CurrentOptions = new SavedOptions();
        // The current saved game data, if no save data is found we don't show the continue game button on the main menu.
        public static SavedGameData CurrentGameData = new SavedGameData();
        // The current slot to save/read data from.
        public static SaveSlot CurrentSaveSlot;

        public const string SAVEDIRECTORY = "/ColdClimbSavedData/";

        public const string SLOT1DIRECTORY = "/SLOT1/";
        public const string SLOT2DIRECTORY = "/SLOT2/";
        public const string SLOT3DIRECTORY = "/SLOT3/";

        // File for read/write game settings.
        public const string SETTINGS = "GameOptions.gear";
        // File for read/write for Continue Game.
        public const string SAVEGAME = "SaveGame.gear";

        private static string GameDirectory => Application.dataPath + SAVEDIRECTORY;

        public static void InitializeFolderStructure(){
            Directory.CreateDirectory(GameDirectory);

            var slot1 = GameDirectory + SLOT1DIRECTORY;
            var slot2 = GameDirectory + SLOT2DIRECTORY;
            var slot3 = GameDirectory + SLOT3DIRECTORY;

            Directory.CreateDirectory(slot1);
            Directory.CreateDirectory(slot2);
            Directory.CreateDirectory(slot3);
        }

        public static void IntializeSaveValues(){
            LoadSettings();
            LoadValidSaveData();
            LoadValidContinueGame();
        }

        public static void SetCurrentSaveSlot(SaveSlot slot){
            if(CurrentSaveSlot == slot) return;
            CurrentSaveSlot = slot;
        }

        public static void SaveTempData(){
            OnSaveCallback?.Invoke();
        } 

        // Defines a path that we write a JSON file to, and writes our CurrentSaveData to that file.
        // Override a save file.
        public static bool SaveGame(string FileName){
            OnSaveInjectionCallback?.Invoke();
            SaveTempData();

            if(!Directory.Exists(GameDirectory)){
              InitializeFolderStructure();
              // No save game exists in the directory so return.
              return false;
            } 

            string json = JsonUtility.ToJson(CurrentSaveData, true);
            string slotDirectory = GameDirectory;
            switch (CurrentSaveSlot){
                case SaveSlot.SLOT1: slotDirectory = slotDirectory + SLOT1DIRECTORY + FileName;
                    break;
                case SaveSlot.SLOT2: slotDirectory = slotDirectory + SLOT2DIRECTORY + FileName;
                    break;
                case SaveSlot.SLOT3: slotDirectory = slotDirectory + SLOT3DIRECTORY + FileName;
                    break;
            }
            File.WriteAllText(slotDirectory, json);
            // Update file name.
            string UpdatedFileName = DateTime.Now.ToString("MM_dd_hh_mm_sstt") + ".sav";
            File.Move(slotDirectory, UpdatedFileName);
            SaveData(UpdatedFileName);
            return true;
        }

        // Creating a new save file.
        public static bool SaveGame(){ 
            OnSaveInjectionCallback?.Invoke();
            OnSaveCallback?.Invoke();

            if(!Directory.Exists(GameDirectory)) InitializeFolderStructure();

            string slotDirectory = GameDirectory + GetSlotDirectory(CurrentSaveSlot);

            string json = JsonUtility.ToJson(CurrentSaveData, true);

            string finalFileName = DateTime.Now.ToString("MM_dd_hh_mm_sstt") + ".sav";
            File.WriteAllText(slotDirectory + finalFileName, json);
            SaveData(finalFileName);
            return true;
        }

        public static Action ReturnOnLoadAction(){
            return OnLoadCallback;
        }

        // Loads the SaveGameData.sav file from the directory, and if found, reads the file and applys it to our CurrentSaveData
        public static void LoadGame(string FileName){

            string savedGameFullPath = GameDirectory + GetSlotDirectory(CurrentSaveSlot) + FileName;

            if(!File.Exists(savedGameFullPath)){
                ResourceLoader.GlobalLogger.Log("No save file found...");
                return;
            }

            string json = File.ReadAllText(savedGameFullPath);

            SaveData tempData = JsonUtility.FromJson<SaveData>(json);
            CurrentSaveData = tempData;
            ResourceLoader.GlobalLogger.Log("Loaded File Successfully!");
            
            LoadGameScene?.Invoke(CurrentSaveData.CurrentPlayerLocation.currentSceneIndex, OnLoadCallback);
        }

        public static void LoadContinueGame(){
            //search if we have a SaveGame.sav file to load to the continue game 
            if(!Directory.Exists(GameDirectory)){
                InitializeFolderStructure();
                return;
            } 

            if(!File.Exists(GameDirectory + SAVEGAME)){
                return;
            }

            //TO-DO: Add valid file detection

            string json = File.ReadAllText(GameDirectory + SAVEGAME);

            SavedGameData tempData = JsonUtility.FromJson<SavedGameData>(json);
            CurrentGameData = tempData;

            // Set the slot and load the file.
            SetCurrentSaveSlot(CurrentGameData.CurrentContinueGame.previousSlot);
            LoadGame(CurrentGameData.CurrentContinueGame.previousSaveFileName);
        }

        public static void LoadValidSaveData(){
            if(!Directory.Exists(GameDirectory)){
                InitializeFolderStructure();
                return;
            } 
            // Go through each slot and if a valid save data exists trigger the OnLoadValidSaveData event.
            string[] validSaveFiles = Directory.GetFiles(GameDirectory, "*.sav", SearchOption.AllDirectories);

            if(validSaveFiles.Length > 0){
                OnLoadValidSaveDataCallback?.Invoke();
            }
        }

        private static void LoadValidContinueGame(){
            if(!Directory.Exists(GameDirectory)){
                InitializeFolderStructure();
                return;
            } 

            if(!File.Exists(GameDirectory + SAVEGAME)){
                return; // No valid file found.
            } 

            OnLoadValidContinueGame?.Invoke();
        }

        public static void NewGame(SaveSlot slot){
            SetCurrentSaveSlot(slot);
            // Remove any save data in that slot.
            DeleteSaveData(slot);

            // Hard coded starting scene for now.
            LoadGameScene?.Invoke(SceneIndex.FOREST, OnNewGameCallback);
        }

        public static void TriggerNewGameValues() => OnNewGameCallback?.Invoke();

        public static void SaveSettings(){
            OnSaveOptionsCallback?.Invoke();
            if(!Directory.Exists(GameDirectory)) InitializeFolderStructure();
            string json = JsonUtility.ToJson(CurrentOptions, true);
            File.WriteAllText(GameDirectory + SETTINGS, json);
        }

        public static void LoadSettings(){
            if(!Directory.Exists(GameDirectory)) InitializeFolderStructure();

            if(!File.Exists(GameDirectory + SETTINGS)){
                //Create new game settings.
                OnNewOptionsCallback?.Invoke();
                return; // Don't need to load.
            } 
            
            string json = File.ReadAllText(GameDirectory + SETTINGS);

            SavedOptions tempData = JsonUtility.FromJson<SavedOptions>(json);
            CurrentOptions = tempData;

            OnLoadOptionsCallback?.Invoke();
        }

        private static void SaveData(string FileName){
            CurrentGameData.CurrentContinueGame.previousSaveFileName = FileName;
            CurrentGameData.CurrentContinueGame.previousSlot = CurrentSaveSlot;
            if(!Directory.Exists(GameDirectory)) InitializeFolderStructure();
            string json = JsonUtility.ToJson(CurrentGameData, true);
            File.WriteAllText(GameDirectory + SAVEGAME, json);
        }   

        // Used when deleting all save data in a slot directory.
        private static void DeleteSaveData(SaveSlot slot){
            DirectoryInfo directoryInfo = new DirectoryInfo(GameDirectory + GetSlotDirectory(slot));

            FileInfo[] files = directoryInfo.GetFiles("*.sav").Where(f => f.Extension == ".sav").ToArray();

            for (int i = 0; i < files.Length; i++){
                try{
                    files[i].Attributes = FileAttributes.Normal;
                    File.Delete(files[i].FullName);
                }
                catch { }
            }
        }

        // Used when deleting a save data file in a slot directory.
        private static void DeleteSaveData(SaveSlot slot, string fileName){

        }

        private static string GetSlotDirectory(SaveSlot slot){
            return slot switch
            {
                SaveSlot.SLOT1 => SLOT1DIRECTORY,
                SaveSlot.SLOT2 => SLOT2DIRECTORY,
                SaveSlot.SLOT3 => SLOT3DIRECTORY,
                _ => null
            };
        }
    }
}