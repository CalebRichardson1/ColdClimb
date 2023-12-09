using ColdClimb.Player;
using ColdClimb.UI;
using UnityEngine;

namespace ColdClimb.Interactable{
    public class InspectObject : MonoBehaviour, IInteractable{
        public bool NeedsLight => needsLightToInspect;

        [SerializeField] private Dialogue onInspectDialogue;
        [SerializeField] private Dialogue tooDarkDialogue;        
        [SerializeField] private bool needsLightToInspect = false;
        private bool inspectable = true;

        public string InteractionPrompt => "Inspect";

        private void Start() {
            if(needsLightToInspect){
                inspectable = false;
            }
        }

        public bool Interact(PlayerInteract player){
            if(needsLightToInspect && !inspectable){
                GlobalUIReference.DialogueController.StartDialogue(tooDarkDialogue);
                return false;
            }

            if(!inspectable) return false;
            GlobalUIReference.DialogueController.StartDialogue(onInspectDialogue);
            return true;
        }

        public void SetInspectable(bool state){
            inspectable = state;
        }

        public void CancelOption(){
            GlobalUIReference.DialogueController.EndDialogue();
        }
    }
}
