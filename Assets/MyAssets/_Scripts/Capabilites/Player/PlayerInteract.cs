using UnityEngine;
using UnityEngine.InputSystem;

// Our interact script that gets called when the game state is in the main game state
public class PlayerInteract : MonoBehaviour
{
    #region Variables
    [SerializeField] private float interactPointRadius;
    [SerializeField] private float interactDistance;

    [SerializeField] private LayerMask interactableLayerMask;

    private InputManager InputManager => ResourceLoader.InputManager;

    private readonly RaycastHit[] colliders = new RaycastHit[2];
    private int numFound;

    private bool canInteract = true;
    #endregion

    #region Setup
    private void OnEnable() {
        GameManager.OnGameStateChange += (state) => canInteract = state == GameState.MainGame;
    }
    
    private void OnDisable() {
        GameManager.OnGameStateChange -= (state) => canInteract = state == GameState.MainGame;
    }

    private void Start() {
        InputManager.ReturnInteractAction().started += TriggerInteract; 
    }

    private void OnDestroy() {
        InputManager.ReturnInteractAction().started -= TriggerInteract; 
    }
    #endregion

    #region Interact Action
    private void TriggerInteract(InputAction.CallbackContext context){
        if(!canInteract) return;
        numFound = Physics.SphereCastNonAlloc(transform.position, interactPointRadius, transform.forward, colliders, interactDistance, interactableLayerMask);
        if(numFound > 0 && colliders[0].collider.TryGetComponent(out IInteractable interactable) && interactable != null){
            interactable.Interact(this); 
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;

        Gizmos.DrawRay(transform.position, transform.forward * interactDistance);
    }
    #endregion
}
