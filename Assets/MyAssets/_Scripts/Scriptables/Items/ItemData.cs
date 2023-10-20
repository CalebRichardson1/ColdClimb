using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class ItemData : ScriptableObject
{
    public string Name => itemName;
    public ItemType ItemType => itemType;
    public List<string> Description => itemDescription;
    public Image Image => itemImage;
    public bool IsItemStackable => isStackable;
    public int MaxStackSize => maxStackSize;

    [Header("Base Item Info")]
    [SerializeField] private ItemType itemType;

    [Header("Item Visual")]
    [SerializeField] private string itemName;
    [SerializeField] private List<string> itemDescription;
    [SerializeField] private Image itemImage;

    [Header("Item Stack Settings")]
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStackSize;
    
    public abstract void UseItem(InventoryContextMenu contextMenu);
    public abstract void DiscardItem();
    public abstract void CombineItem();
}

public enum ItemType{
    HealthConsumable,
    AmmoConsumable,
    KeyItem,
    Gun,
    Tool,
    EquipmentModifier,
    ConsumableModifier,
    PlayerModifier
}
