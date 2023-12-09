using ColdClimb.Interactable;
using ColdClimb.Player;
using UnityEngine;

namespace ColdClimb.Global.SaveSystem{
    public class SaveSpot : MonoBehaviour{
        private PlayerData PlayerData => ResourceLoader.MainPlayerData;

        public void Interact(PlayerInteract player){
            PlayerData.playerLocation.playerPosition = player.transform.position;

            GameDataHandler.SaveGame();
        }
    }
}
