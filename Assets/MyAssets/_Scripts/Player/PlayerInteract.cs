using ColdClimb.Global;
using ColdClimb.Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    // Our interact script that gets called when the game state is in the main game state
    public class PlayerInteract : MonoBehaviour{
        [SerializeField] private float interactPointRadius;
        [SerializeField] private float interactDistance;

        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask interactLayer;
        [SerializeField] private GameObject crossHairIMG; 

        private InputManager InputManager => ResourceLoader.InputManager;

        private bool canInteract = true;
        private bool interactCooldown = false;

        private void OnEnable(){
            GameManager.OnGameStateChange += EvaluateGameState;
        }
        
        private void OnDisable(){
            GameManager.OnGameStateChange -= EvaluateGameState;
        }

        private void Start() {
            InputManager.ReturnInteractAction().started += TriggerInteract;
            crossHairIMG.SetActive(false); 
        }

        private void OnDestroy() {
            InputManager.ReturnInteractAction().started -= TriggerInteract; 
        }

        private void Update(){
            if(!canInteract){
                if(crossHairIMG.activeSelf) crossHairIMG.SetActive(false);
                return;  
            } 

            //Raycast with the interact range
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactDistance, ~playerLayer);
            
            if(hitInfo.collider != null){
                if(hitInfo.collider.TryGetComponent(out IInteractable interactable)){
                    crossHairIMG.SetActive(true);
                }
            }   
            else if(crossHairIMG.activeSelf){
                crossHairIMG.SetActive(false);
            }

        }

        private void EvaluateGameState(GameState state){
            if(state == GameState.MainGame){
                if(interactCooldown){
                    Invoke(nameof(InteractValid), 0.5f);
                    interactCooldown = false;
                    return;
                }

                InteractValid();
                return;
            }

            // See if the state is a dialogue state, and next time we switch back to main game, lock the interact function for a couple of seconds.
            if(state == GameState.DialogueScreen || state == GameState.QuestionScreen){
                interactCooldown = true;
            }

            canInteract = false;
        }

        private void TriggerInteract(InputAction.CallbackContext context){
            if(!canInteract) return;
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactDistance, ~playerLayer)){
                if(hitInfo.collider.TryGetComponent(out IInteractable interactable)){
                    interactable.Interact(this);
                }
            }
        }

        private void InteractValid(){
            canInteract = true;
        }

        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * interactDistance);
        }
    }
}
