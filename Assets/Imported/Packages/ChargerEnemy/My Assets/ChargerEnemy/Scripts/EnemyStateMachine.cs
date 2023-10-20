using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyStateMachine : MonoBehaviour
{
    private const string PLAYER = "Player";

    [Header("Enemy Type")]
    [SerializeField] private EnemyType enemyType;
    [Header("Layer Mask Definitions")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [Header("Enemy Stats")]
    [SerializeField, Range(1f, 30f)] private float maxSpotRange;
    [SerializeField, Range(5f, 30f)] private float maxMoveDistance;
    [SerializeField, Range(1f, 30f)] private float attackRange;
    [SerializeField, Range(0.5f, 5f)] private float attackSpeed;
    [SerializeField, Range(5f, 50f)] private float moveSpeed;

    private EnemyState currentState;
    private EnemyStateMachine stateMachine;

    public Transform PlayerTargetTransform { get; private set; }

    public EnemyStationaryState StationaryState { get; private set; }
    public EnemyPatrollingState PatrollingState { get; private set; }
    public EnemyChasingState ChasingState { get; private set; }
    public EnemyChargerAttackingState ChargerAttackingState { get; private set; }

    private Coroutine transitionCoroutine;
    private bool isTouchingPlayer = false;
    private void Awake()
    {
        stateMachine = this;

        //would use lazy instantiate instead to save memory but this is simplier and faster for prototyping
        StationaryState = new EnemyStationaryState(stateMachine);
        PatrollingState = new EnemyPatrollingState(stateMachine);
        ChasingState = new EnemyChasingState(stateMachine);
        switch (enemyType)
        {
            case EnemyType.BombThrower:
                break;
            case EnemyType.GravityManipulator:
                break;
            case EnemyType.Charger:
                ChargerAttackingState = new EnemyChargerAttackingState(stateMachine);
                break;
        }
    }

    private void Start()
    {
        TransitionToState(StationaryState);
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    public void TransitionToState(EnemyState nextState)
    {
        if(currentState != null)
        {
            currentState.ExitState();
        }

        currentState = nextState;
        currentState.EnterState(gameObject, playerLayer);
    }

    #region Getters
    public float GetMaxSpotRange()
    {
        return maxSpotRange;
    }

    public LayerMask GetGroundLayerMask()
    {
        return groundLayer;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public bool GetIsTouchingPlayer()
    {
        return isTouchingPlayer;
    }
    #endregion

    #region Setters
    public void SetPlayerTarget(Transform target)
    {
        PlayerTargetTransform = target;
    }
    #endregion

    #region Functions For States
    public void StartTransitionTimer(float time, EnemyState nextState)
    {
        transitionCoroutine = StartCoroutine(StateTransitionTimer(time, nextState));
    }

    public void StopTransitionTimer()
    {
        StopCoroutine(transitionCoroutine);
        transitionCoroutine = null;
    }
    private IEnumerator StateTransitionTimer(float time, EnemyState nextState)
    {
        yield return new WaitForSeconds(time);
        TransitionToState(nextState);
    }

    public Vector3 GenerateRandomPoint()
    {
        Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * Random.Range(4, maxMoveDistance);
        Vector3 randomPoint = new Vector3(randomCirclePoint.x, 0, randomCirclePoint.y);
        return randomPoint;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(enemyType == EnemyType.Charger)
        {
            if(collision.collider.CompareTag(PLAYER)) isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (enemyType == EnemyType.Charger)
        {
            if (collision.collider.CompareTag(PLAYER)) isTouchingPlayer = false;
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        //for the line of sight ray
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * maxSpotRange;
        Vector3 offset = new Vector3(0, 0.5f);
        Gizmos.DrawRay(transform.position + offset, direction);
        //max movement sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxMoveDistance);
        //draw a wire sphere for the attack range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    #endregion
}
