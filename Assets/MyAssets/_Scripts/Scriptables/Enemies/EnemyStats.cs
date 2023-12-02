using UnityEngine;

namespace ColdClimb.StateMachines{
    [CreateAssetMenu(menuName = "Enemy/New Enemy Stats", fileName = "NewEnemyStats"), System.Serializable]
    public class EnemyStats : ScriptableObject{
        // Enemy lore
        public string enemyName;
        public string enemyDescription;
        public EnemyType enemyType;

        // Base Stats
        public int health;
        public float attackDamage;
        public float attackSpeed;
        public float minAttackRange;
        public float maxAttackRange;
        public float walkingMovementSpeed;
        public float runningMovementSpeed;

        // Detection Stats
        public float targetRegistrationTime;
        public float maxSpotRange;
        public float immediateChaseRange;
        public float softChaseRange;
        public float minSearchTime;
        public float maxSearchTime;

        // Enemy definitions
        public PatrolType patrolType;
        public GoalType goalType;
        public ChaseType chaseType;
        public AttackType attackType;  
    }

    public enum PatrolType{
        Homepoint,
        FixedPoints,
        GoalOriented,
        Chaser
    }

    public enum GoalType{
        Food,
        Injured
    }

    public enum ChaseType{
        Lonewolf,
        GroupHunter,
        WatchfulEye
    }

    public enum AttackType{
        HitRun,
        Relentless,
        Grabber,
        Bomber
    }

    public enum EnemyType{
        Blightbloom
    }
}
