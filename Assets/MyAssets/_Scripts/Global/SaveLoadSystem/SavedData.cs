using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem
{
/// <summary>
/// A class that holds structs to our various data that we want to save.
/// </summary>

    [System.Serializable]
    public class SaveData
    {
        public PlayerStats CurrentPlayerStats = new PlayerStats();
        public PlayerLocation CurrentPlayerLocation = new PlayerLocation();
        public PlayerInventory CurrentPlayerInventory = new PlayerInventory();
    }

    [System.Serializable]
    // All the variables that directly affect the player character
    public struct PlayerStats
    {
        public int currentHealth;
        public int maxHealth;
        public float walkSpeed;
        public float runSpeed;
        public float lookSpeed;
    }

    [System.Serializable]
    // The location where our player is located. Including the currentSceneIndex for level loading.
    public struct PlayerLocation
    {
        public SceneIndex currentSceneIndex;
        public Vector3 playerPosition;
        public float playerLookX;
        public float playerLookY;
    }

    [System.Serializable]
    // The Inventory used for our player.
    public struct PlayerInventory
    {
        public InventoryItem currentEquippedItem;
        public List<InventoryItem> inventoryItems;
        public int maxInventory;
    }

    //level states (item states, puzzle states, enemy states, etc.)
}

