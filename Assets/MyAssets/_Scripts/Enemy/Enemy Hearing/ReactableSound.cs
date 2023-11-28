using UnityEngine;

namespace ColdClimb.Generic{
    public class ReactableSound{
        // members
        public SoundType soundType {get; private set;}
        public Vector3 posMade {get; private set;}
        public float soundRange{get; private set;}
        public int soundPriority {get; private set;}


        public ReactableSound(Vector3 pos, float range, int priority,SoundType type = SoundType.Default){
            soundType = type;
            posMade = pos;
            soundRange = range;
            soundPriority = priority;
        }
    }

    public enum SoundType{
        Default,
        Interesting,
        Dangerous,
        Call,
        Help
    }
}
