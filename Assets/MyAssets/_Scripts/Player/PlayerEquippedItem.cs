using ColdClimb.Global;
using ColdClimb.Item.Equipped;
using ColdClimb.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
   public class PlayerEquippedItem : MonoBehaviour{
      private InputManager InputManager => ResourceLoader.InputManager;

      private EquippedItemBehavior currentEquippedItem;

      private InputAction UseAction => InputManager.ReturnUseEquippedItemAction();
      private InputAction AltUseAction => InputManager.ReturnAltUseEquippedItemAction();
      private InputAction ReloadAction => InputManager.ReturnReloadAction();

      private bool canUseItem = true;

      private void OnEnable(){
         ResourceLoader.PlayerInventory.OnEquipItem += SetCurrentEquippedItem;
         GameManager.OnGameStateChange += (state) => canUseItem = state == GameState.MainGame;
      }

      private void OnDisable() {
         ResourceLoader.PlayerInventory.OnEquipItem -= SetCurrentEquippedItem;
      }

      public void SetCurrentEquippedItem(InventoryItem equippedItem){
         EquipableItem equipableItem = (EquipableItem)equippedItem.ItemData;

         if(currentEquippedItem != null){
            //unsub from event with this item behavior
            Destroy(currentEquippedItem.gameObject);
            GameManager.OnGameStateChange -= (state) => currentEquippedItem.enabled = state == GameState.MainGame;
         }

         if(equipableItem == null) return;

         EquippedItemBehavior prefabBehavior = equipableItem.ItemBehavior;

         if(prefabBehavior == null){
            Debug.Log("No item behavior was found on: " + equipableItem + " returning...");
            return;
         }

         //spawning the equipped item and setting the local variables to 0
         currentEquippedItem = Instantiate(prefabBehavior, transform.position, Quaternion.identity, transform);
         currentEquippedItem.transform.localPosition = Vector3.zero;
         currentEquippedItem.transform.localEulerAngles = Vector3.zero;
         currentEquippedItem.SetupBehavior(equipableItem);
      }

      private void Update() {
         if(currentEquippedItem == null || !canUseItem) return;

         currentEquippedItem.Use(UseAction);
         currentEquippedItem.AltUse(AltUseAction);
         currentEquippedItem.UseResource(ReloadAction);
      }
   }
}
