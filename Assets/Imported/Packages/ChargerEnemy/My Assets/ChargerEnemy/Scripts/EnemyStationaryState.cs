using UnityEngine;

public class EnemyStationaryState : EnemyState
{
    //constructor
    public EnemyStationaryState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private Transform enemyCharacterTransform;
    private LayerMask playerLayerMask;
    private Vector3 offset = new Vector3(0, 0.5f); //offset for raycast

    private float randomRotateSpeed;
    private float MaxSeeRange => stateMachine.GetMaxSpotRange(); //get the enemies max spot range

    public override void EnterState(GameObject enemyObject, LayerMask playerLayer)
    {
        //transfer to patrolling state after a random amount of time
        if (enemyCharacterTransform == null) enemyCharacterTransform = enemyObject.transform; //set the reference to our in-game enemy object
        if (playerLayerMask != playerLayer) playerLayerMask = playerLayer; //set the reference to the player layermask

        randomRotateSpeed = Random.Range(30f, 60f) * Time.deltaTime;
        var randomTime = Random.Range(2f, 10f);

        stateMachine.StartTransitionTimer(randomTime, stateMachine.PatrollingState);
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        //look for the player using rays from enemyObject
        LookForPlayer();
        //rotate around randomly to find the player
        RotateRandomly();
    }

    private void RotateRandomly()
    {
        enemyCharacterTransform.Rotate(Vector3.up, randomRotateSpeed);
    }

    private void LookForPlayer()
    {
        if (Physics.Raycast(enemyCharacterTransform.position + offset, enemyCharacterTransform.forward.normalized, MaxSeeRange, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            stateMachine.StopTransitionTimer(); //stop the timer from going into patrolling
            stateMachine.TransitionToState(stateMachine.ChasingState); //transition to the chasing state
        }
    }
}
