using System;
using UnityEngine;

public class ItemLock : MonoBehaviour
{
    public Action OnUnlockEvent;
    [SerializeField] private string interactPrompt;
    [SerializeField] private KeyItem itemToUnlock;
    [SerializeField] private bool consumeItem;

    public string InteractionPrompt => interactPrompt;

    private bool subscribed;

    public void AttemptUnlock(ItemData itemAttempt){
        if(itemAttempt == itemToUnlock){
            print("Unlocked!");
            if(consumeItem) InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(1);
            OnUnlockEvent?.Invoke();
        }
        else{
            print("Wrong Item!");
        }
    }

    private void OnTriggerEnter(Collider other) {
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
