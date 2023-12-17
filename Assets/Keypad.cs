using System;
using System.Collections;
using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Player;
using ColdClimb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class Keypad : MonoBehaviour{
        public string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }
        //members
        [Header("Keypad Meta")]
        [SerializeField] private string password;
        [SerializeField] private UnityEvent unlockEvent;
        [SerializeField] private Transform playerCamPlacement;
        [SerializeField] private float playerMoveSpeed = 5f;

        [Header("Keypad Visuals")]
        [SerializeField] private GameObject keyPadCanvas;
        [SerializeField] private Transform firstButtonPos;
        [SerializeField] private TMP_Text answerText;

        [Header("Keypad Audio")]
        [SerializeField] private AudioType buttonClickedSoundEffect;
        [SerializeField] private AudioType unlockSoundEffect;
        [SerializeField] private AudioType failedSoundEffect;

        private InputManager InputManager => ResourceLoader.InputManager;
        private ScenesData ScenesData => ResourceLoader.ScenesData;

        private const float snapDistance = 0.25f;

        private Transform playerCamPos; 

        private Vector3 playerCamOriginalPos;
        private Quaternion playerCamOriginalRotation;
        private PlayerInteract player;

        private AudioSource audioSource;

        private bool isMoving;
        private bool isUnlocked;

        private string currentEntry = "";
        private int previousCharPos = 0;
        private char incomingChar = new();

        private void Awake() {
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;
        }

        private void OnDestroy(){
            InputManager.ReturnCancelAction().started -= CancelKeypad;
            InputManager.ReturnPauseAction().started -= CancelKeypad;
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }

        private void SaveState(){
            if(ScenesData.CurrentSceneData.KeyPadsUnlocked.ContainsKey(id)){
                ScenesData.CurrentSceneData.KeyPadsUnlocked.Remove(id);
            }
            ScenesData.CurrentSceneData.KeyPadsUnlocked.Add(id, isUnlocked);
        }

        private void LoadData(){
            ScenesData.CurrentSceneData.InspectObjectsUpdated.TryGetValue(id, out isUnlocked);
            if(isUnlocked){
                GetComponent<Collider>().enabled = false;
            }
        }

        private void Start() {
            InputManager.ReturnCancelAction().started += CancelKeypad;
            InputManager.ReturnPauseAction().started += CancelKeypad;
            keyPadCanvas.SetActive(false);
            answerText.text = "";
            TryGetComponent(out audioSource);
        }

        public void ShowKeypadUI(PlayerInteract playerInteract){
            if(player == null){
                player = playerInteract;
            }

            Invoke(nameof(ShowKeypad), 0.1f);
        }   

        public void NumButtonPressed(int number){
            if(buttonClickedSoundEffect != AudioType.None && audioSource != null){
                AudioController.instance.PlayAudio(buttonClickedSoundEffect, false, 0, 0, audioSource);
            }
            incomingChar = number switch{
                0 => '0',
                1 => '1',
                2 => '2',
                3 => '3',
                4 => '4',
                5 => '5',
                6 => '6',
                7 => '7',
                8 => '8',
                9 => '9',
                _ => ' ',
            };

            UpdateAnswerPanel(incomingChar);
        }

        public void ClearNUM(){
            if(failedSoundEffect != AudioType.None && audioSource != null){
                AudioController.instance.PlayAudio(failedSoundEffect, false, 0, 0, audioSource);
            }
            currentEntry = "";
            previousCharPos = -1;
            answerText.text = currentEntry;
        }

        public void EnterNUM(){
            if(currentEntry == password){
                unlockEvent?.Invoke();
                isUnlocked = true;
                //Play unlock audio
                if(unlockSoundEffect != AudioType.None && audioSource != null){
                    AudioController.instance.PlayAudio(unlockSoundEffect, false, 0, 0, audioSource);
                }
                StartCoroutine(ResetPlayerCam(player.transform));
                GetComponent<Collider>().enabled = false;
            }
            else{
                //play failed unlock audio
                if(failedSoundEffect != AudioType.None && audioSource != null){
                    AudioController.instance.PlayAudio(failedSoundEffect, false, 0, 0, audioSource);
                }
            }
        }

        private void UpdateAnswerPanel(char num){
            if(num == ' '){
                Debug.Log("Invalid number put into the NumButtonPressed Function...");
                return;
            }

            //check the char count of current entry
            int charCount = 0;
            foreach (char character in currentEntry){
                charCount++;
            }

            if(charCount >= 4){
                //get the next char pos from the previousCharPos
                int nextCharPos = previousCharPos + 1;

                if(nextCharPos >= 4){
                    nextCharPos = 0;
                }

                char[] currentCharArray = currentEntry.ToCharArray();

                if(currentCharArray[nextCharPos] == num){
                    //same char
                    return;
                }

                //replace the number after previous char pos
                currentCharArray[nextCharPos] = num;
                currentEntry = new string(currentCharArray);

                //set the next char as the previous char
                previousCharPos = nextCharPos;
            }
            else{
                //add the char to the current string
                currentEntry += num;

                previousCharPos = currentEntry.Length - 1;
            }


            answerText.text = currentEntry;
        }

        private void ShowKeypad(){
            GameManager.UpdateGameState(GameState.KeypadScreen);

            keyPadCanvas.SetActive(true);
            playerCamOriginalPos = player.transform.position;
            playerCamOriginalRotation = player.transform.rotation;

            MenuSelector.Instance.SetDefaultSelectedObject(firstButtonPos);

            StartCoroutine(MovePlayerCam(player.transform));
        }

        private IEnumerator MovePlayerCam(Transform playerCam){
            isMoving = true;
            while(playerCam.position != playerCamPlacement.position){
                if(Vector3.Distance(playerCam.position, playerCamPlacement.position) <= snapDistance){
                    playerCam.position = playerCamPlacement.position;
                    playerCam.rotation = playerCamPlacement.rotation;
                    break;
                }
                playerCam.position = Vector3.Lerp(playerCam.position, playerCamPlacement.position, playerMoveSpeed * Time.deltaTime);
                playerCam.rotation = Quaternion.Lerp(playerCam.rotation, playerCamPlacement.rotation, playerMoveSpeed * Time.deltaTime);
                yield return null;
            }
            isMoving = false;

            //set the selector to be visible
            MenuSelector.Instance.SetIsValid(true);
        }

        private IEnumerator ResetPlayerCam(Transform playerCam){
            isMoving = true;
            keyPadCanvas.SetActive(false);
            MenuSelector.Instance.SetIsValid(false);
             while(playerCam.position != playerCamOriginalPos){
                if(Vector3.Distance(playerCam.position, playerCamOriginalPos) <= snapDistance){
                    playerCam.position = playerCamOriginalPos;
                    playerCam.rotation = playerCamOriginalRotation;
                    break;
                }
                playerCam.position = Vector3.Lerp(playerCam.position, playerCamOriginalPos, playerMoveSpeed * Time.deltaTime);
                playerCam.rotation = Quaternion.Lerp(playerCam.rotation, playerCamOriginalRotation, playerMoveSpeed * Time.deltaTime);
                yield return null;
            }

            isMoving = false;
            keyPadCanvas.SetActive(false);
            GameManager.UpdateGameState(GameState.MainGame);
        }

        private void CancelKeypad(InputAction.CallbackContext context){
            if(isMoving) return;
            if(GameManager.CurrentState == GameState.KeypadScreen){
                StartCoroutine(ResetPlayerCam(player.transform));
            }
        }
    }
}
