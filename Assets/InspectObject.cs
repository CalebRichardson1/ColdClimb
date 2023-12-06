using ColdClimb.Player;
using ColdClimb.UI;
using UnityEngine;

namespace ColdClimb.Interactable{
    public class InspectObject : MonoBehaviour, IInteractable{
        [SerializeField] private Dialogue onInspectDialogue;

        public string InteractionPrompt => "Inspect";

        public bool Interact(PlayerInteract player){
            GlobalUIReference.DialogueController.StartDialogue(onInspectDialogue);
            return true;
        }
    }
}
