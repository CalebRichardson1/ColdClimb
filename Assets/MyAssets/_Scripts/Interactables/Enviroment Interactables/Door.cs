using System.Collections;
using ColdClimb.Player;
using UnityEngine;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class Door : MonoBehaviour, IInteractable{
        public string InteractionPrompt => "Open Door";
        [Header("Door Audio")]
        [SerializeField] private AudioType openDoorAudio;
        [SerializeField] private AudioType lockedDoorAudio;

        [Header("Door Variables")]        
        [SerializeField] private float speed = 8f;
        [SerializeField] private float rangeToSnap = 0.1f;
        [SerializeField] private bool isOpened = false;
        [SerializeField] private bool isLocked = false;

        [Header("Door Positions")]
        [SerializeField] private Transform openPos;
        [SerializeField] private Transform closePos;

        private Transform doorTransform;

        private bool isMoving = false;

#region Unity Functions
        private void Start() {
            doorTransform = transform;
        }

        private void OnDestroy(){
            StopAllCoroutines();
        }

#endregion

#region Public Functions
        public bool Interact(PlayerInteract player){
            if(isLocked || isMoving){
                return false;
            }

            // If player is far enough of the door to open it then open or close the door
            if(Vector3.Distance(player.transform.position, doorTransform.position) >= 2.1f){
                Transform targetPos = doorTransform.localRotation == openPos.localRotation ? closePos : openPos;
                StartCoroutine(RotateDoorCoroutine(targetPos));
                
                return true;
            }
            
            return false;
        }

        public void UnlockDoor(){
            isLocked = false;
        }

#endregion

#region Private Functions
        private IEnumerator RotateDoorCoroutine(Transform pos){
            isMoving = true;
            while(doorTransform.localRotation != pos.localRotation){
                var currentLerpRotation = Quaternion.Lerp(doorTransform.localRotation, pos.localRotation, Time.deltaTime * speed);
                doorTransform.localRotation = currentLerpRotation;
                if(Vector3.Distance(doorTransform.localRotation.eulerAngles, pos.eulerAngles) <= rangeToSnap){
                    doorTransform.localRotation = pos.localRotation;
                    break;
                }
                yield return null;
            }
            isMoving = false;
        }
        #endregion
    }
}

