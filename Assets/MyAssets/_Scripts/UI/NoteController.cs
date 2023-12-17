using System;
using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Player;
using ColdClimb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class NoteController : MonoBehaviour, IInteractable{
        public string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }
        public string InteractionPrompt => "Read Note";
        [SerializeField, TextArea(6, 6)] private string noteText;

        [Header("Note Juice")]
        [SerializeField] private AudioType noteShowSFX;
        [SerializeField] private AudioType noteHideSFX;
        [SerializeField] private Dialogue dialogueAfterNote;

        private GameObject NoteCanvas => GlobalUIReference.NoteCanvas;
        private TMP_Text NoteTextAreaUI => GlobalUIReference.NoteTextAreaUI;
        private Animator noteAnimator;
        private InputManager InputManager => ResourceLoader.InputManager;
        private ScenesData ScenesData => ResourceLoader.ScenesData;

        private const string SHOW_NOTE = "Show_Note";
        private const string HIDE_NOTE = "Hide_Note";

        private bool isNoteOpen = false;
        private bool hasRead = false;
#region Unity Functions
        private void Awake() {
            TryGetComponent(out noteAnimator); 
            InputManager.ReturnPauseAction().started += PauseGameAction;
            InputManager.ReturnCancelAction().started += PauseGameAction;
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;
        }

        private void OnDestroy() {
            InputManager.ReturnPauseAction().started -= PauseGameAction;
            InputManager.ReturnCancelAction().started -= PauseGameAction;
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }
#endregion

#region Public Functions
        public bool Interact(PlayerInteract player){
            ShowNote();
            return true;
        }
#endregion

#region Private Functions
        private void SaveState(){
            if(ScenesData.CurrentSceneData.ReadNotes.ContainsKey(id)){
                ScenesData.CurrentSceneData.ReadNotes.Remove(id);
            }
            ScenesData.CurrentSceneData.ReadNotes.Add(id, hasRead);
        }

        private void LoadData(){
            ScenesData.CurrentSceneData.ReadNotes.TryGetValue(id, out hasRead);
        }

        private void ShowNote(){
            NoteTextAreaUI.text = noteText;
            NoteCanvas.SetActive(true);
            GameManager.UpdateGameState(GameState.NoteScreen);
            isNoteOpen = true;

            //Juice
            AudioController.instance.PlayAudio(noteShowSFX);
            if(noteAnimator != null){
                PlayAnimation(SHOW_NOTE);
            }
        }

        private void HideNote(){
            NoteCanvas.SetActive(false);
            isNoteOpen = false;

            //Juice
            AudioController.instance.PlayAudio(noteHideSFX);
            if(noteAnimator != null){
                PlayAnimation(HIDE_NOTE);
            }

            if(dialogueAfterNote.sentences.Length != 0 && !hasRead){
                GlobalUIReference.DialogueController.StartDialogue(dialogueAfterNote);
                hasRead = true;
                return;
            }
            GameManager.UpdateGameState(GameState.MainGame);
        }

        private void PlayAnimation(string animationName){
            noteAnimator.Play(animationName, -1, 0);
        }
        
        private void PauseGameAction(InputAction.CallbackContext context){
            if(isNoteOpen && GameManager.CurrentState == GameState.NoteScreen){
                HideNote();
            }
        }
#endregion
    }
}
