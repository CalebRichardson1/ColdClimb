using System;
using System.Collections.Generic;
using ColdClimb.Global;
using UnityEngine;

namespace ColdClimb.StateMachines{
    public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum{
        // members
        protected Dictionary<EState, BaseState<EState>> States = new();

        protected BaseState<EState> CurrentState;

        protected bool IsTransitioningState = false;
        protected bool IsPaused = false;
#region Unity Functions
        private void Start() {
            GameManager.OnGameStateChange += UpdateGameState;
            CurrentState.EnterState();
        }

        private void Update() {
            EState nextStateKey = CurrentState.GetNextState();

            if(IsTransitioningState) return;

            if(nextStateKey.Equals(CurrentState.StateKey)){
                CurrentState.UpdateState();
            }
            else{
                TransitionToState(nextStateKey);
            }
        }

        private void OnTriggerEnter(Collider other) {
            CurrentState.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other) {
            CurrentState.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other) {
            CurrentState.OnTriggerExit(other);
        }

        private void OnDestroy() {
            GameManager.OnGameStateChange -= UpdateGameState;
        }
#endregion

#region Public Functions

#endregion

#region Private Functions
        protected void TransitionToState(EState nextStateKey){
            IsTransitioningState = true;
            CurrentState.ExitState();
            CurrentState = States[nextStateKey];
            CurrentState.EnterState();
            IsTransitioningState = false;
        }

        private void UpdateGameState(GameState state){
            if(state == GameState.PauseMenu || state == GameState.GameOver){
                CurrentState.PauseState(true);
                return;
            }
            
            CurrentState.PauseState(false);
        }
#endregion
    }
}

