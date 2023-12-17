using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using ColdClimb.UI;
using UnityEngine;

namespace ColdClimb.Generic{
    public class DialogueTrigger : MonoBehaviour, ILoadable{
        public string id;
        public bool activatable = false;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }

        [SerializeField] private Dialogue onTriggerDialogue;
        
        private ScenesData ScenesData => ResourceLoader.ScenesData;
        private bool triggered;

        private void Awake() {
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;
        }

        private void Start() {
            if(activatable){
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }

        private void OnDestroy(){
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }

        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("Player") || GameManager.CurrentState != GameState.MainGame) return;

            GlobalUIReference.DialogueController.StartDialogue(onTriggerDialogue);
            if(!onTriggerDialogue.isQuestion){
                gameObject.GetComponent<Collider>().enabled = false;
                triggered = true;
            }
        }

        public void EndGame(){
            GameManager.UpdateGameState(GameState.MainMenu);
        }

        public void ReadText(){
            GlobalUIReference.DialogueController.StartDialogue(onTriggerDialogue);
        }

        public void CancelInteraction(){
            GlobalUIReference.DialogueController.EndDialogue();
        }

        public void SaveState(){
            if(ScenesData.CurrentSceneData.DialogueTriggersActivated.ContainsKey(id)){
                ScenesData.CurrentSceneData.DialogueTriggersActivated.Remove(id);
            }
            ScenesData.CurrentSceneData.DialogueTriggersActivated.Add(id, triggered);
        }

        public void LoadData(){
            ScenesData.CurrentSceneData.DialogueTriggersActivated.TryGetValue(id, out triggered);
            if(triggered){
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
