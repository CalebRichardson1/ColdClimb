using System;
using System.Collections.Generic;
using ColdClimb.Global.SaveSystem;
using UnityEngine;

namespace ColdClimb.Global{
    // A singleton that handles our SO systems by being in our scene and defining the starting game state
    public class SystemManager : MonoBehaviour{

        public static SystemManager Instance;
        public OptionsData OptionsData => optionsData;

        [SerializeField] private GameState startingGameState;
        [SerializeField] private OptionsData optionsData;

        [SerializeField] private List<DataHolder> dataHolders;

        private void Awake(){
            if(Instance != null && Instance != this){
                Destroy(gameObject);
                return;
            }
            Instance = this;

            ResourceLoader.MainPlayerData.LoadValuesCallback += ResourceLoader.PlayerInventory.LoadData;

            IntializeDataHolders();
        }

        private void IntializeDataHolders(){
            for (int i = 0; i < dataHolders.Count; i++){
                dataHolders[i].Intialize();
            }
        }

        private void Start(){
            GameManager.UpdateGameState(startingGameState);
        }
    }
}
