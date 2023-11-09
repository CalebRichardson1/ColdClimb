using System;
using System.Collections.Generic;
using System.Linq;
using SaveLoadSystem;
using UnityEngine;

// Player inventory class that holds a list of inventory items that can manipulated
[CreateAssetMenu(menuName = "Systems/Inventory", fileName = "NewInventory")]
public class PlayerInventory : ScriptableObject, ILoadable
{
    #region Variables
    public List<InventoryItem> CurrentInventory => PlayerData.playerInventory.inventoryItems;
    public InventoryItem CurrentEquippedItem => PlayerData.playerInventory.currentEquippedItem;
    private int MaxInventorySize => PlayerData.playerInventory.maxInventory;

    public Action CreatedInventoryCallback;
    public Action LoadedInventoryCallback;
    public Action<InventoryItem> OnEquipItem;

    private Logger GlobalLogger => ResourceLoader.GlobalLogger;
    private PlayerData PlayerData => ResourceLoader.MainPlayerData;
    #endregion

    #region Setup
    private void OnEnable() {
        PlayerData.LoadValuesCallback += LoadData;
    }

    private void OnDisable() {
        PlayerData.LoadValuesCallback -= LoadData;
    }

    public void LoadData(){
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
    #endregion

    #region Manipulating The Inventory
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
    #endregion

    #region Reading The Inventory
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
    #endregion
}