using ColdClimb.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    [RequireComponent(typeof(NavMeshAgent), typeof(Health))]
    public class EnemyStateMachine : StateMachine<EnemyStateMachine.EnemyState>, IHearer{
        // members
        public Health CurrentHealth => EnemyHealth;

        [SerializeField] private EnemyStats enemyStats;
        [SerializeField] private LayerMask playerLayer;

        [Header("Enemy Hitspots")]
        [SerializeField] private Collider[] goodSpots;
        [SerializeField] private Collider[] neutralSpots;
        [SerializeField] private Collider[] weakSpots;

        private NavMeshAgent NavMeshAgent => GetComponent<NavMeshAgent>();
        private Health EnemyHealth => GetComponent<Health>();
        private SearchState searchState;

        private Collider[] detectedTargets = new Collider[1];
        
        public enum EnemyState{
            Patrol,
            Searching,
            Chasing,
            Attacking
        }

#region Unity Functions
        private void Awake() {
            if(enemyStats == null) return;
            GenerateStateDictionary();
            Configure();
            CurrentState = States[EnemyState.Patrol];
        }

        private void OnDrawGizmosSelected() {
            if(enemyStats == null) return;

            // Detection circles
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyStats.immediateChaseRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyStats.softChaseRange);
        }
#endregion

#region Public Functions
        public void LocateTargets(){
            SeePlayer();
        }
        #endregion

        #region Private Functions
        private void Configure(){
            EnemyHealth.SetupHealth(enemyStats.health);

            // Generate Parts
            for (int i = 0; i < goodSpots.Length; i++){
                var bodyPart = goodSpots[i].gameObject.AddComponent<DamageableBodyPart>();
                bodyPart.SetupBodyPart(this, BodyPartRank.Good);
            }
            
            for (int i = 0; i < neutralSpots.Length; i++){
                var bodyPart = neutralSpots[i].gameObject.AddComponent<DamageableBodyPart>();
                bodyPart.SetupBodyPart(this, BodyPartRank.Neutral);
            }
            
            for (int i = 0; i < weakSpots.Length; i++){
                var bodyPart = weakSpots[i].gameObject.AddComponent<DamageableBodyPart>();
                bodyPart.SetupBodyPart(this, BodyPartRank.Weak);
            }
        }
        private void GenerateStateDictionary(){
            // Add the correct states to the dictionary
            switch (enemyStats.patrolType){
                case PatrolType.Homepoint: States.Add(EnemyState.Patrol, new PatrolHomePoint(EnemyState.Patrol, NavMeshAgent));
                    break;
                case PatrolType.FixedPoints: States.Add(EnemyState.Patrol, new PatrolFixedPoint(EnemyState.Patrol, NavMeshAgent));
                    break;
                case PatrolType.GoalOriented: States.Add(EnemyState.Patrol, new PatrolGoalOriented(EnemyState.Patrol, NavMeshAgent, enemyStats, this));
                    break;
                case PatrolType.Chaser: States.Add(EnemyState.Patrol, new PatrolChaser(EnemyState.Patrol, NavMeshAgent));
                    break;
            }

            States.Add(EnemyState.Searching, new SearchState(EnemyState.Searching, NavMeshAgent, enemyStats, this));
            searchState = (SearchState)States[EnemyState.Searching];
        }

        private void SeePlayer(){
            // Immediate Detection
            detectedTargets = Physics.OverlapSphere(transform.position, enemyStats.immediateChaseRange, playerLayer);
            if(detectedTargets.Length > 0){
                Debug.Log("Found Player!");
                return;    
            }

            // Soft Detection
            detectedTargets = Physics.OverlapSphere(transform.position, enemyStats.softChaseRange, playerLayer);
            if(detectedTargets.Length > 0){
                transform.LookAt(detectedTargets[0].transform);
                if(Physics.Raycast(transform.position, transform.forward, enemyStats.softChaseRange, playerLayer)){
                    Debug.Log("Found Player!");
                    return;
                }
            }
        }

        public void RespondToSound(ReactableSound sound){
            // if(CurrentState == States[EnemyState.Chasing] || CurrentState == States[EnemyState.Attacking]) return;

            // If the current search sound is higher priority over the incoming one, ignore the incoming one. 
            if(CurrentState == States[EnemyState.Searching] && searchState.currentPriority > sound.soundPriority) return;
            
            if(sound.soundType == SoundType.Interesting){
                //Chance not to investigate the noise, based on priority
                int chance = Random.Range(1, 2 * sound.soundPriority);
        
                if(chance < 3) return; 

                // Search State (SearchType = Investigate)
                TransitionToState(EnemyState.Searching);
                searchState.InjectSound(sound, SearchStatus.Investigating);
                return;
            }
        }
        #endregion
    }

    public enum BodyPartRank{
        Good,
        Neutral,
        Weak
    }

    public enum SearchStatus{
        None,
        Investigating,
        Hunting
    }
}
