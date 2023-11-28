using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    public class PatrolGoalOriented : BaseState<EnemyStateMachine.EnemyState>{
        private NavMeshAgent navMeshAgent;
        private EnemyStats enemyStats;
        private EnemyStateMachine enemyStateMachine;

        private float searchRadius = 25f;

        private Vector3 currentGoalPos;
        private Transform enemyTransform;

        private bool isWaiting;

        public PatrolGoalOriented(EnemyStateMachine.EnemyState key, NavMeshAgent agent, EnemyStats stats, EnemyStateMachine stateMachine) : base(key){
            navMeshAgent = agent;
            enemyStats = stats;
            enemyStateMachine = stateMachine;
            enemyTransform = navMeshAgent.transform;
        }

        public override void EnterState(){
            Debug.Log("Patrolling");
            navMeshAgent.speed = enemyStats.walkingMovementSpeed;
            MoveEnemy();
        }

        public override void ExitState(){
            currentGoalPos = Vector3.zero;
        }

        public override void UpdateState(){
            if(navMeshAgent.pathPending || isWaiting) return;

            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f){
                var random = Random.Range(1, 11);
                if(random < 5){
                    MoveEnemy();
                }
                else{
                    EnemyWaiting(Random.Range(enemyStats.minSearchTime, enemyStats.maxSearchTime + 1));
                }
            }
        }

        public override void PauseState(bool isPaused){
            navMeshAgent.isStopped = isPaused;
        }

        public override EnemyStateMachine.EnemyState GetNextState(){
            enemyStateMachine.LocateTargets();
            return EnemyStateMachine.EnemyState.Patrol;
        }

        public override void OnTriggerEnter(Collider other){
            
        }

        public override void OnTriggerExit(Collider other){
            
        }

        public override void OnTriggerStay(Collider other){
            
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

        private void MoveEnemy(){
            currentGoalPos = RandomNavmeshLocation(searchRadius);
            navMeshAgent.SetDestination(currentGoalPos);
        }

        private async void EnemyWaiting(float waitSeconds){
            isWaiting = true;
            await Task.Delay(1000 * (int)waitSeconds);
            isWaiting = false;
        }
    }
}
