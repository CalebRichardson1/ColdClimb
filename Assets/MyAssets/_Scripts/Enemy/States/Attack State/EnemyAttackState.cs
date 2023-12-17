using ColdClimb.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    public class EnemyAttackState : BaseState<EnemyStateMachine.EnemyState>{
        //members
        public bool FinishedAttack = false;

        private NavMeshAgent navMeshAgent;
        private EnemyStats enemyStats;
        private EnemyStateMachine enemyStateMachine;
        private Transform enemyTransform;
        
        private Collider[] attackColliders;

        public EnemyAttackState(EnemyStateMachine.EnemyState key, NavMeshAgent agent, EnemyStats stats, EnemyStateMachine stateMachine, Collider[] colliders) : base(key){
            navMeshAgent = agent;
            enemyStats = stats;
            enemyStateMachine = stateMachine;
            enemyTransform = agent.transform;
            attackColliders = colliders;
        }

        public override void EnterState(){
            navMeshAgent.isStopped = true;
            foreach(Collider collider in attackColliders){
                collider.enabled = true;
                collider.TryGetComponent(out AttackTrigger attackTrigger);
                if(attackTrigger == null){
                    attackTrigger = collider.AddComponent<AttackTrigger>();
                }
                attackTrigger.HitPlayerEvent += DamagePlayer;
                //Trigger Attack Animation
                enemyStateMachine.EnemyAnimation.StartAttackAnimation();
                FinishedAttack = false;
            }
        }

        public override void ExitState(){
            navMeshAgent.isStopped = false;
            foreach(Collider collider in attackColliders){
                collider.enabled = false;
                collider.TryGetComponent(out AttackTrigger attackTrigger);
                if(attackTrigger == null){
                    attackTrigger = collider.AddComponent<AttackTrigger>();
                }
                attackTrigger.HitPlayerEvent -= DamagePlayer;
                FinishedAttack = true;
            }
        }

        public override void UpdateState(){
            //Smoothly look at the target
            Vector3 lookPos = enemyStateMachine.CurrentTarget.transform.position - enemyTransform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, rotation, 0.15f);
            
            if(FinishedAttack){
                Debug.Log("Attacked");
                //Trigger Attack Animation
                enemyStateMachine.EnemyAnimation.StartAttackAnimation();
                FinishedAttack = false;
            }
        }

        private void DamagePlayer(Health playerHealth){
            playerHealth.TakeDamage((int)enemyStats.attackDamage);
        }

        public override EnemyStateMachine.EnemyState GetNextState(){
            if(!FinishedAttack) return EnemyStateMachine.EnemyState.Attacking;

            var distance = Vector3.Distance(enemyTransform.position,  enemyStateMachine.CurrentTarget.transform.position);
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

        public override void PauseState(bool isPaused){
            
        }

    }
}
