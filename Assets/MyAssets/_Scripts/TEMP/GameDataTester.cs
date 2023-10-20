using UnityEngine;

namespace SaveLoadSystem{
    /// <summary>
    /// Test class for testing the saving and loading
    /// </summary>

    public class GameDataTester : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool loadGame;
        public string InteractionPrompt => new string("");

        public bool Interact(PlayerInteract player)
        {
            if(loadGame){
                GameManager.UpdateGameState(GameState.LoadingGame);
            }
            else{
                GameManager.UpdateGameState(GameState.SavingGame);
            }

            return true;
        }
    }

}
