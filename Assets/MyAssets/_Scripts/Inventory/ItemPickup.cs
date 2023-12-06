using ColdClimb.Global;
using ColdClimb.Interactable;
using ColdClimb.Item;
using ColdClimb.Player;
using ColdClimb.UI;
using UnityEngine;
using Logger = ColdClimb.Global.Logger;

namespace ColdClimb.Inventory{
    public class ItemPickup : MonoBehaviour, IInteractable{
        [Header("Item Data")]
        [SerializeField] private ItemData itemData;
        [SerializeField] private int itemStackAmount;

        [Header("Pickup Flavor")]
        [SerializeField] private Dialogue interactQuestionDialogue;
        [SerializeField] private Dialogue successfulGrabbedDialogue;
        [SerializeField] private Dialogue failedGrabDialogue;

        private Logger GlobalLogger => ResourceLoader.GlobalLogger;

        public string InteractionPrompt => new("Pickup " + itemData.Name);

        private void Start(){
            if(itemStackAmount > itemData.MaxStackSize){
                itemStackAmount = itemData.MaxStackSize;
                GlobalLogger.Log("Item pickup stack size is to big for the max item stack size for the item data, setting it the the max stack size...");
            }
            if(itemStackAmount == 0) itemStackAmount = 1;
        }

        public void PickUpItem(){
            //Returns a int of 0 if item was successfully added to inventory
            //else change the stack amount of this pickup item
            var itemRemainder = ResourceLoader.PlayerInventory.AttemptToAddItemToInventory(itemData, itemStackAmount);
            if(itemRemainder == 0){
                Destroy(gameObject, 0.1f);
                if(successfulGrabbedDialogue.sentences.Length > 0){
                    GlobalUIReference.DialogueController.StartDialogue(successfulGrabbedDialogue);
                    return;
                }
            }  
            else{
                itemStackAmount = itemRemainder;
                if(failedGrabDialogue.sentences.Length > 0){
                    GlobalUIReference.DialogueController.StartDialogue(failedGrabDialogue);
                    return;
                }
            } 

            if(interactQuestionDialogue.sentences.Length > 0){
                GlobalUIReference.DialogueController.EndDialogue();
            }
        }

        public void CancelPickUp(){
            GlobalUIReference.DialogueController.EndDialogue();
        }

        public bool Interact(PlayerInteract player){
            if(itemData == null){
                GlobalLogger.Log("No Item Data Assigned...", this);
                return false;
            }

            if(interactQuestionDialogue.sentences.Length > 0){
                GlobalUIReference.DialogueController.StartDialogue(interactQuestionDialogue);
                return false;
            }

            PickUpItem();
            return true;
        } 
    }
}
