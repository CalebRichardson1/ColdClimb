using UnityEngine;

namespace ColdClimb.Generic{

    public class LockRotation : MonoBehaviour{
        [SerializeField] private Transform lockRot;
        private void Update(){
            transform.localRotation = lockRot.localRotation;
        }
    }
}
