using ColdClimb.Audio;
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

        [Header("Flavor Text")]
        [SerializeField] private Dialogue onUnlockDialogue;
        [SerializeField] private Dialogue onFailedUnlockDialogue;

        [Header("Audio Optiopns")]
        [SerializeField] private AudioType onUnlockAudio;
        [SerializeField] private AudioType onFailedUnlockAudio;

        private AudioSource AudioSource => GetComponent<AudioSource>();
        private bool subscribed;

        public void AttemptUnlock(ItemData itemAttempt){
            if(itemAttempt == itemToUnlock){
                //TODO: Add unlock text to text box
                if(onUnlockDialogue.sentences.Length != 0){
                    GlobalUIReference.DialogueController.StartDialogue(onUnlockDialogue);
                }

                if(consumeItem) InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(1);
                OnUnlockEvent?.Invoke();
                if(onUnlockAudio != AudioType.None){
                    AudioController.instance.PlayAudio(onUnlockAudio, false, 0, 0, AudioSource);
                }

                // After triggering the event turn off this component.
                enabled = false;
            }
            else{
                //TODO: Add failed text to text box
                if(onFailedUnlockDialogue.sentences.Length != 0){
                    GlobalUIReference.DialogueController.StartDialogue(onFailedUnlockDialogue);
                }
                if(onUnlockAudio != AudioType.None){
                    AudioController.instance.PlayAudio(onFailedUnlockAudio, false, 0, 0, AudioSource);
                }
            }
        }

        private void OnTriggerEnter(Collider other){
            if(!other.CompareTag("Player")) return;

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
