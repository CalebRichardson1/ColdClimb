using SaveLoadSystem;
using UnityEngine;

public class SaveSpot : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Save";

    private PlayerData PlayerData => ResourceLoader.MainPlayerData;

    public bool Interact(PlayerInteract player){
        PlayerData.playerLocation.playerPosition = player.transform.position;

        GameDataHandler.SaveGame();
        return true;
    }
}
