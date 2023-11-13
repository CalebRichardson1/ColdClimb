using ColdClimb.Player;

namespace ColdClimb.Interactable{
    public interface IInteractable
    {
        public string InteractionPrompt{ get; }

        public bool Interact(PlayerInteract player);    
    }
}
