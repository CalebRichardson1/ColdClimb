using System;
using ColdClimb.Player;
using UnityEngine;

namespace ColdClimb.Interactable{
    // Basic switch that the player can interact with
    public class Switch : MonoBehaviour, IInteractable{
        public bool SwitchState => isActivated;

        [Header("Prompt Name")]
        [SerializeField] private string prompt;

        [Header("Switch Options")]
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool oneShot = false;

        public string InteractionPrompt => prompt;
        
        public Action<bool> OnSwitchChange;

        private bool startState;

        private void Start(){
            startState = isActivated;
        }

        public bool Interact(PlayerInteract player){
            if(oneShot && isActivated != startState) return false;

            isActivated = !isActivated;
            OnSwitchChange?.Invoke(isActivated);
            return true;
        }
    }
}