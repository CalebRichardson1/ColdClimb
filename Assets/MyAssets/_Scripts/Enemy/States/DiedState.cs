using UnityEngine;

namespace ColdClimb.StateMachines{
    public class DiedState : BaseState<EnemyStateMachine.EnemyState>
    {
        public DiedState(EnemyStateMachine.EnemyState key) : base(key){

        }

        public override void EnterState(){
        }

        public override void ExitState(){
        }

        public override EnemyStateMachine.EnemyState GetNextState(){
            return EnemyStateMachine.EnemyState.Died;
        }

        public override void OnTriggerEnter(Collider other){
        }

        public override void OnTriggerExit(Collider other){
        }

        public override void OnTriggerStay(Collider other){
        }

        public override void PauseState(bool isPaused){
        }

        public override void UpdateState()
        {
            
        }
    }
}
