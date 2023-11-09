using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Systems/New Input Manager", fileName = "NewInputManagerScriptableObject")]
public class InputManager : ScriptableObject
{
    private PlayerGroundCharacter playerControls;

    private void OnEnable() {
        playerControls ??= new PlayerGroundCharacter();
        playerControls.Enable();   
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement(){
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookDelta(){
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public InputAction ReturnSprintAction(){
        return playerControls.Player.Sprint;
    }

    public InputAction ReturnInteractAction(){
        return playerControls.Player.Interact;
    }

    public InputAction ReturnStatusAction(){
        return playerControls.Player.Status;   
    }

    public InputAction ReturnUseEquippedItemAction(){
        return playerControls.Player.UseEquippedItem;
    }

    public InputAction ReturnAltUseEquippedItemAction(){
        return playerControls.Player.AltUseEquippedItem;
    }

    public InputAction ReturnCancelAction(){
        return playerControls.UI.Cancel;
    }

    public InputAction ReturnReloadAction(){
        return playerControls.Player.Reload;
    }

    public InputAction ReturnPauseAction(){
        return playerControls.Player.PauseGame;
    }
}
