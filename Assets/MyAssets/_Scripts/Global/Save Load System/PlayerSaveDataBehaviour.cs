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

        private void Awake(){
            // GameDataHandler.OnSaveInjectionCallback += SavePlayerLocation;
            MainPlayerData.LoadValuesCallback += LoadPlayerLocation;
            GameManager.OnGameStateChange += EvaluateGameState;
        }

        private void OnDisable(){
            // GameDataHandler.OnSaveInjectionCallback -= SavePlayerLocation;
            MainPlayerData.LoadValuesCallback -= LoadPlayerLocation;
            GameManager.OnGameStateChange -= EvaluateGameState;
        }

        private void SavePlayerLocation(){
            MainPlayerData.playerLocation.playerLookX = PlayerCamController.XValue;
            MainPlayerData.playerLocation.playerLookY = PlayerCamController.YValue;
        }

        private void LoadPlayerLocation(){
            Vector3 pos = MainPlayerData.playerLocation.playerPosition;
            transform.position = pos;

            StartCoroutine(MovementTimer());
        }

        private IEnumerator MovementTimer(){
            PlayerMovement.enabled = false;
            PlayerRB.isKinematic = true;

            yield return new WaitForSeconds(0.05f);

            PlayerMovement.enabled = true;
            PlayerRB.isKinematic = false;
        }

        private void EvaluateGameState(GameState state){
            if(state == GameState.GameOver){
                //Turn off the player collider
                gameObject.TryGetComponent(out Collider collider);
                if(collider != null){
                    collider.enabled = false;
                }
            }
        }
    }
}
