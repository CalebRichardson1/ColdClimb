using ColdClimb.Global;
using ColdClimb.Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    // Our interact script that gets called when the game state is in the main game state
    public class PlayerInteract : MonoBehaviour{
        [SerializeField] private float interactPointRadius;
        [SerializeField] private float interactDistance;

        [SerializeField] private LayerMask interactableLayerMask;

        private InputManager InputManager => ResourceLoader.InputManager;

        private readonly RaycastHit[] colliders = new RaycastHit[2];
        private int numFound;

        private bool canInteract = true;

        private void OnEnable(){
            GameManager.OnGameStateChange += (state) => canInteract = state == GameState.MainGame;
        }
        
        private void OnDisable(){
            GameManager.OnGameStateChange -= (state) => canInteract = state == GameState.MainGame;
        }

        private void Start() {
            InputManager.ReturnInteractAction().started += TriggerInteract; 
        }

        private void OnDestroy() {
            InputManager.ReturnInteractAction().started -= TriggerInteract; 
        }

        private void TriggerInteract(InputAction.CallbackContext context){
            if(!canInteract) return;
            numFound = Physics.SphereCastNonAlloc(transform.position, interactPointRadius, transform.forward, colliders, interactDistance, interactableLayerMask);
            if(numFound > 0 && colliders[0].collider.TryGetComponent(out IInteractable interactable) && interactable != null){
                interactable.Interact(this); 
            }
        }

        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * interactDistance);
        }
    }
}
