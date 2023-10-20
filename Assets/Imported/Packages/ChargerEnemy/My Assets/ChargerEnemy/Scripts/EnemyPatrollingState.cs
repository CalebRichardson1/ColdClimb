using UnityEngine;

public class EnemyPatrollingState : EnemyState
{
    public EnemyPatrollingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private Transform enemyCharacterTransform;
    private Rigidbody rb;
    private LayerMask playerLayerMask;
    private LayerMask GroundLayerMask => stateMachine.GetGroundLayerMask();

    private Vector3 randomPatrolPoint;
    private Vector3 offset = new Vector3(0, 0.5f); //offset for raycast

    private float MaxSeeRange => stateMachine.GetMaxSpotRange(); //get the enemies max spot range
    private float MovementSpeed => stateMachine.GetMoveSpeed();

    private int patrolPointsGenerated = 0;
    private bool moveTowardPoint = false;
    private bool isAtPoint = false;
    public override void EnterState(GameObject enemyObject, LayerMask playerLayer)
    {
        if (enemyCharacterTransform == null) enemyCharacterTransform = enemyObject.transform; //set the reference to our enemy character game object
        if (playerLayerMask != playerLayer) playerLayerMask = playerLayer; //set the reference to the player layermask
        if (rb == null) rb = enemyObject.GetComponent<Rigidbody>();

        GeneratePatrolPoint();
    }

    private void GeneratePatrolPoint()
    {
        patrolPointsGenerated++;
        //get a random point a minimun of 5 units away
        randomPatrolPoint = stateMachine.GenerateRandomPoint() + enemyCharacterTransform.position;
        //detect if that point has ground to stand on and there are no walls between the enemy and the point
        CheckRandomPosition();
    }

    private void CheckRandomPosition()
    {
        if(Physics.Raycast(randomPatrolPoint, -enemyCharacterTransform.up, GroundLayerMask))
        {
            if (!Physics.Linecast(enemyCharacterTransform.position, randomPatrolPoint))
            {
                moveTowardPoint = true;
            }
            else GeneratePatrolPoint();
        }
        else GeneratePatrolPoint();
    }

    public override void ExitState()
    {
        moveTowardPoint = false;
        isAtPoint = false;
        patrolPointsGenerated = 0;
    }

    public override void UpdateState()
    {
        //if a valid point is found start move toward it
        if (moveTowardPoint && !isAtPoint) Patrol();
        //randomly select between stationary state or patrol another point once at the selected random patrol point
        if (isAtPoint) SelectNextState();
        //look for the player
        LookForPlayer();
        //draw gizmos in the scene
        DrawingGizmos();
    }

    //selects random state to go into
    private void SelectNextState()
    {
        var randomTime = Random.Range(0.5f, 1f);
        var randomState = Random.Range(0, 11);

        if (randomState > 4) stateMachine.StartTransitionTimer(randomTime, stateMachine.PatrollingState);
        else stateMachine.StartTransitionTimer(randomTime, stateMachine.StationaryState);

        isAtPoint = false;
        moveTowardPoint = false;
    }

    //constantly looking for the player
    private void LookForPlayer()
    {
        if (Physics.Raycast(enemyCharacterTransform.position + offset, enemyCharacterTransform.forward.normalized, MaxSeeRange, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            stateMachine.StopTransitionTimer(); //stop the transition timer if waiting at patrol point
            stateMachine.TransitionToState(stateMachine.ChasingState); //transition to the chasing state
        }
    }

    private void Patrol()
    {
        //look toward the point
        enemyCharacterTransform.LookAt(randomPatrolPoint);

        //get the move direction
        Vector3 moveDirection = (randomPatrolPoint - enemyCharacterTransform.position).normalized;
        //calculate the target position based on the direction and movement speed
        Vector3 targetPosition = enemyCharacterTransform.position + moveDirection * MovementSpeed * Time.deltaTime;
        //travel to the point
        rb.MovePosition(targetPosition);
        
        //detect if enemy is near patrol point
        DetectAtPatrolPoint();
    }

    private void DetectAtPatrolPoint()
    {
        float dist = Vector3.Distance(enemyCharacterTransform.position, randomPatrolPoint);

        if (dist <= 1f) isAtPoint = true; 
    }

    private void DrawingGizmos()
    {
        Debug.DrawRay(randomPatrolPoint, -enemyCharacterTransform.transform.up * 2);
        Debug.DrawLine(enemyCharacterTransform.transform.position, randomPatrolPoint);
    }
}
