using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Health Consumable", fileName = "NewHealthConsumable"), System.Serializable]
public class HealthConsumableItem : ItemData
{
    //consumable data
    [Header("Health Consumable Heal Amount")]
    [SerializeField] private int healAmount = 10;
    private Health playerHealth;

    #region Setup
    private void OnEnable() {
        Health.IsPlayerAction += (currentPlayerHealth) => playerHealth = currentPlayerHealth;
    }

    private void OnDisable() {
        Health.IsPlayerAction -= (currentPlayerHealth) => playerHealth = currentPlayerHealth;
    }
    #endregion

    //item base methods
    public override void CombineItem()
    {
        
    }

    public override void DiscardItem()
    {
        
    }

    public override void UseItem(InventoryContextMenu contextMenu){
        if(playerHealth.CurrentHealth == playerHealth.MaxHealth){
            //print that you can't use this item
            return;
        }

        playerHealth.HealHealth(healAmount);
        InventoryContextMenu.ContextedInventoryItem.RemoveFromStack(1);
    }
}