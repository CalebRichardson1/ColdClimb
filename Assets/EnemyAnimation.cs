using ColdClimb.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    public class EnemyAnimation : MonoBehaviour{
        private Animator enemyAnimator;
        private NavMeshAgent navMeshAgent;
        private EnemyStateMachine enemyStateMachine;
        private Health enemyHealth;

        private const string SCREAM = "Scream";
        private const string FINISHEDCALL = "FinishedScream";
        private const string FINISHEDATTACK = "FinishedAttack";
        private const string DEATH = "Death";
        private const string ATTACK = "Attack";

        private void Awake() {
            TryGetComponent(out enemyAnimator);
            TryGetComponent(out navMeshAgent);
            TryGetComponent(out enemyStateMachine);
            TryGetComponent(out enemyHealth);
            enemyHealth.OnDieCallback += OnEnemyDeath;
        }

        private void OnDestroy() {
            enemyHealth.OnDieCallback -= OnEnemyDeath;
        }

        private void Update() {
            enemyAnimator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }

        public void StartCallAnimation(){
            enemyAnimator.Play(SCREAM);

            Invoke(nameof(FinishedCallAnimation), 1.33f);
        }

        public void StartAttackAnimation(){
            enemyAnimator.SetTrigger(FINISHEDATTACK);

            Invoke(nameof(FinishedAttackAnimation), 2.1f);
        }

        private void FinishedAttackAnimation(){
             enemyStateMachine.FinishedAttack();
             enemyAnimator.SetTrigger(FINISHEDATTACK);
        }

        private void FinishedCallAnimation(){
            enemyStateMachine.FinishedCall();
            enemyAnimator.SetTrigger(FINISHEDCALL);
        }

        private void OnEnemyDeath(){
            enemyAnimator.Play(DEATH);
        }
    }
}
