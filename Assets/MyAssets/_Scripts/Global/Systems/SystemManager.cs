using ColdClimb.Global.SaveSystem;
using UnityEngine;

namespace ColdClimb.Global{
    // A singleton that handles our SO systems by being in our scene and defining the starting game state
    public class SystemManager : MonoBehaviour{
        #region Variables
        public static SystemManager Instance;
        public OptionsData OptionsData => optionsData;

        [SerializeField] private GameState startingGameState;
        [SerializeField] private OptionsData optionsData;
        #endregion

        #region Setup
        private void Awake(){
            if(Instance != null && Instance != this){
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start(){
            GameManager.UpdateGameState(startingGameState);
        }
        #endregion

    }
}
