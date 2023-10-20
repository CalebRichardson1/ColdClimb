using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int itemStackAmount;

    private Logger GlobalLogger => ResourceLoader.GlobalLogger;

    public string InteractionPrompt => new("Pickup " + itemData.Name);

    private void Start() {
        if(itemStackAmount > itemData.MaxStackSize){
            itemStackAmount = itemData.MaxStackSize;
            GlobalLogger.Log("Item pickup stack size is to big for the max item stack size for the item data, setting it the the max stack size...");
        }
        if(itemStackAmount == 0) itemStackAmount = 1;
    }

    public bool Interact(PlayerInteract player){
        if(itemData == null){
            GlobalLogger.Log("No Item Data Assigned...", this);
            return false;
        }

        //Returns a int of 0 if item was successfully added to inventory
        //else change the stack amount of this pickup item
        var itemRemainder = ResourceLoader.PlayerInventory.AttemptToAddItemToInventory(itemData, itemStackAmount);
        if(itemRemainder == 0){
            Destroy(gameObject, 0.1f);
            return true;
        }  
        else{
            itemStackAmount = itemRemainder;
            return false;
        } 
    } 
}
