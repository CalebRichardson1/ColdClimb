// Class that holds item data and a stack size
using System;
using ColdClimb.Item;
using UnityEngine;

namespace ColdClimb.Inventory{
    [Serializable]
    public class InventoryItem{
        public ItemData ItemData => itemData;
        public int CurrentStackSize => currentStackSize;

        [SerializeField] private int currentStackSize;
        [SerializeField] private ItemData itemData;

        public InventoryItem(ItemData heldItem){
            itemData = heldItem;
        }

        public void AddToStack(int amount){
            currentStackSize += amount;
        }

        public void RemoveFromStack(int amount){
            currentStackSize -= amount;

            if(currentStackSize == 0){
                itemData = null;
            }
        }

        public void DiscardItem(){
            RemoveFromStack(currentStackSize);
        }

        public bool IsFull(){
            return itemData == null || !itemData.IsItemStackable || currentStackSize == itemData.MaxStackSize;
        }

        public void SetItem(ItemData item, int stackSize){
            itemData = item;
            if(itemData == null){
                currentStackSize = 0;
                return;
            }
            currentStackSize = stackSize;
        }
    }
}
