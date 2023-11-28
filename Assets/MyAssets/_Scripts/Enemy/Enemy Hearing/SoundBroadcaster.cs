using UnityEngine;

namespace ColdClimb.Generic{
    public static class SoundBroadcaster{
        public static void MakeSound(ReactableSound sound){
            Collider[] colliders = Physics.OverlapSphere(sound.posMade, sound.soundRange);

            for (int i = 0; i < colliders.Length; i++){
                if(colliders[i].TryGetComponent(out IHearer hearer)){
                    hearer.RespondToSound(sound);
                }
            }
        }
    }
}
