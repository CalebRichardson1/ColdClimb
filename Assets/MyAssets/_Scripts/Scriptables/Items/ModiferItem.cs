using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Modifer Item", fileName = "NewModifer"), System.Serializable]
public class ModiferItem : ItemData
{
    //modifer item data
    public static Action<bool> EventToTrigger; //change later

    //item base methods
    public override void CombineItem()
    {
        
    }

    public override void DiscardItem()
    {
        
    }

    public override void UseItem(InventoryContextMenu contextMenu){
        EventToTrigger?.Invoke(true);
        InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(1);
    }
}
