using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;

    public EnemyState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState(GameObject enemyObject, LayerMask playerLayer) { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}
