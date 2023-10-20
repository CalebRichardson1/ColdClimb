using System;
using TMPro;
using UnityEngine;

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

    [SerializeField] private GameObject contextMenuVisual;
    [SerializeField] private TMP_Text UseText;

    private ItemData itemContext;
    private Transform firstButtonTransform;
    private InventoryUIController controller;
    private MenuSelector inventorySelector;

    private bool isEquipableSlot = false;

    #region Setup
    private void OnEnable() {
        firstButtonTransform = contextMenuVisual.transform.GetChild(0).transform;
    }

    public void SetupContextMenu(InventoryUIController inventoryUI, MenuSelector selector){
        controller = inventoryUI;
        inventorySelector = selector;
        HideContextMenu();
        inventoryUI.OnSlotClicked += ItemContext;
    }

    private void OnDestroy() {
        controller.OnSlotClicked -= ItemContext;
    }

    private void ItemContext(InventorySlot slot){
        SlotContext = slot;
        itemContext = slot.ItemInSlot.ItemData;
        ContextedInventoryItem = slot.ItemInSlot;
        isEquipableSlot = slot.IsEquippmentSlot;

        UpdateContextVisuals();

        inventorySelector.SetTarget(firstButtonTransform);
        contextMenuVisual.SetActive(true);
        GameManager.UpdateGameState(GameState.ContextScreen);
    }

    private void UpdateContextVisuals(){
        if(isEquipableSlot){
            UseText.text = "Unequip";
            return;
        }

        switch (itemContext.ItemType)
        {
            case ItemType.Tool: UseText.text = "Equip";
                return;
            case ItemType.Gun: UseText.text = "Equip";
                return;
        }

        UseText.text = "Use";
    }
    #endregion

    public void CloseContextMenu(){
        HideContextMenu();
        controller.NotInContextMenu();
    }

    public void HideContextMenu(){
        if(SlotContext != null) inventorySelector.SetTarget(SlotContext.transform);
        itemContext = null;
        SlotContext = null;
        contextMenuVisual.SetActive(false);
    }

    public void UseItemAction(){
        if(isEquipableSlot){
            Inventory.UnEquipItem();
            controller.DrawInventorySlotsVisuals();
            isEquipableSlot = false;
        }
        else{
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
        itemContext.CombineItem();
        GameManager.UpdateGameState(GameState.StatusScreen);
    }

    public void DiscardItemAction(){
        itemContext.DiscardItem();
        GameManager.UpdateGameState(GameState.StatusScreen);
    }
}
