using ColdClimb.Interactable;
using ColdClimb.Item;
using ColdClimb.Player;
using UnityEngine;
using UnityEngine.Events;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Interactable{
    public class InspectEnviroment : MonoBehaviour, IInteractable{
        [Header("Inspect System")]
        [SerializeField, TextArea(5,5)] private string onInspectText;
        [SerializeField] private AudioType onInspectAudio;
        
        [Header("Inspect Actions")]
        [SerializeField] private bool giveItem;
        [SerializeField] private ItemData itemToGive;

        [SerializeField] private bool triggerEventOnInspect;
        [SerializeField] private UnityEvent eventToTrigger;


        public string InteractionPrompt => "";


        public bool Interact(PlayerInteract player){
            //dispay string info in the text box
            return true;
        }
    }
}
