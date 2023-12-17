using System;
using ColdClimb.Generic;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using UnityEngine;
using UnityEngine.AI;

namespace ColdClimb.StateMachines{
    [RequireComponent(typeof(NavMeshAgent), typeof(Health))]
    public class EnemyStateMachine : StateMachine<EnemyStateMachine.EnemyState>{
        // members
        public string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }

        public LayerMask IgnoreLayermask => ignoreLayer;
        private ScenesData ScenesData => ResourceLoader.ScenesData;
        public Health CurrentHealth => EnemyHealth;
        public EnemyAnimation EnemyAnimation => enemyAnimation;
        public LayerMask EnemyLayermask => enemyLayer;
        public Transform CurrentTarget => currentTarget;

        [SerializeField] private EnemyStats enemyStats;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Enemy Hitspots")]
        [SerializeField] private Collider[] goodSpots;
        [SerializeField] private Collider[] neutralSpots;
        [SerializeField] private Collider[] weakSpots;

        [Header("Enemy Attack Colliders")]
        [SerializeField] private Collider[] attackColliders;

        private NavMeshAgent NavMeshAgent => GetComponent<NavMeshAgent>();
        private Health EnemyHealth => GetComponent<Health>();
        private SearchState searchState;
        private ChaseGroupHunter chaseGroupHunter;
        private EnemyAttackState attackState;

        private Collider[] detectedTargets = new Collider[1];
        private Ray losRay = new();

        private EnemyAnimation enemyAnimation;
        
        private Transform currentTarget = null;

        private bool hasDied;

        public enum EnemyState{
            Patrol,
            Searching,
            Chasing,
            Attacking,
            Died
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyStats.minAttackRange);
            Gizmos.DrawWireSphere(transform.position, enemyStats.maxAttackRange);
        }

        private void OnDestroy() {
            EnemyHealth.OnDieCallback -= OnDie;
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }
#endregion

#region Public Functions
        public void FinishedCall(){
            chaseGroupHunter.ChaseImmediate(true);
            NavMeshAgent.isStopped = false;
        }

        public void FinishedAttack(){
            chaseGroupHunter.ChaseImmediate(true);
            attackState.FinishedAttack = true;
        }

        public void AlertToTarget(Transform target){
            Debug.Log("Alerted!");
            if(CurrentState == States[EnemyState.Chasing] || CurrentState == States[EnemyState.Attacking]) return;
            currentTarget = target;
            chaseGroupHunter.ChaseImmediate(true);
            TransitionToState(EnemyState.Chasing);
        }
#endregion

#region Private Functions
        private void Configure(){
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;

            TryGetComponent(out enemyAnimation);

            EnemyHealth.OnDieCallback += OnDie;

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

        private void SaveState(){
            if(ScenesData.CurrentSceneData.EnemiesDied.ContainsKey(id)){
                ScenesData.CurrentSceneData.EnemiesDied.Remove(id);
            }
            ScenesData.CurrentSceneData.EnemiesDied.Add(id, hasDied);
        }

        private void LoadData(){
            ScenesData.CurrentSceneData.EnemiesDied.TryGetValue(id, out hasDied);
            if(hasDied){
                Destroy(gameObject);
            }
        }

        private void OnDie(){
            foreach (var collider in goodSpots){
                collider.enabled = false;
            }

            foreach(var collider in neutralSpots){
                collider.enabled = false;
            }
            
            foreach(var collider in weakSpots){
                collider.enabled = false;
            }

            hasDied = true;
            TransitionToState(EnemyState.Died);
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

            switch (enemyStats.chaseType)
            {
                case ChaseType.Lonewolf:
                    break;
                case ChaseType.GroupHunter: States.Add(EnemyState.Chasing, new ChaseGroupHunter(EnemyState.Chasing, NavMeshAgent, enemyStats, this));
                                            chaseGroupHunter = (ChaseGroupHunter)States[EnemyState.Chasing];                    
                    break;
                case ChaseType.WatchfulEye:
                    break;
            }

            States.Add(EnemyState.Attacking, new EnemyAttackState(EnemyState.Attacking, NavMeshAgent, enemyStats, this, attackColliders));

            attackState = (EnemyAttackState)States[EnemyState.Attacking];

            States.Add(EnemyState.Died, new DiedState(EnemyState.Died));
        }

        public EnemyState LocateTargets(){
            // LOS Detection
            losRay.origin = transform.position;
            losRay.direction = transform.forward;

            // Close Range Detection
            detectedTargets = Physics.OverlapSphere(transform.position, enemyStats.softChaseRange, playerLayer);
            if(detectedTargets.Length > 0){
                //Smoothly look at the detected target
                Vector3 lookPos = detectedTargets[0].transform.position - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
                NavMeshAgent.isStopped = true;
            }

            if(TargetInLOS()){
                //Chase State
                if(chaseGroupHunter != null){
                    //Start the call animation
                    enemyAnimation.StartCallAnimation();
                }
                return EnemyState.Chasing;
            }
            else{
                return EnemyState.Patrol;
            }

        }

        private bool TargetInLOS(){
            Debug.DrawRay(transform.position, transform.forward * enemyStats.maxSpotRange, Color.blue);
            if (Physics.Raycast(losRay, out RaycastHit hit, enemyStats.maxSpotRange, ~enemyLayer)){
                // Hate tags
                if(currentTarget = hit.collider.CompareTag("Player") ? hit.collider.transform : null){
                    return true;
                }
            }
            if(currentTarget != null) currentTarget = null;
            return false;
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
