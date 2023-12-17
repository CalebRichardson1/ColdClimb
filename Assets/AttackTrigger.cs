using System;
using UnityEngine;

namespace ColdClimb.Generic{
    public class AttackTrigger : MonoBehaviour{
        public Action<Health> HitPlayerEvent;

        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("Player")) return;

            if(other.TryGetComponent(out Health playerHealth)){
                HitPlayerEvent?.Invoke(playerHealth);
            }
        }
    }
}
