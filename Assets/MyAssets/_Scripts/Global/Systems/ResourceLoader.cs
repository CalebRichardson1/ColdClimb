using ColdClimb.Global.SaveSystem;
using ColdClimb.Inventory;
using UnityEngine;

namespace ColdClimb.Global{
    //A class that loads SOs from the resouce folder
    public class ResourceLoader{
        public static PlayerInventory PlayerInventory => Resources.Load<PlayerInventory>("Player Data/PlayerInventory");
        public static PlayerData MainPlayerData => Resources.Load<PlayerData>("Player Data/PlayerDataHolder");
        public static ScenesData ScenesData => Resources.Load<ScenesData>("Game Data/ScenesDataHolder");

        public static InputManager InputManager => Resources.Load<InputManager>("Systems/InputManagerScriptableObject");
        public static Logger GlobalLogger => Resources.Load<Logger>("Systems/LoggerScriptableObject");
    }
}
