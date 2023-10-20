using UnityEngine;

public class EnemyChargerAttackingState : EnemyState
{
    public EnemyChargerAttackingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    private Transform enemyCharacterTransform;
    private LayerMask playerLayerMask;
    private Rigidbody rb;

    private Transform PlayerTarget => stateMachine.PlayerTargetTransform;
    private float AttackSpeed => stateMachine.GetAttackSpeed();
    private float AttackRange => stateMachine.GetAttackRange();
    private float MovementSpeed => stateMachine.GetMoveSpeed();
    private float ChargingWindupTimer;
    private float ActivatingChargeTimer;

    private float attackTimer;
    private float chargeWindupTimer;
    private float activeChargeTimer;

    private bool hasAttacked = false;
    private bool transitionStarted = false;
    private bool isCharging = false;
    public override void EnterState(GameObject enemyObject, LayerMask playerLayer)
    {
        //state setup
        Debug.Log("Entering Attacking State");
        if (enemyCharacterTransform == null) enemyCharacterTransform = enemyObject.transform;
        if (playerLayerMask != playerLayer) playerLayerMask = playerLayer;
        if (rb == null) rb = enemyObject.GetComponent<Rigidbody>();
        //timers
        attackTimer = AttackSpeed;
        var randomChargingWindupTime = Random.Range(0.5f, 1.5f);
        ChargingWindupTimer = randomChargingWindupTime;
        chargeWindupTimer = ChargingWindupTimer;
        var randomActiveChargeTime = Random.Range(0.5f, 2f);
        ActivatingChargeTimer = randomActiveChargeTime;
        activeChargeTimer = ActivatingChargeTimer;
    }

    public override void ExitState()
    {
        hasAttacked = false;
        transitionStarted = false;
        isCharging = false;
    }

    public override void UpdateState()
    {
        LookAtPlayer();
        //telegraph the charge by waiting
        if (!hasAttacked && !isCharging) ChargeWindupTimer();
        //charge at the player for a set amount of time
        if (!hasAttacked && isCharging) 
        {
            ChargeAtPlayer();
            KnockupPlayer();
            ActiveChargeTimer();
        }
        //do a cooldown between attacks
        if (hasAttacked) AttackCooldownTimer();

        CheckForPlayerInAttackRadius();
    }

    #region Default Attack Methods
    private void LookAtPlayer()
    {
        //face the player excluding the y axis
        Vector3 playerDirection = PlayerTarget.position - enemyCharacterTransform.position;
        playerDirection.y = 0;
        enemyCharacterTransform.rotation = Quaternion.LookRotation(playerDirection);
    }

    private void CheckForPlayerInAttackRadius()
    {
        //go back into chase state if player gets out of attack range
        if (!Physics.CheckSphere(enemyCharacterTransform.position, AttackRange, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (!transitionStarted) ChangeIntoChasingState();
        }

        else if (transitionStarted)
        {
            transitionStarted = false;
            stateMachine.StopTransitionTimer();
        }
    }

    private void ChangeIntoChasingState()
    {
        transitionStarted = true;
        var randomTime = Random.Range(0.5f, 1f);
        stateMachine.StartTransitionTimer(randomTime, stateMachine.ChasingState); //transition to our chase state
    }
    #endregion

    private void ChargeAtPlayer()
    {
        //get the move direction toward the player
        Vector3 moveDirection = (PlayerTarget.position - enemyCharacterTransform.position).normalized;
        moveDirection.y = 0;
        //calculate the target position based on the direction and movement speed
        Vector3 targetPosition = enemyCharacterTransform.position + (MovementSpeed * 3) * Time.deltaTime * moveDirection;
        //travel to the point
        rb.MovePosition(targetPosition);
    }

    private void KnockupPlayer()
    {
        //if hitting the player apply knock up effect
        if (stateMachine.GetIsTouchingPlayer())
        {
            Rigidbody playerRb = PlayerTarget.GetComponent<Rigidbody>();
            playerRb.AddForce(1000 * enemyCharacterTransform.forward.normalized, ForceMode.Acceleration);

            //apply damage
            Health playerHealth = PlayerTarget.GetComponent<Health>();
            playerHealth.TakeDamage(1);
        }
    }

    #region Timers
    private void AttackCooldownTimer()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            attackTimer = AttackSpeed;
            hasAttacked = false;
        }
    }

    private void ChargeWindupTimer()
    {
        chargeWindupTimer -= Time.deltaTime;
        if (chargeWindupTimer <= 0)
        {
            chargeWindupTimer = ChargingWindupTimer;
            isCharging = true;
        }
    }

    private void ActiveChargeTimer()
    {
        activeChargeTimer -= Time.deltaTime;
        if (activeChargeTimer <= 0)
        {
            activeChargeTimer = ActivatingChargeTimer;
            hasAttacked = true;
            isCharging = false;
        }
    }
    #endregion
}
