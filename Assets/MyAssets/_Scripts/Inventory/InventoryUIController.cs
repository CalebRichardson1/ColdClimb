using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using ColdClimb.Global;
using ColdClimb.UI;

namespace ColdClimb.Inventory{
    // Handles the Inventory UI Logic
    public class InventoryUIController : MonoBehaviour{
        public Action<InventorySlot> OnSlotSelected;
        public Action<InventorySlot> OnSlotClicked;
        public static InventorySlot CurrentEquippedItemSlot;

        [Header("Dependencies")]
        [SerializeField] private InventoryUIVisual inventoryScreenPrefab;
        [SerializeField] private InventorySlot inventorySlotPrefab; 
        [SerializeField] private InventoryContextMenu contextMenu;

        private InventoryUIVisual instancedInventoryScreen;
        private InventoryContextMenu instancedContexMenu;

        private MenuSelector Selector => MenuSelector.Instance;
        private PlayerInventory Inventory => ResourceLoader.PlayerInventory;

        private List<InventorySlot> inventorySlotsUI = new();

        private InventorySlot currentSelectedSlot;

        private void Awake(){
            GameManager.OnGameStateChange += GameStateChange;
            Inventory.CreatedInventoryCallback += CreateInventoryMenu;
            Inventory.LoadedInventoryCallback += LoadInventorySlots;
        }

        private void OnDestroy(){
            GameManager.OnGameStateChange -= GameStateChange;
            Inventory.CreatedInventoryCallback -= CreateInventoryMenu;
            Inventory.LoadedInventoryCallback -= LoadInventorySlots;
        } 

        public void OnSlotSelectedAction(InventorySlot selectedSlot){
            currentSelectedSlot = selectedSlot;
            OnSlotSelected?.Invoke(selectedSlot);
        }

        public void OnViableSlotClickedAction(InventorySlot selectedSlot){
            //See if we are in the combine state
            if(GameManager.CurrentState == GameState.CombineItemScreen && Inventory.CurrentInitialCombinedItem != null){
                Inventory.CombineItems(selectedSlot);
                return;
            }

            InContextMenu();
            OnSlotClicked?.Invoke(selectedSlot);
        }

        public void IntialCombineSlot(InventorySlot slot){
            GlobalUIReference.CombineSelector.SetTarget(slot.transform);
        }

        // Creates the visual inventory that the player sees
        private void CreateInventoryMenu(){
            instancedInventoryScreen = Instantiate(inventoryScreenPrefab, transform.position, Quaternion.identity, transform);
            instancedInventoryScreen.SetInventoryUIController(this);
            instancedInventoryScreen.transform.SetAsFirstSibling();
            CurrentEquippedItemSlot = instancedInventoryScreen.EquippedItemButton.GetComponent<InventorySlot>();

            //instanitate instanced context menu
            instancedContexMenu = Instantiate(contextMenu, instancedInventoryScreen.ContextMenuSpawnPoint.position, 
                                        Quaternion.identity, instancedInventoryScreen.transform);
            //pass in this controller so the context menu can subscribe to the on click event
            instancedContexMenu.SetupContextMenu(this);

            CreateInventorySlots();
        }
        
        //Creates the slots that the player can interact with
        private void CreateInventorySlots(){
            foreach (var item in Inventory.CurrentInventory){
                var newSlot = Instantiate(inventorySlotPrefab, transform.position, Quaternion.identity, instancedInventoryScreen.InventorySlotContent);
                newSlot.SetupSlot(this, item);
                inventorySlotsUI.Add(newSlot);
            }
            //scale the layout grid group cell size by the max inventory size
            CurrentEquippedItemSlot.SetupSlot(this, Inventory.CurrentEquippedItem);
        }

        private void LoadInventorySlots(){
            if(instancedInventoryScreen == null){
                CreateInventoryMenu();
                return;
            }

            foreach (var slot in inventorySlotsUI){
                Destroy(slot.gameObject);
            }

            inventorySlotsUI.Clear();
            CreateInventorySlots();
        }

        private void GameStateChange(GameState state){
            //if our state isn't the inventory screen set the inventory ui to false and return
            if(state != GameState.ContextScreen && instancedContexMenu != null){
                instancedContexMenu.CloseContextMenu();
            }
            
            if (state != GameState.StatusScreen && state != GameState.ContextScreen && state != GameState.CombineItemScreen && instancedInventoryScreen != null){
                instancedInventoryScreen.gameObject.SetActive(false);
                return;
            }

            if (currentSelectedSlot == null){
                // Setting the default cursor position in our inventory;
                currentSelectedSlot = inventorySlotsUI[0];
                Selector.SetDefaultSelectedObject(inventorySlotsUI[0].transform);
            }
            else{
                Selector.SetDefaultSelectedObject(currentSelectedSlot.transform);
            }

            DrawInventorySlotsVisuals();
            instancedInventoryScreen.gameObject.SetActive(true);
        }

        private void InContextMenu(){
            instancedInventoryScreen.EquippedItemButton.interactable = false;
            inventorySlotsUI.ForEach(slot => slot.GetComponent<Button>().interactable = false);
        }



        public void DrawInventorySlotsVisuals(){
            CurrentEquippedItemSlot.DrawSlotVisual();
            inventorySlotsUI.ForEach(slot => slot.DrawSlotVisual());
            instancedInventoryScreen.UpdateSelectedInfo(currentSelectedSlot);
        }

        public void NotInContextMenu(){
            instancedInventoryScreen.EquippedItemButton.interactable = true;
            inventorySlotsUI.ForEach(slot => slot.GetComponent<Button>().interactable = true);
        }
    }
}