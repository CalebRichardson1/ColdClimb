using System;
using System.Threading.Tasks;
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
        private bool finishedCall;
        private bool startedLooseTimer;
        private bool searchState;

        public ChaseGroupHunter(EnemyStateMachine.EnemyState key, NavMeshAgent agent, EnemyStats stats, EnemyStateMachine stateMachine) : base(key){
            navMeshAgent = agent;
            enemyStats = stats;
            enemyStateMachine = stateMachine;
            enemyTransform = agent.transform;
        }

        public override void EnterState(){
            Debug.Log("Chase State");
            target = enemyStateMachine.CurrentTarget;
            navMeshAgent.speed = enemyStats.runningMovementSpeed;
            if(finishedCall) return;

            var enemies = Physics.OverlapSphere(enemyTransform.position, enemyStats.softChaseRange + 25, enemyStateMachine.EnemyLayermask);
            foreach(Collider enemy in enemies){
                if(enemy.TryGetComponent(out EnemyStateMachine stateMachine) && stateMachine != enemyStateMachine){
                    stateMachine.AlertToTarget(target);
                }
            }
        }

        public override void ExitState(){
            finishedCall = false;
            startedLooseTimer = false;
            searchState = false;
            navMeshAgent.speed = enemyStats.walkingMovementSpeed;
        }

        public override void PauseState(bool isPaused){
            navMeshAgent.isStopped = isPaused;
        }

        public override void UpdateState(){
            if(!finishedCall) return;
            ChaseTarget();
            CheckValidChase();
        }

        private void CheckValidChase(){
            if(Physics.Linecast(enemyTransform.position, target.position, out RaycastHit hit, ~enemyStateMachine.IgnoreLayermask) && !startedLooseTimer){
                //start loose timer
                Debug.Log(hit.collider.gameObject);
                LooseTimer(1.3f);
            }
        }

        private async void LooseTimer(float waitSeconds){
            startedLooseTimer = true;
            await Task.Delay(1000 * (int)waitSeconds);
            if(Physics.Linecast(enemyTransform.position, target.position, ~enemyStateMachine.IgnoreLayermask)){
                searchState = true;
            }
            startedLooseTimer = false;
        }


        public override EnemyStateMachine.EnemyState GetNextState(){
            if(searchState){
                return EnemyStateMachine.EnemyState.Searching;
            }
            var distance = Vector3.Distance(enemyTransform.position, target.position);
            if(distance > enemyStats.minAttackRange && distance < enemyStats.maxAttackRange){
                return EnemyStateMachine.EnemyState.Attacking;
            }

            return EnemyStateMachine.EnemyState.Chasing;
        }

        public override void OnTriggerEnter(Collider other){
            
        }

        public override void OnTriggerExit(Collider other){
            
        }

        public override void OnTriggerStay(Collider other){
            
        }

        public void ChaseImmediate(bool state){
            finishedCall = state;
        }

        private void ChaseTarget(){
            navMeshAgent.SetDestination(target.position);
        }
    }
}
