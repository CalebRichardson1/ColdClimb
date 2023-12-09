using ColdClimb.Audio;
using ColdClimb.Generic;
using ColdClimb.Inventory;
using ColdClimb.Item;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.Events;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class ItemLock : MonoBehaviour{
        [Header("Unlock Options")]
        [SerializeField] private UnityEvent OnUnlockEvent;
        [SerializeField] private KeyItem itemToUnlock;
        [SerializeField] private bool consumeItem;

        [Header("Item Cutscene")]
        [SerializeField] private Cutscene onSuccessfulUnlockCutscene;

        [Header("Failed Text")]
        [SerializeField] private Dialogue onFailedUnlockDialogue;

        private bool subscribed;
        private bool unlocked = false;

        public void AttemptUnlock(ItemData itemAttempt){
            if(unlocked) return;

            if(itemAttempt == itemToUnlock){
                onSuccessfulUnlockCutscene.StartCutscene();
                if(onSuccessfulUnlockCutscene == null){
                    Unlock();
                }
            }
            else{
                //TODO: Add failed text to text box
                if(onFailedUnlockDialogue.sentences.Length != 0){
                    GlobalUIReference.DialogueController.StartDialogue(onFailedUnlockDialogue);
                }
            }
        }

        public void Unlock(){
             if(consumeItem) InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(1);
                OnUnlockEvent?.Invoke();

                // After triggering the event set unlocked to true so it doesn't subscribe or use any items.
                unlocked = true;
        }

        private void OnTriggerEnter(Collider other){
            if(!other.CompareTag("Player") || unlocked) return;

            InventoryContextMenu.OnUseKeyItemAction += AttemptUnlock; 
            subscribed = true;
        }

        private void OnTriggerExit(Collider other){
            if(!other.CompareTag("Player")) return;
            if(subscribed){
                InventoryContextMenu.OnUseKeyItemAction -= AttemptUnlock;
                subscribed = false; 
            }
        }
    }
}
