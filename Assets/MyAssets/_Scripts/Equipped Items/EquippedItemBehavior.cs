using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Item.Equipped{

    public abstract class EquippedItemBehavior : MonoBehaviour{
        [Header("Base Equipped Item Variables")]
        [SerializeField] protected bool canAltUse;
        [SerializeField] protected float actionCooldownTime;

        protected bool onCooldown;

        public abstract void SetupBehavior(EquipableItem item);
        public abstract void Use(InputAction action);
        public abstract void AltUse(InputAction action);
        public abstract void UseResource(InputAction action);
        public async void ActionCooldown(float cooldownTime){
            onCooldown = true;
            await Task.Delay((int)(cooldownTime * 1000));
            onCooldown = false;
        }
    }
}
    
