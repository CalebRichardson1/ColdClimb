using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    public class ChaseGroupHunter : BaseState<EnemyStateMachine.EnemyState>{
        //members
        private NavMeshAgent navMeshAgent;
        private EnemyStats enemyStats;
        private EnemyStateMachine enemyStateMachine;
        private Transform enemyTransform;

        private Transform target;

        public ChaseGroupHunter(EnemyStateMachine.EnemyState key, NavMeshAgent agent, EnemyStats stats, EnemyStateMachine stateMachine) : base(key){
            navMeshAgent = agent;
            enemyStats = stats;
            enemyStateMachine = stateMachine;
            enemyTransform = agent.transform;
        }

        public override void EnterState(){
            Debug.Log("Chase State");
        }

        public override void ExitState(){
            
        }

        public override void PauseState(bool isPaused){
            
        }

        public override void UpdateState(){
            
        }

        public override EnemyStateMachine.EnemyState GetNextState(){
            return EnemyStateMachine.EnemyState.Chasing;
        }

        public override void OnTriggerEnter(Collider other){
            
        }

        public override void OnTriggerExit(Collider other){
            
        }

        public override void OnTriggerStay(Collider other){
            
        }

        public void ChaseImmediate(bool state){

        }

        private void NotfiyGroup(){

        }

        private void ChaseTarget(){

        }

        private void ReceiveCall(){
            
        }
    }
}
