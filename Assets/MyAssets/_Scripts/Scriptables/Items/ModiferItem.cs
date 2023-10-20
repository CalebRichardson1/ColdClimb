using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Modifer Item", fileName = "NewModifer"), System.Serializable]
public class ModiferItem : ItemData
{
    //modifer item data

    //item base methods
    public override void CombineItem()
    {
        
    }

    public override void DiscardItem()
    {
        
    }

    public override void UseItem(InventoryContextMenu contextMenu){
        Debug.Log("Modifying...");
    }
}
