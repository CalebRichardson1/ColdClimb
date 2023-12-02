using ColdClimb.Inventory;
using UnityEngine;

namespace ColdClimb.Item{
    [CreateAssetMenu(menuName = "Item/New Key Item", fileName = "NewKeyItem"), System.Serializable]
    public class KeyItem : ItemData
    {
        public bool IsItemCombinable => isCombinable;

        //key item data
        [Header("Key Item Settings")]
        [SerializeField] private bool isCombinable;

        //item base methods
        public override void CombineItem()
        {
            
        }

        public override void DiscardItem()
        {
            
        }

        public override void UseItem(InventoryContextMenu contextMenu){
            //fire a static event from the inventory context menu when a key item is used
            InventoryContextMenu.OnUseKeyItemAction?.Invoke(this);
        }
    }
}
