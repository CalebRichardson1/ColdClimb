using ColdClimb.Global;
using ColdClimb.UI;
using UnityEngine;

namespace ColdClimb.Generic{
    public class DialogueTrigger : MonoBehaviour{
        [SerializeField] private Dialogue onTriggerDialogue;

        private void OnTriggerEnter(Collider other) {
            if(!other.CompareTag("Player") || GameManager.CurrentState != GameState.MainGame) return;

            GlobalUIReference.DialogueController.StartDialogue(onTriggerDialogue);
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
