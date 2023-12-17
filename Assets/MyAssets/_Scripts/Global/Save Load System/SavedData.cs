using System.Collections.Generic;
using ColdClimb.Inventory;
using UnityEngine;

namespace ColdClimb.Global.SaveSystem{
/// <summary>
/// A class that holds structs to our various data that we want to save.
/// </summary>

    [System.Serializable]
    public class SaveData{
        public PlayerStats CurrentPlayerStats = new PlayerStats();
        public PlayerLocation CurrentPlayerLocation = new PlayerLocation();
        public PlayerInventoryData CurrentPlayerInventory = new PlayerInventoryData();
        public GameScenesData CurrentScenesData = new GameScenesData();       
    }

    [System.Serializable]
    // All the variables that directly affect the player character
    public struct PlayerStats{
        public int currentHealth;
        public int maxHealth;
        public float walkSpeed;
        public float runSpeed;
        public float lookSpeed;
    }

    [System.Serializable]
    // The location where our player is located. Including the currentSceneIndex for level loading.
    public struct PlayerLocation{
        public SceneIndex currentSceneIndex;
        public Vector3 playerPosition;
        public float playerLookX;
        public float playerLookY;
    }

    [System.Serializable]
    // The Inventory used for our player.
    public struct PlayerInventoryData{
        public InventoryItem currentEquippedItem;
        public List<InventoryItem> inventoryItems;
        public int maxInventory;
    }

    [System.Serializable]
    public class GameScenesData{
        public List<SceneData> SceneDatasModified;
    }

   [System.Serializable]
   public class SceneData{
        public int associatedSceneIndex;
        public SerializableDictionary<string, bool> DialogueTriggersActivated;
        public SerializableDictionary<string, bool> InspectObjectsUpdated;
        public SerializableDictionary<string, bool> ItemsPickuped;
        public SerializableDictionary<string, bool> ItemLocksUnlocked;
        public SerializableDictionary<string, bool> DoorsUnlocked;
        public SerializableDictionary<string, bool> DoorsOpened;
        public SerializableDictionary<string, bool> EnemiesDied;
        public SerializableDictionary<string, bool> ReadNotes;
        public SerializableDictionary<string, bool> KeyPadsUnlocked;
        public SerializableDictionary<string, bool> CutscenesActivated;
   }
}

