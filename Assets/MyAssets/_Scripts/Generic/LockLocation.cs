using UnityEngine;

namespace ColdClimb.Generic{

    public class LockLocation : MonoBehaviour{
        [SerializeField] private Transform lockPos;

        private void Update(){
            transform.position = lockPos.position;
        }
    }   
}
