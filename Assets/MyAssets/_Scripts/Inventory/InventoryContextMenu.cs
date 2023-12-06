using System;
using ColdClimb.Global;
using ColdClimb.Item;
using ColdClimb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColdClimb.Inventory{
    public class InventoryContextMenu : MonoBehaviour
    {
        public static Action<ItemData> OnUseKeyItemAction;
        public static Action<ItemData> OnInspectAction;
        public static Action<ItemData> OnCombineAction;
        public static Action<ItemData> OnDiscardAction;

        public static InventoryItem ContextedInventoryItem;
        public static InventorySlot SlotContext;

        public InventoryUIController Controller => controller;
        public PlayerInventory Inventory => ResourceLoader.PlayerInventory;

        [Header("UI References")]
        [SerializeField] private GameObject contextMenuVisual;
        [SerializeField] private TMP_Text useText;
        [SerializeField] private TMP_Text combineText;
        [SerializeField] private Button discardButton;
        [SerializeField] private Button combineButton;
        [SerializeField] private Transform firstButtonTransform;

        [Header("Dialogue")]
        [SerializeField] private Dialogue failedDiscardDialogue;
        [SerializeField] private Dialogue failedKeyItemUseDialogue;

        private ItemData itemContext;
        private InventoryUIController controller;

        private bool isEquipableSlot = false;

        #region Setup
        public void SetupContextMenu(InventoryUIController inventoryUI){
            controller = inventoryUI;
            HideContextMenu();
            inventoryUI.OnSlotClicked += ItemContext;
        }

        private void OnDestroy(){
            controller.OnSlotClicked -= ItemContext;
        }

        private void ItemContext(InventorySlot slot){
            SlotContext = slot;
            itemContext = slot.ItemInSlot.ItemData;
            ContextedInventoryItem = slot.ItemInSlot;
            isEquipableSlot = slot.IsEquippmentSlot;
            contextMenuVisual.SetActive(true);

            MenuSelector.Instance.SetDefaultSelectedObject(firstButtonTransform);

            UpdateContextVisuals();
            GameManager.UpdateGameState(GameState.ContextScreen);
        }

        private void UpdateContextVisuals(){
            if(isEquipableSlot){
                useText.text = "Unequip";
                discardButton.interactable = false;
                combineButton.interactable = false;
                return;
            }

            switch (itemContext.ItemType){
                case ItemType.Tool or ItemType.Gun or ItemType.PlayerModifier:
                    useText.text = "Equip";
                    combineText.text = "Move";
                    return;
                case ItemType.KeyItem:
                    KeyItem keyItem = (KeyItem)itemContext;
                    if(!keyItem.IsItemCombinable){
                        useText.text = "Use";
                        combineText.text = "Move";
                    }
                    return;
            }

            if(itemContext.IsItemStackable && ContextedInventoryItem.IsFull()){
                combineText.text = "Move";
                return;
            }

            useText.text = "Use";
            combineText.text = "Combine";
        }
        #endregion

        public void CloseContextMenu(){
            HideContextMenu();
            controller.NotInContextMenu();
        }

        public void HideContextMenu(){
            if(SlotContext != null) MenuSelector.Instance.SetDefaultSelectedObject(SlotContext.transform);
            itemContext = null;
            SlotContext = null;
            discardButton.interactable = true;
            combineButton.interactable = true;
            contextMenuVisual.SetActive(false);
        }

        public void UseItemAction(){
            if(isEquipableSlot){
                Inventory.UnEquipItem();
                controller.DrawInventorySlotsVisuals();
                isEquipableSlot = false;
            }
            else if(itemContext.ItemType == ItemType.KeyItem && OnUseKeyItemAction == null){
                if(failedKeyItemUseDialogue.sentences.Length != 0){
                    GlobalUIReference.DialogueController.StartDialogue(failedKeyItemUseDialogue);
                    return;
                }
                
                itemContext.UseItem(this);
                SlotContext.DrawSlotVisual();  
            } 

            GameManager.UpdateGameState(GameState.StatusScreen);
        }

        public void InspectItemAction(){
            OnInspectAction?.Invoke(ContextedInventoryItem.ItemData);
            GameManager.UpdateGameState(GameState.StatusScreen);
        }

        public void CombineItemAction(){
            if(!SlotContext.IsEquippmentSlot){
                itemContext.CombineItem();
                Inventory.CurrentInitialCombinedItem = ContextedInventoryItem;
                controller.IntialCombineSlot(SlotContext);
            }
            GameManager.UpdateGameState(GameState.CombineItemScreen);
        }

        public void DiscardItemAction(){
            if(itemContext.ItemType == ItemType.KeyItem || itemContext.ItemType == ItemType.Gun || itemContext.ItemType == ItemType.Tool){
                // Can't Discard this item
                if(failedDiscardDialogue.sentences.Length != 0){
                    GlobalUIReference.DialogueController.StartDialogue(failedDiscardDialogue);
                    return;
                }
            }
            else{
                itemContext.DiscardItem();
            }

            GameManager.UpdateGameState(GameState.StatusScreen);
        }
}
}
