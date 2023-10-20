using UnityEngine;

public class EnemyChasingState : EnemyState
{
    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private Transform enemyCharacterTransform;
    private LayerMask playerLayerMask;
    private Vector3 offset = new Vector3(0, 0.5f); //offset for raycast
    private RaycastHit hit;
    private Rigidbody rb;

    private EnemyType EnemyType => stateMachine.GetEnemyType();
    private Transform PlayerTarget => stateMachine.PlayerTargetTransform;
    private float MaxSeeRange => stateMachine.GetMaxSpotRange();
    private float MovementSpeed => stateMachine.GetMoveSpeed();
    private float AttackRange => stateMachine.GetAttackRange();
    private bool isChasing = true;
    private bool checkForPlayer = true;
    public override void EnterState(GameObject enemyObject, LayerMask playerLayer)
    {
        Debug.Log("Entered Chase State");
        //setting up variables
        if (enemyCharacterTransform == null) enemyCharacterTransform = enemyObject.transform;
        if (playerLayerMask != playerLayer) playerLayerMask = playerLayer;
        if (rb == null) rb = enemyObject.GetComponent<Rigidbody>();

        //to make sure that eventually the enemy will stop chasing if play is not getting into attack range
        var randomChaseTimer = Random.Range(10f, 20f);
        stateMachine.StartTransitionTimer(randomChaseTimer, stateMachine.PatrollingState);

        //get the players transform
        Physics.Raycast(enemyCharacterTransform.position + offset, enemyCharacterTransform.forward.normalized, out hit, MaxSeeRange, playerLayerMask, QueryTriggerInteraction.Ignore);
        stateMachine.SetPlayerTarget(hit.transform); 
        if (PlayerTarget == null) ChangeIntoRandomState();
    }

    public override void ExitState()
    {
        ResetXZRotation();
        isChasing = true;
        checkForPlayer = true;
    }

    public override void UpdateState()
    {
        if (isChasing) 
        {
            ChasePlayer();
            DetectPlayerInAttackRange();
        } 
        if(checkForPlayer) CheckPlayerInRange();
    }

    private void ResetXZRotation()
    {
        //get the current rotation of the enemy transform
        Vector3 currentEulerAngles = enemyCharacterTransform.rotation.eulerAngles;
        //reset the z rotation to 0
        currentEulerAngles.z = 0;
        currentEulerAngles.x = 0;
        //set the edited euler angles to a quaternion
        Quaternion newRotation = Quaternion.Euler(currentEulerAngles);
        //apply the new rotation quaternion
        enemyCharacterTransform.rotation = newRotation;
    }

    private void DetectPlayerInAttackRange()
    {
        if(Physics.CheckSphere(enemyCharacterTransform.position, AttackRange, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            ChangeIntoAttackState();
        }
    }
    private void CheckPlayerInRange()
    {
        //detect if the player gets out of line of sight then go back to a random state between stationary and partrolling
        if (!Physics.Raycast(enemyCharacterTransform.position + offset, enemyCharacterTransform.forward.normalized, MaxSeeRange, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            ChangeIntoRandomState();
        }
    }

    private void ChangeIntoAttackState()
    {
        stateMachine.StopTransitionTimer(); //stop the timer from going into patrolling
        switch (EnemyType) //change the attacking state depending on what type our enemy is
        {
            case EnemyType.BombThrower:
                break;
            case EnemyType.GravityManipulator:
                break;
            case EnemyType.Charger:
                stateMachine.TransitionToState(stateMachine.ChargerAttackingState);
                break;
        }
    }

    private void ChangeIntoRandomState()
    {
        stateMachine.StopTransitionTimer(); //stop the timer from going into patrolling
        var randomTime = Random.Range(0.5f, 1f);
        var randomState = Random.Range(0, 11);

        if (randomState > 6) stateMachine.StartTransitionTimer(randomTime, stateMachine.PatrollingState);
        else stateMachine.StartTransitionTimer(randomTime, stateMachine.StationaryState);

        //change these bools so this function doesn't get duplicated
        isChasing = false;
        checkForPlayer = false;

        //reset the X and Z rotation to avoid the enemy to be in a weird rotation while waiting
        ResetXZRotation();
    }

    //chase the player until in attack range with a speed multiplier
    private void ChasePlayer()
    {
        //face the player excluding the y axis
        Vector3 playerDirection = PlayerTarget.position - enemyCharacterTransform.position;
        playerDirection.y = 0;
        enemyCharacterTransform.rotation = Quaternion.LookRotation(playerDirection);
        //get the move direction toward the player
        Vector3 moveDirection = (PlayerTarget.position - enemyCharacterTransform.position).normalized;
        //remove y to prevent float upward
        moveDirection.y = 0;
        //calculate the target position based on the direction and movement speed
        Vector3 targetPosition = enemyCharacterTransform.position + (MovementSpeed * 2 ) * Time.deltaTime * moveDirection;
        //travel to the point
        rb.MovePosition(targetPosition);
    }
}

public enum EnemyType{
    BombThrower,
    GravityManipulator,
    Charger
}
