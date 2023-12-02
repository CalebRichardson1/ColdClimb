using ColdClimb.Item;
using UnityEngine;

namespace ColdClimb.Inventory{
    [CreateAssetMenu(menuName = "Item/New Item Recipe", fileName = "NewItemRecipe")]
    public class ItemRecipe : ScriptableObject{        
        public ItemData item1;
        public ItemData item2;
        public ItemData resultItem;
        public int resultItemAmount;

        public bool DetectValidRecipe(ItemData firstItem, ItemData secondItem){
            if(firstItem == null || secondItem == null){
                return false;
            }

            if ((firstItem == item1 && secondItem == item2) || (firstItem == item2 && secondItem == item1)){
                return true; // Valid recipe found
            }
            
            return false;
        }
    }
}
