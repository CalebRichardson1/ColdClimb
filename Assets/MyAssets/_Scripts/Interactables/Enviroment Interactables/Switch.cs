using System;
using UnityEngine;

// Basic switch that the player can interact with
public class Switch : MonoBehaviour, IInteractable
{
    #region Variables
    public bool SwitchState => isActivated;

    [Header("Prompt Name")]
    [SerializeField] private string prompt;

    [Header("Switch Options")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool oneShot = false;

    public string InteractionPrompt => prompt;
    
    public Action<bool> OnSwitchChange;

    private bool startState;
    #endregion

    #region Setup
    private void Start() {
        startState = isActivated;
    }
    #endregion

    #region Interact Action
    public bool Interact(PlayerInteract player)
    {
        if(oneShot && isActivated != startState) return false;

        isActivated = !isActivated;
        OnSwitchChange?.Invoke(isActivated);
        return true;
    }
    #endregion
}
