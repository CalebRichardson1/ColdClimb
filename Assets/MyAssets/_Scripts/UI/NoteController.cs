using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.Player;
using TMPro;
using UnityEngine;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class NoteController : MonoBehaviour, IInteractable{
        public string InteractionPrompt => "Read Note";

        [Header("UI Text References")]
        [SerializeField] private GameObject noteCanvas;
        [SerializeField] private TMP_Text noteTextAreaUI;

        [Header("Note Juice")]
        [SerializeField] private AudioType noteShowSFX;
        [SerializeField] private AudioType noteHideSFX;

        [Space(10)]
        [SerializeField, TextArea] private string noteText;

        private Animator NoteAnimator => GetComponent<Animator>();

        private const string SHOW_NOTE = "Show_Note";
        private const string HIDE_NOTE = "Hide_Note";

        private bool isNoteOpen = false;
#region Unity Functions
        private void Awake() {
            GameManager.OnGameStateChange += (state) => {if(state == GameState.MainGame && isNoteOpen == true) HideNote();};
        }

        private void Start() {
            noteCanvas.SetActive(false);
        }

        private void OnDestroy() {
            GameManager.OnGameStateChange -= (state) => {if(state == GameState.MainGame && isNoteOpen == true) HideNote();};
        }
#endregion

#region Public Functions
        public bool Interact(PlayerInteract player){
            ShowNote();
            return true;
        }
#endregion

#region Private Functions
        private void ShowNote(){
            noteTextAreaUI.text = noteText;
            noteCanvas.SetActive(true);
            GameManager.UpdateGameState(GameState.NoteScreen);
            isNoteOpen = true;

            //Juice
            AudioController.instance.PlayAudio(noteShowSFX);
            PlayAnimation(SHOW_NOTE);
        }

        private void HideNote(){
            noteCanvas.SetActive(false);
            isNoteOpen = false;

            //Juice
            AudioController.instance.PlayAudio(noteHideSFX);
            PlayAnimation(HIDE_NOTE);
        }

        private void PlayAnimation(string animationName){
            NoteAnimator.Play(animationName, -1, 0);
        }
#endregion
    }
}
