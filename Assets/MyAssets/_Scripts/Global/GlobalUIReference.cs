using TMPro;
using UnityEngine;

namespace ColdClimb.UI{
    public class GlobalUIReference : MonoBehaviour{
        [SerializeField] private GameObject noteCanvas;
        [SerializeField] private TMP_Text noteTextAreaUI;
        [SerializeField] private CombineSelector combineSelector;
        [SerializeField] private ScreenFader screenFader;
        [SerializeField] private DialogueController dialogueController;

        public static GlobalUIReference Instance;
        public static GameObject NoteCanvas;
        public static TMP_Text NoteTextAreaUI;
        public static CombineSelector CombineSelector;
        public static ScreenFader ScreenFader;
        public static DialogueController DialogueController;

        private void Awake() {
            if(Instance != null && Instance != this){
             Destroy(gameObject);
                return;
            } 

            Instance = this;

            NoteCanvas = noteCanvas;
            NoteTextAreaUI = noteTextAreaUI;
            CombineSelector = combineSelector;
            ScreenFader = screenFader;
            DialogueController = dialogueController;
        }

        private void Start() {
            NoteCanvas.SetActive(false);
        }
    }
}
