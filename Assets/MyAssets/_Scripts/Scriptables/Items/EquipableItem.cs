using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Equipable Item", fileName = "NewEquipableItem"), System.Serializable]
public class EquipableItem : ItemData
{
    //equipment data
    [Header("Equipment Data")]
    [SerializeField] private EquippedItemBehavior itemBehavior;

    public EquippedItemBehavior ItemBehavior => itemBehavior;

    //base item methods
    public override void CombineItem(){
        
    }

    public override void DiscardItem(){
        
    }

    public override void UseItem(InventoryContextMenu contextMenu){
        //hold contexted item
        var contextedItemData = InventoryContextMenu.ContextedInventoryItem.ItemData;
        var contextedInventoryItem = InventoryContextMenu.ContextedInventoryItem;
        var playerInventory = ResourceLoader.PlayerInventory;

        if(playerInventory.CurrentEquippedItem.ItemData == null){
            playerInventory.EquipItem(contextedItemData, contextedInventoryItem.CurrentStackSize);
            InventoryContextMenu.ContextedInventoryItem.SetItem(null, 0);
            InventoryUIController.CurrentEquippedItemSlot.DrawSlotVisual();
            return;
        }

        //remove the contexted item from the inventory
        contextedInventoryItem.RemoveFromStack(1);

        //add the current equipped item to the inventory
        if(playerInventory.AttemptToAddItemToInventory(playerInventory.CurrentEquippedItem.ItemData, playerInventory.CurrentEquippedItem.CurrentStackSize) == 0){
            InventoryUIController.CurrentEquippedItemSlot.ItemInSlot.RemoveFromStack(1);
            
            //finally add the held contexted item to the equipment slot
            playerInventory.EquipItem(contextedItemData, contextedInventoryItem.CurrentStackSize);

            InventoryUIController.CurrentEquippedItemSlot.DrawSlotVisual();
            contextMenu.Controller.DrawInventorySlotsVisuals();
            return;
        }

        //if it can't be added add the contexted item back into the inventory
        else{
            playerInventory.AttemptToAddItemToInventory(contextedItemData, contextedInventoryItem.CurrentStackSize);
            Debug.Log("Can't add the item! Adding the contexted item back into the inventory...");
        }
    }
}
