using UnityEngine;

namespace SaveLoadSystem{
    public class PlayerSaveDataBehaviour : MonoBehaviour{
        private FirstPersonCameraController PlayerCamController => GetComponent<FirstPersonCameraController>();

        private PlayerData MainPlayerData => ResourceLoader.MainPlayerData;

        private void OnEnable() {
            GameDataHandler.OnSaveInjectionCallback += SavePlayerLocation;
            MainPlayerData.LoadValuesCallback += LoadPlayerLocation;
        }

        private void OnDisable() {
            GameDataHandler.OnSaveInjectionCallback -= SavePlayerLocation;
            MainPlayerData.LoadValuesCallback -= LoadPlayerLocation;
        }

        private void SavePlayerLocation(){
            MainPlayerData.playerLocation.playerPosition = transform.position;
            MainPlayerData.playerLocation.playerLookX = PlayerCamController.XValue;
            MainPlayerData.playerLocation.playerLookY = PlayerCamController.YValue;
        }

        private void LoadPlayerLocation(){
            // transform.position = MainPlayerData.playerLocation.playerPosition;
        }
    }
}
