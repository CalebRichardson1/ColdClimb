using SaveLoadSystem;
using UnityEngine;

// A singleton that handles our SO systems by being in our scene and defining the starting game state
public class SystemManager : MonoBehaviour
{
    #region Variables
    public static SystemManager Instance;
    [SerializeField] private GameState startingGameState;
    #endregion

    #region Setup
    private void Awake() {
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        GameManager.UpdateGameState(startingGameState);
    }

    public void LoadMainGame(){
        GameManager.UpdateGameState(GameState.MainGame);
    }
    #endregion
}
