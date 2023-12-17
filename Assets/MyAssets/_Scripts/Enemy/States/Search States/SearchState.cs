using System.Threading.Tasks;
using ColdClimb.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    public class SearchState : BaseState<EnemyStateMachine.EnemyState>
    {
        // members
        public int currentPriority = 0;

        private NavMeshAgent navMeshAgent;
        private EnemyStats enemyStats;
        private EnemyStateMachine enemyStateMachine;
        private SearchStatus currentSearchStatus;
        private Transform enemyTransform;
        
        private Vector3 baseSearchPos;
        private bool isSearching = false;

        public SearchState(EnemyStateMachine.EnemyState key, NavMeshAgent agent, EnemyStats stats, EnemyStateMachine stateMachine) : base(key){
            navMeshAgent = agent;
            enemyStats = stats;
            enemyStateMachine = stateMachine;
            enemyTransform = navMeshAgent.transform;
        }

        public override void EnterState(){
            Debug.Log("Searching");
            baseSearchPos = enemyStateMachine.CurrentTarget.position;
            navMeshAgent.SetDestination(baseSearchPos);
            SearchTimer();
        }

        public override void ExitState(){
            currentSearchStatus = SearchStatus.None;
            isSearching = false;
        }

        public override EnemyStateMachine.EnemyState GetNextState(){
            enemyStateMachine.LocateTargets();
            if(isSearching) return EnemyStateMachine.EnemyState.Searching;
            
            return EnemyStateMachine.EnemyState.Patrol;
        }

        public override void OnTriggerEnter(Collider other){
            
        }

        public override void OnTriggerExit(Collider other){
            
        }

        public override void OnTriggerStay(Collider other){
            
        }

        public override void PauseState(bool isPaused){
            navMeshAgent.isStopped = isPaused;
        }

        public override void UpdateState(){
            if(navMeshAgent.pathPending) return;

            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f){
                if(currentSearchStatus == SearchStatus.Hunting){
                    SearchArea();
                }
            }
        }

        public void SearchForTarget(){
            switch (currentSearchStatus){
                case SearchStatus.None: return;
                case SearchStatus.Investigating: Investigate();
                    break;
                case SearchStatus.Hunting: Hunt();
                    break;
            }
        }

        public void InjectSound(ReactableSound sound, SearchStatus status){
            currentPriority = sound.soundPriority;
            currentSearchStatus = status;
            baseSearchPos = sound.posMade;

            SearchForTarget();
        }

        private async void SearchTimer(){
            isSearching = true;
            await Task.Delay(1000 * (int)Random.Range(enemyStats.minSearchTime, enemyStats.maxSearchTime + 1));
            isSearching = false;
        }

        private void Investigate(){
            navMeshAgent.SetDestination(baseSearchPos);
        }

        private void Hunt(){
            navMeshAgent.SetDestination(baseSearchPos);
        }

        private Vector3 RandomNavmeshLocation(float radius) {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += enemyTransform.position;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1)) {
                finalPosition = hit.position;            
            }
            return finalPosition;
        }
        
        private void SearchArea(){
            navMeshAgent.SetDestination(RandomNavmeshLocation(10));
        }
    }
}
