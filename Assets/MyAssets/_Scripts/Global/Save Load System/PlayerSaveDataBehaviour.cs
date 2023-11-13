using System.Collections;
using ColdClimb.Camera;
using ColdClimb.Player;
using UnityEngine;

namespace ColdClimb.Global.SaveSystem{
    public class PlayerSaveDataBehaviour : MonoBehaviour{
        private FirstPersonCameraController PlayerCamController => GetComponent<FirstPersonCameraController>();

        private PlayerMovement PlayerMovement => GetComponent<PlayerMovement>();
        private Rigidbody PlayerRB => GetComponent<Rigidbody>();

        private PlayerData MainPlayerData => ResourceLoader.MainPlayerData;

        private void OnEnable(){
            GameDataHandler.OnSaveInjectionCallback += SavePlayerLocation;
            MainPlayerData.LoadValuesCallback += LoadPlayerLocation;
        }

        private void OnDisable(){
            GameDataHandler.OnSaveInjectionCallback -= SavePlayerLocation;
            MainPlayerData.LoadValuesCallback -= LoadPlayerLocation;
        }

        private void SavePlayerLocation(){
            MainPlayerData.playerLocation.playerLookX = PlayerCamController.XValue;
            MainPlayerData.playerLocation.playerLookY = PlayerCamController.YValue;
        }

        private void LoadPlayerLocation(){
            PlayerMovement.enabled = false;
            PlayerRB.isKinematic = true;

            Vector3 pos = MainPlayerData.playerLocation.playerPosition;
            transform.position = pos;

            StartCoroutine(MovementTimer());
        }

        private IEnumerator MovementTimer(){
            yield return new WaitForSeconds(0.05f);

            PlayerMovement.enabled = true;
            PlayerRB.isKinematic = false;
        }
    }
}
