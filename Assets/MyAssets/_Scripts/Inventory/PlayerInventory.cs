using System;
using System.Collections.Generic;
using System.Linq;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Item;
using ColdClimb.Item.Equipped;
using UnityEngine;
using Logger = ColdClimb.Global.Logger;

namespace ColdClimb.Inventory{
    // Player inventory class that holds a list of inventory items that can manipulated
    [CreateAssetMenu(menuName = "Systems/Inventory", fileName = "NewInventory")]
    public class PlayerInventory : ScriptableObject, ILoadable{
        //members
        public List<ItemRecipe> ItemRecipes = new();
        public List<InventoryItem> CurrentInventory => PlayerData.playerInventory.inventoryItems;
        public InventoryItem CurrentEquippedItem => PlayerData.playerInventory.currentEquippedItem;
        [HideInInspector]
        public InventoryItem CurrentInitialCombinedItem;
        private int MaxInventorySize => PlayerData.playerInventory.maxInventory;

        public Action CreatedInventoryCallback;
        public Action LoadedInventoryCallback;
        public Action<InventoryItem> OnEquipItem;

        private Logger GlobalLogger => ResourceLoader.GlobalLogger;
        private PlayerData PlayerData => ResourceLoader.MainPlayerData;

        private void OnDisable(){
            PlayerData.LoadValuesCallback -= LoadData;
        }

        public void LoadData(){
            // Don't load the inventory correctly upon load
            if(CurrentInventory.Count <= 0 && MaxInventorySize > 0){
                CreateInventory();
                return;
            }

            if(CurrentEquippedItem != null){
                OnEquipItem?.Invoke(CurrentEquippedItem);
            }

            LoadedInventoryCallback?.Invoke();
        }

        // Method to create the inital inventory for our player.
        public void CreateInventory(){
            ResourceLoader.GlobalLogger.Log("Created the player inventory", this);
            for (int i = 0; i < MaxInventorySize; i++){
                PlayerData.playerInventory.inventoryItems.Add(new InventoryItem(null));
            }
            
            PlayerData.playerInventory.currentEquippedItem = new InventoryItem(null);

            CreatedInventoryCallback?.Invoke();
        }

        public int AttemptToAddItemToInventory(ItemData item, int itemAmount){
            //check if there is enough room in a item stack already
            var validStackableItems = CurrentInventory.Where(inventoryItem => inventoryItem.ItemData == item && !inventoryItem.IsFull()).ToList();

            if(validStackableItems.Count > 0){
                var fullyStackableSlot = validStackableItems.FirstOrDefault(inventoryItem => inventoryItem.CurrentStackSize + itemAmount <= inventoryItem.ItemData.MaxStackSize);

                if(fullyStackableSlot != null){
                    //Add the item into the slot
                    fullyStackableSlot.AddToStack(itemAmount);
                    GlobalLogger.Log("Added to " + fullyStackableSlot.ItemData.Name + " it's total count is now " + fullyStackableSlot.CurrentStackSize, this);
                    return 0;
                }

                int spaceAvailable;
                //cycle through each valid inventory item and add the amount its able to add
                for (int i = 0; i < validStackableItems.Count; i++){
                    spaceAvailable = validStackableItems[i].ItemData.MaxStackSize - validStackableItems[i].CurrentStackSize;
                    validStackableItems[i].AddToStack(spaceAvailable);
                    itemAmount -= spaceAvailable;
                    if(itemAmount <= 0) return 0;
                }
            }
            //check if there is a empty slot in the inventory list
            InventoryItem emptyInventoryItem = CurrentInventory.FirstOrDefault(inventoryItem => inventoryItem.ItemData == null);

            if(emptyInventoryItem != null){
                emptyInventoryItem.SetItem(item, itemAmount);
                GlobalLogger.Log(emptyInventoryItem.ItemData.ToString(), this);
                return 0;
            }
            
            GlobalLogger.Log("Inventory is FULL!", this);
            return itemAmount;
        }

        public void EquipItem(ItemData item, int stackAmount){
            PlayerData.playerInventory.currentEquippedItem.SetItem(item, stackAmount);
            OnEquipItem?.Invoke(CurrentEquippedItem);
        }

        public void UnEquipItem(){
            var EquippedItemData = PlayerData.playerInventory.currentEquippedItem.ItemData;

            if(AttemptToAddItemToInventory(EquippedItemData, PlayerData.playerInventory.currentEquippedItem.CurrentStackSize) == 0){
                PlayerData.playerInventory.currentEquippedItem.SetItem(null, 0);
                OnEquipItem?.Invoke(CurrentEquippedItem);
            }
        }

        public void CombineItems(InventorySlot incomingItemSlot){
            InventoryItem initalItem = CurrentInitialCombinedItem;
            InventoryItem incomingItem = incomingItemSlot.ItemInSlot;

            if(initalItem == incomingItem && initalItem.CurrentStackSize == 1){
                GameManager.UpdateGameState(GameState.StatusScreen);
                return;
            }
            
            // See if there is a valid recipe for the two items
            ItemRecipe validRecipe = null;
            foreach(var recipe in ItemRecipes){
                if(recipe.DetectValidRecipe(initalItem.ItemData, incomingItem.ItemData)){
                    validRecipe = recipe;
                    break;
                }
            }
            
            if(validRecipe != null){
                //Remember them incase not enough space in inventory
                var item1 = initalItem.ItemData;
                var item2 = incomingItem.ItemData;

                //Remove the items and add the new item
                initalItem.RemoveFromStack(1);
                incomingItem.RemoveFromStack(1);
                    
                if(AttemptToAddItemToInventory(validRecipe.resultItem, validRecipe.resultItemAmount) != 0){
                    AttemptToAddItemToInventory(item1, 1);
                    AttemptToAddItemToInventory(item2, 1);
                } 
                GameManager.UpdateGameState(GameState.StatusScreen);
                return;
            }

            if(initalItem == incomingItem){
                GameManager.UpdateGameState(GameState.StatusScreen);
                return;
            }

            // See if there is room in a stack and update the two stacks
            if(initalItem.ItemData == incomingItem.ItemData && !incomingItem.IsFull() && !initalItem.IsFull()){
                int amountMissing = incomingItem.ItemData.MaxStackSize - incomingItem.CurrentStackSize;
                if(amountMissing <= initalItem.CurrentStackSize){
                    incomingItem.AddToStack(amountMissing);
                    initalItem.RemoveFromStack(amountMissing);
                }
                else{
                    incomingItem.AddToStack(initalItem.CurrentStackSize);
                    initalItem.SetItem(null, 0);
                }

                GameManager.UpdateGameState(GameState.StatusScreen);
                return;
            }

            // If the slot is the equip slot return to not swap
            if(incomingItemSlot.IsEquippmentSlot){
                return;
            }

            // Swap the two slots
            SwapItems(incomingItem); 

            GameManager.UpdateGameState(GameState.StatusScreen);
        }

        public void SwapItems(InventoryItem item){
            int index1 = CurrentInventory.IndexOf(CurrentInitialCombinedItem);
            int index2 = CurrentInventory.IndexOf(item);

            // See if the swapped item data is null
            if(item.ItemData == null){
                CurrentInventory[index2].SetItem(CurrentInitialCombinedItem.ItemData, CurrentInitialCombinedItem.CurrentStackSize);
                CurrentInitialCombinedItem.SetItem(null, 0);
                return;
            }

            // Store the item data for swapping
            var swappedItem = item.ItemData;
            var swappedItemStack = item.CurrentStackSize;

            // Swap the item data
            CurrentInventory[index2].SetItem(CurrentInitialCombinedItem.ItemData, CurrentInitialCombinedItem.CurrentStackSize);
            CurrentInventory[index1].SetItem(swappedItem, swappedItemStack);
        }

        public void RemoveGunAmmo(GunType ammoType, int amount){
            var ammoAmount = amount;
            var validInventoryAmmo = ReturnInventoryAmmo(ammoType);
            for (int i = 0; i < validInventoryAmmo.Count; i++){
                if(validInventoryAmmo[i].CurrentStackSize >= ammoAmount){
                    validInventoryAmmo[i].RemoveFromStack(ammoAmount);
                    return;
                }

                ammoAmount -= validInventoryAmmo[i].CurrentStackSize;
                validInventoryAmmo[i].RemoveFromStack(validInventoryAmmo[i].CurrentStackSize);
                if(ammoAmount <= 0) return;
            }
        }

        public int GetAmmoAmount(GunType ammoType){
            var validInventoryAmmo = ReturnInventoryAmmo(ammoType);
            if(validInventoryAmmo == null) return 0;
            int totalStackSize = 0;
            for (int i = 0; i < validInventoryAmmo.Count(); i++){ 
                totalStackSize += validInventoryAmmo[i].CurrentStackSize;
            }
            return totalStackSize;
        }

        public GunEquipableItem GetGunByType(GunType type){
            //check first if the equippable item is the valid gun
            if(CurrentEquippedItem.ItemData != null && CurrentEquippedItem.ItemData.ItemType == ItemType.Gun){
                var EquippedGun = (GunEquipableItem)CurrentEquippedItem.ItemData;
                if(EquippedGun.GunStats.gunType == type){
                    return EquippedGun;
                }
            }
            //if we don't have a gun in the equipped slot check in the inventory if we have a valid gun
            var validInventoryGuns = CurrentInventory.Where(item => item.ItemData != null && item.ItemData.ItemType == ItemType.Gun);
            //guard to protect if we don't have any guns in the inventory
            if(validInventoryGuns == null) return null;
            var validGunEquipableItem = validInventoryGuns.Select(item => item.ItemData)
                                                        .Cast<GunEquipableItem>()
                                                        .Where(gun => gun.GunStats != null && gun.GunStats.gunType == type)
                                                        .FirstOrDefault();
            return validGunEquipableItem;
        }

        private List<InventoryItem> ReturnInventoryAmmo(GunType ammoType){
            IEnumerable<InventoryItem> validInventoryAmmo = CurrentInventory.Where(item => item.ItemData != null && item.ItemData.ItemType == ItemType.AmmoConsumable);
            if(validInventoryAmmo == null) return null;
            List<ItemData> validInventoryItemData = validInventoryAmmo.Select(item => item.ItemData)
                                            .Cast<AmmoConsumableItem>()
                                            .Where(ammo => ammo.AmmoGunType == ammoType)
                                            .Cast<ItemData>()
                                            .ToList();
            List<InventoryItem> validInventoryItems = CurrentInventory.Where(item => validInventoryItemData.Any(data => data == item.ItemData)).ToList();
            return validInventoryItems;
        }
    }
}