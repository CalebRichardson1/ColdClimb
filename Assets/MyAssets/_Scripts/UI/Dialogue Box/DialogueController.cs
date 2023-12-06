using System;
using System.Collections;
using System.Collections.Generic;
using ColdClimb.Global;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ColdClimb.UI{
    public class DialogueController : MonoBehaviour{
        //members
        [Header("Standard UI References")]
        [SerializeField] private GameObject dialogueUI;
        [SerializeField] private TMP_Text standardDialogueText;

        [Header("Question UI References")]
        [SerializeField] private GameObject questionUI;
        [SerializeField] private TMP_Text questionDialogueText;
        [SerializeField] private Button answerOneButton;
        [SerializeField] private TMP_Text answerOneText;
        [SerializeField] private Button answerTwoButton;
        [SerializeField] private TMP_Text answerTwoText;

        private Queue<string> sentences;
        private WaitForSecondsRealtime printTime;

        private TMP_Text currentTextObject;

        private bool isPrinting = false;
        private bool showedAnswer = false;
        private string currentSentence;
        private Dialogue currentDialogue;
        private UnityEvent answerOneAction;
        private UnityEvent answerTwoAction;

#region Unity Functions
        private void Start() {
            dialogueUI.SetActive(false);

            sentences = new Queue<string>();
            printTime = new WaitForSecondsRealtime(0.03f);
        }
#endregion

#region Public Functions
        public void StartDialogue(Dialogue dialogue){

            sentences.Clear();

            foreach (string sentence in dialogue.sentences){
                sentences.Enqueue(sentence);
            }

            dialogueUI.SetActive(true);

            currentDialogue = dialogue;

            if(dialogue.isQuestion){
                showedAnswer = false;
                standardDialogueText.enabled = false;
                questionUI.SetActive(true);
                answerOneButton.gameObject.SetActive(false);
                answerTwoButton.gameObject.SetActive(false);

                GameManager.UpdateGameState(GameState.QuestionScreen);
                currentTextObject = questionDialogueText;
            }
            else{
                standardDialogueText.enabled = true;
                questionUI.SetActive(false);
                GameManager.UpdateGameState(GameState.DialogueScreen);
                currentTextObject = standardDialogueText;
            }

            DisplayNextSentence();
        }

        public void AnswerOneAction(){
            answerOneAction?.Invoke();
        }

        public void AnswerTwoAction(){
            answerTwoAction?.Invoke();
        }

        public void DisplayNextSentence(){
            if(isPrinting){
                StopAllCoroutines();
                currentTextObject.text = currentSentence;
                isPrinting = false;
                if(currentDialogue.isQuestion){
                    ShowAnswers();
                }
                return;
            }

            if(sentences.Count == 0){
                EndDialogue();
                return;
            }

            currentSentence = sentences.Dequeue();
            StartCoroutine(TypeSentence(currentSentence, 0.02f));
        }

        public void EndDialogue(){
            if(currentDialogue.isQuestion && !showedAnswer){
                ShowAnswers();
                return;
            }

            dialogueUI.SetActive(false);
            GameManager.UpdateGameState(GameState.MainGame);
        }
        #endregion

        #region Private Functions


        private IEnumerator TypeSentence(string sentence, float typeSpeed){
            isPrinting = true;
            printTime.waitTime = typeSpeed;
            currentTextObject.text = "";
            foreach(char letter in sentence.ToCharArray()){
                currentTextObject.text += letter;

                yield return printTime;
            }
            isPrinting = false;
            // If the dialogue is a question then show the answers
            if(currentDialogue.isQuestion){
                ShowAnswers();
            }
        }

        private void ShowAnswers(){
            // Update Buttons
            answerOneText.text = currentDialogue.answerOneText;
            answerTwoText.text = currentDialogue.answerTwoText;

            // Show the buttons
            answerOneButton.gameObject.SetActive(true);
            answerTwoButton.gameObject.SetActive(true);

            // Update the button events
            answerOneAction = currentDialogue.answerOneEvent;
            answerTwoAction = currentDialogue.answerTwoEvent;

            // Move the cursor
            MenuSelector.Instance.SetDefaultSelectedObject(answerOneButton.transform);

            MenuSelector.Instance.SetIsValid(true);
            showedAnswer = true;
        }
        #endregion
    }
}
