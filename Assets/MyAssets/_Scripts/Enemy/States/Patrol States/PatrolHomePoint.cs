using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    public class PatrolHomePoint : BaseState<EnemyStateMachine.EnemyState>
    {
        private NavMeshAgent navMeshAgent;
        public PatrolHomePoint(EnemyStateMachine.EnemyState key, NavMeshAgent agent) : base(key){
            navMeshAgent = agent;
        }

        public override void EnterState(){
            
        }

        public override void ExitState(){
            
        }

        public override void UpdateState(){
            
        }

        public override EnemyStateMachine.EnemyState GetNextState(){
            return EnemyStateMachine.EnemyState.Patrol;
        }

        public override void OnTriggerEnter(Collider other){
            
        }

        public override void OnTriggerExit(Collider other){
            
        }

        public override void OnTriggerStay(Collider other){
            
        }

        public override void PauseState(bool isPaused){
            
        }
    }
}
