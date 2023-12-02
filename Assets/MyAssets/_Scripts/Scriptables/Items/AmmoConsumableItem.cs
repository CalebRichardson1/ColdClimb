using ColdClimb.Inventory;
using UnityEngine;

namespace ColdClimb.Item{
    [CreateAssetMenu(menuName = "Item/New Ammo Consumable", fileName = "NewAmmoConsumable"), System.Serializable]
    public class AmmoConsumableItem : ItemData
    {
        public GunType AmmoGunType => ammoGunType;

        [Header("Gun Type")]
        [SerializeField] private GunType ammoGunType;

        public override void CombineItem(){
            
        }

        public override void DiscardItem(){
            InventoryContextMenu.ContextedInventoryItem.DiscardItem();
        }

        public override void UseItem(InventoryContextMenu contextMenu){
            //search the inventory for a gun with the same gun type of the ammo
            var selectedGun = contextMenu.Inventory.GetGunByType(ammoGunType);
            
            if(selectedGun == null) return;

            //if gun type is this ammo's type and the gun stats current ammo isn't equal to the gun stats max ammo
            if(selectedGun.GunStats.currentAmmo == selectedGun.GunStats.maxAmmo) return;

                //then see how must ammo the gun is missing (maxAmmo - current Ammo) 
                var ammoMissing = selectedGun.GunStats.maxAmmo - selectedGun.GunStats.currentAmmo;
                //see if this inventory item has enough stacks to fill it
                if(ammoMissing <= InventoryContextMenu.ContextedInventoryItem.CurrentStackSize){
                    selectedGun.GunStats.currentAmmo += ammoMissing;
                    InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(ammoMissing);
                    return;
                }
                //if their isn't enough stacks then add to the current ammo with the remaining stacks and delete this item from the inventory
                selectedGun.GunStats.currentAmmo += InventoryContextMenu.ContextedInventoryItem.CurrentStackSize;
                InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(InventoryContextMenu.ContextedInventoryItem.CurrentStackSize);
        }
    }
}
