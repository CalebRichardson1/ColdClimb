using System;
using System.Collections;
using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Player;
using ColdClimb.UI;
using UnityEngine;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class Door : MonoBehaviour, IInteractable{
        public string InteractionPrompt => "Open Door";
        public string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }

        [Header("Door Variables")]        
        [SerializeField] private float speed = 8f;
        [SerializeField] private float rangeToSnap = 0.1f;
        [SerializeField] private float minRangeFromToOpen = 2.1f;
        [SerializeField] private bool isLocked = false;

        [Header("Door Positions")]
        [SerializeField] private Transform openPos;
        [SerializeField] private Transform closePos;

        [Header("Door Audio")]
        [SerializeField] private AudioType openDoorAudio;
        [SerializeField] private AudioType closeDoorAudio;
        [SerializeField] private AudioType lockedDoorAudio;

        [Header("Door Flavor")]
        [SerializeField] private Dialogue lockedDoorDialogue;

        private ScenesData ScenesData => ResourceLoader.ScenesData;

        private AudioSource audioSource;

        private Transform doorTransform;

        private bool isMoving = false;

#region Unity Functions
        private void Awake() {
            TryGetComponent(out audioSource);
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;
        }

        private void Start() {
            doorTransform = transform;
        }

        private void OnDestroy(){
            StopAllCoroutines();
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }

#endregion

#region Public Functions
        public bool Interact(PlayerInteract player){
            if(isLocked){
                if(audioSource != null && lockedDoorAudio != AudioType.None){
                    // Play Locked Audio
                    AudioController.instance.PlayAudio(lockedDoorAudio, false, 0, 0, audioSource);
                }

                if(lockedDoorDialogue.sentences.Length != 0){
                    //Trigger Locked Dialouge
                    GlobalUIReference.DialogueController.StartDialogue(lockedDoorDialogue);
                }
                return false;
            }

            if(isMoving){
                return false;
            }
            
            // If player is far enough of the door to open it then open or close the door
            if(Vector3.Distance(player.transform.position, doorTransform.position) >= minRangeFromToOpen){
                Transform targetPos = doorTransform.localRotation == openPos.localRotation ? closePos : openPos;
                AudioType type = targetPos == closePos ? closeDoorAudio : openDoorAudio;

                if(audioSource != null && type != AudioType.None){
                    AudioController.instance.PlayAudio(type, false, 0, 0, audioSource);
                }

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
            while(doorTransform.rotation != pos.rotation && isMoving){
                var currentLerpRotation = Quaternion.Lerp(doorTransform.rotation, pos.rotation, Time.deltaTime * speed);
                doorTransform.rotation = currentLerpRotation;
                if(Vector3.Distance(doorTransform.rotation.eulerAngles, pos.eulerAngles) <= rangeToSnap){
                    doorTransform.rotation = pos.rotation;
                    isMoving = false;
                    break;
                }
                yield return null;
            }
        }

        private void SaveState(){
            
        }

        private void LoadData(){

        }
#endregion
    }
}

