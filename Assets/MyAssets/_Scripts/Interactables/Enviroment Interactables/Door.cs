using ColdClimb.Player;
using UnityEngine;

namespace ColdClimb.Interactable{
    public class Door : MonoBehaviour, IInteractable
    {
        public string InteractionPrompt => "Open Door";

        [Header("Door Variables")]        
        [SerializeField] private float speed = 8f;
        [SerializeField] private float rangeToSnap = 0.01f;
        [SerializeField] private bool isOpened = false;
        [SerializeField] private bool isLocked = false;

        [Header("Door Positions")]
        [SerializeField] private Transform openPos;
        [SerializeField] private Transform closePos;

        private Transform doorTransform;

#region Unity Functions
        private void Start() {
            doorTransform = transform;
        }

        private void Update() {
            if(isOpened && !CheckDoorRotation()){
                RotateDoor(openPos);
            }

            if(!isOpened && !CheckDoorRotation()){
                RotateDoor(closePos);
            }
        }

#endregion

#region Public Functions
        public bool Interact(PlayerInteract player){
            if(isLocked){
                return false;
            }

            isOpened = !isOpened;
            return true;
        }

#endregion

#region Private Functions
        private void RotateDoor(Transform pos){
            var currentLerpRotation = Quaternion.Slerp(doorTransform.localRotation, pos.localRotation, Time.deltaTime * speed);
            doorTransform.localRotation = currentLerpRotation;
            if(Vector3.Distance(doorTransform.localRotation.eulerAngles, pos.eulerAngles) <= rangeToSnap){
                doorTransform.localRotation = pos.localRotation;
            }
        }

        private bool CheckDoorRotation(){
            if(isOpened && doorTransform.localRotation == openPos.localRotation){
                return true;
            }
            else if(isOpened){
                return false;
            }

            if(!isOpened && doorTransform.localRotation == closePos.localRotation){
                return true;
            }
            else{
                return false;
            }
        }

#endregion
    }
}

