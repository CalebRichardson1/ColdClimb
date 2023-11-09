using System;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data based on the strucs defined. PlayerData tracks all the data relating to our player.
/// </summary>

namespace SaveLoadSystem{
    [CreateAssetMenu(menuName = "Save Data Holders/Player Data Holder", fileName = "NewPlayerDataHolder")] 
    public class PlayerData : DataHolder
    {
        // Stores reference to all saved structs relating to our player.
        public PlayerStats playerStats;
        public PlayerLocation playerLocation;
        public PlayerInventory playerInventory;

        public override event Action LoadValuesCallback;

        // During the OnSave event, we save the structs we have onto the current save data.
        protected override void OnSave(){
            GameDataHandler.CurrentSaveData.CurrentPlayerStats = playerStats;
            GameDataHandler.CurrentSaveData.CurrentPlayerLocation = playerLocation;
            GameDataHandler.CurrentSaveData.CurrentPlayerInventory = playerInventory;
        }

        // During the OnLoad event, we take the structs we need from the current saved data.
        protected override void OnLoad(){
            playerStats = GameDataHandler.CurrentSaveData.CurrentPlayerStats;
            playerLocation = GameDataHandler.CurrentSaveData.CurrentPlayerLocation;
            playerInventory = GameDataHandler.CurrentSaveData.CurrentPlayerInventory;

            LoadValuesCallback?.Invoke();
            
            //TMP
            GameManager.UpdateGameState(GameState.MainGame);
        }

        // Load what values we have set in the inspector as default and call load values callback to set them.
        protected override void OnNewGame(){
            //move to a default value file or something similar for easy editing for different difficulties/settings
            playerStats.currentHealth = 100;
            playerStats.maxHealth = 100;
            playerStats.walkSpeed = 4f;
            playerStats.runSpeed = 7f;
            playerStats.lookSpeed = 30f;
            playerLocation.currentSceneIndex = SceneIndex.PARKING_LOT;
            playerLocation.playerPosition = new Vector3(85.18f, 3.06f, 132.12f);
            playerLocation.playerLookX = 0;
            playerLocation.playerLookY = 0;
            
            playerInventory.inventoryItems.Clear();
            playerInventory.currentEquippedItem = null;
            playerInventory.maxInventory = 6;
            LoadValuesCallback?.Invoke();
            
            //TMP
            GameManager.UpdateGameState(GameState.MainGame);
        }
    }
}
