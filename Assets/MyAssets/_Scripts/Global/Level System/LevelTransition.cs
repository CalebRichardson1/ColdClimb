using ColdClimb.Global.SaveSystem;
using ColdClimb.UI;
using UnityEngine;

namespace ColdClimb.Global.LevelSystem{

    public class LevelTransition : MonoBehaviour{
        [SerializeField] private SceneIndex levelToLoad;

        [Header("Player")]
        [SerializeField] private Vector3 playerPosAfterLoad;
        [SerializeField] private float playerLookXAfterLoad;
        [SerializeField] private float playerLookYAfterLoad;

        [Header("Dialogue")]
        [SerializeField] private Dialogue transitionDialogue;

        private PlayerData PlayerData => ResourceLoader.MainPlayerData;

        private void OnTriggerEnter(Collider other){
            if(!other.CompareTag("Player")) return;

            if(transitionDialogue.sentences.Length != 0){
                GlobalUIReference.DialogueController.StartDialogue(transitionDialogue);
                return;
            }

            LoadLevel();            
    }

    public void LoadLevel(){
            if(transitionDialogue.sentences.Length != 0){
                GlobalUIReference.DialogueController.EndDialogue();
            }

        // Update the player
            PlayerData.playerLocation.currentSceneIndex = levelToLoad;
            PlayerData.playerLocation.playerPosition = playerPosAfterLoad;
            PlayerData.playerLocation.playerLookX = playerLookXAfterLoad;
            PlayerData.playerLocation.playerLookY = playerLookYAfterLoad;
            
            // Temp Save the data
            GameDataHandler.SaveTempData();
            
            
            // Load the temp data after we transition
            SceneDirector.Instance.LoadScene(levelToLoad, GameDataHandler.ReturnOnLoadAction());
        }

        public void CancelInteraction(){
            GlobalUIReference.DialogueController.EndDialogue();
        }
    }
}
