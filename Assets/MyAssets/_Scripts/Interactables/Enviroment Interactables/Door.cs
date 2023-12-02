using System.Collections;
using ColdClimb.Player;
using UnityEngine;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class Door : MonoBehaviour, IInteractable
    {
        public string InteractionPrompt => "Open Door";

        [Header("Door Variables")]        
        [SerializeField] private float speed = 8f;
        [SerializeField] private bool isOpened = false;
        [SerializeField] private bool isLocked = false;

        [Header("Rotation Config")]
        [SerializeField] private float rotationAmount = 90f;
        [SerializeField] private Transform forwardTransform;

        [Header("Sound Options")]
        [SerializeField] private AudioType interactAudio;
        [SerializeField] private AudioType lockedAudio;

        private Vector3 startRotation;
        private Vector3 forward;

        private Coroutine doorAnimationCoroutine;

#region Unity Functions
        private void Start() {
            startRotation = transform.rotation.eulerAngles;

            forward = -transform.forward;
        }

#endregion

#region Public Functions
        public bool Interact(PlayerInteract player){
            if(isLocked){
                return false;
            }

            if(doorAnimationCoroutine != null){
                StopCoroutine(doorAnimationCoroutine);
            }

            if(!isOpened){
                // Open the door
                float dot = Vector3.Dot(forward, (player.transform.position - transform.position).normalized);
                Debug.Log($"Dot: {dot:N3}");
                doorAnimationCoroutine = StartCoroutine(OpenDoorAnimationCoroutine(dot));
            }
            else{
                // Close the door
                doorAnimationCoroutine = StartCoroutine(CloseDoorAnimationCoroutine());
            }
            return true;
        }

        public void UnlockDoor(){
            isLocked = false;
        }

#endregion

#region Private Functions
        private IEnumerator OpenDoorAnimationCoroutine(float forwardAmount){
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation;

            if(forwardAmount >= 0){
                endRotation = Quaternion.Euler(new Vector3(0, startRotation.y + rotationAmount, 0));
            } 
            else{
                endRotation = Quaternion.Euler(new Vector3(0, startRotation.y - rotationAmount, 0));
            }

            isOpened = true;

            float time = 0;
            while (time < 1){
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
                yield return null;
                time += Time.deltaTime * speed;
                transform.rotation = endRotation;
            }
        }

        private IEnumerator CloseDoorAnimationCoroutine(){
            Quaternion _startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(startRotation);

            isOpened = false;

            float time = 0;
            while (time < 1){
            transform.rotation = Quaternion.Slerp(_startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
            }
        }

#endregion
    }
}

