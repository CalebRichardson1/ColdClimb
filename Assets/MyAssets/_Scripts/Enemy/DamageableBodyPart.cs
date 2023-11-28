using UnityEngine;

namespace ColdClimb.StateMachines{
    public class DamageableBodyPart : MonoBehaviour{
        private EnemyStateMachine enemyStateMachine;
        private BodyPartRank rank;

        public void SetupBodyPart(EnemyStateMachine stateMachine, BodyPartRank partRank){
            enemyStateMachine = stateMachine;
            rank = partRank;
        }

        public void Hit(int amount){
            switch (rank){
                case BodyPartRank.Good: enemyStateMachine.CurrentHealth.TakeDamage(amount * 2);
                    break;
                case BodyPartRank.Neutral: enemyStateMachine.CurrentHealth.TakeDamage(amount * 1);
                    break;
                case BodyPartRank.Weak: enemyStateMachine.CurrentHealth.TakeDamage(amount / 2);
                    break;
            }
        }
    }
}
