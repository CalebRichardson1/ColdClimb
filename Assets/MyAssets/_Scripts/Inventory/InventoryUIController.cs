using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

// Handles the Inventory UI Logic
public class InventoryUIController : MonoBehaviour
{
    #region Variables
    public Action<InventorySlot> OnSlotSelected;
    public Action<InventorySlot> OnSlotClicked;
    public static InventorySlot CurrentEquippedItemSlot;

    [Header("Dependencies")]
    [SerializeField] private InventoryUIVisual inventoryScreenPrefab;
    [SerializeField] private InventorySlot inventorySlotPrefab; 
    [SerializeField] private MenuSelector selector;
    [SerializeField] private InventoryContextMenu contextMenu;

    private InventoryUIVisual instancedInventoryScreen;
    private InventoryContextMenu instancedContexMenu;

    private PlayerInventory PlayerInventory => ResourceLoader.PlayerInventory;

    private List<InventorySlot> inventorySlotsUI = new();

    private InventorySlot currentSelectedSlot;
    #endregion

    #region Setup
    private void Awake(){
        GameManager.OnGameStateChange += GameStateChange;
        PlayerInventory.CreatedInventoryCallback += CreateInventoryMenu;
        PlayerInventory.LoadedInventoryCallback += LoadInventorySlots;
    }

    private void OnDestroy(){
        GameManager.OnGameStateChange -= GameStateChange;
        PlayerInventory.CreatedInventoryCallback -= CreateInventoryMenu;
        PlayerInventory.LoadedInventoryCallback -= LoadInventorySlots;
    } 
    #endregion

    public void OnSlotSelectedAction(InventorySlot selectedSlot){
        currentSelectedSlot = selectedSlot;
        OnSlotSelected?.Invoke(selectedSlot);
    }

    public void OnViableSlotClickedAction(InventorySlot selectedSlot){
        InContextMenu();
        OnSlotClicked?.Invoke(selectedSlot);
    }

    #region Updating Inventory Screen
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
        instancedContexMenu.SetupContextMenu(this, selector);

        CreateInventorySlots();
    }
    
    //Creates the slots that the player can interact with
    private void CreateInventorySlots(){
        foreach (var item in PlayerInventory.CurrentInventory){
            var newSlot = Instantiate(inventorySlotPrefab, transform.position, Quaternion.identity, instancedInventoryScreen.InventorySlotContent);
            newSlot.SetupSlot(this, item);
            inventorySlotsUI.Add(newSlot);
        }
        //scale the layout grid group cell size by the max inventory size
        CurrentEquippedItemSlot.SetupSlot(this, PlayerInventory.CurrentEquippedItem);
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
        
        if (state != GameState.StatusScreen && state != GameState.ContextScreen && instancedInventoryScreen != null){
            instancedInventoryScreen.gameObject.SetActive(false);
            return;
        }

        if (currentSelectedSlot == null)
        {
            // Setting the default cursor position in our inventory;
            currentSelectedSlot = inventorySlotsUI[0];
            selector.SetDefaultSelectedObject(inventorySlotsUI[0].transform);
        }

        DrawInventorySlotsVisuals();
        instancedInventoryScreen.gameObject.SetActive(true);
    }

    public void DrawInventorySlotsVisuals(){
        CurrentEquippedItemSlot.DrawSlotVisual();
        inventorySlotsUI.ForEach(slot => slot.DrawSlotVisual());
        instancedInventoryScreen.UpdateSelectedInfo(currentSelectedSlot);
    }

    private void InContextMenu(){
        instancedInventoryScreen.EquippedItemButton.interactable = false;
        inventorySlotsUI.ForEach(slot => slot.GetComponent<Button>().interactable = false);
    }

    public void NotInContextMenu(){
        instancedInventoryScreen.EquippedItemButton.interactable = true;
        inventorySlotsUI.ForEach(slot => slot.GetComponent<Button>().interactable = true);
    }
    #endregion
}
