using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoAttack : BossState
{
    // Holds states
    private IdleState idleState;
    private AttackManagerState attackState;

    // Duration of the wind-up phase
    [SerializeField] private float windUpDuration = 5f;
    [SerializeField] private float waitDuration = 3f;
    // Holds Wheather the boss is charging
    [SerializeField] private bool isCharging = false;
    [SerializeField] private bool playerHit = false;
    [SerializeField] private bool hasRun = false;
    public bool coneIndicator = false;
    // Agent charge speed
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 10f;
    [SerializeField] private float chargePushForce = 5f;
    [SerializeField] private float chargePushDuration = 1f;
    [SerializeField] private float stoppingDistance = 3f;
    public float rotationSpeed = 4;
    // Damage for attack
    [SerializeField] private float damage = 0f;
    // Holds charge trigger
    [SerializeField] private BoxCollider chargeDMGTrigger;

    public override void EnterState()
    {
        Debug.Log("Phase2");
        // Checks if the state is null
        if (idleState == null)
        {
            // Gets the connected state
            idleState = GetComponent<IdleState>();
        }
        // Checks if the state is null
        if (attackState == null)
        {
            // Gets the connected state
            attackState = GetComponent<AttackManagerState>();
        }

        attackState.isAttacking = false;
        coneIndicator = true;
        hasRun = false;
        if (stateMachine.BossType == BossSpawner.TYPE.Basilisk)
        {
            stateMachine.BossAnimator.SetBool("isCharging", true);
        }
        if (stateMachine.telegraphs[0].outerShape.gameObject.activeInHierarchy)
        {
            stateMachine.telegraphs[0].outerShape.gameObject.SetActive(false);
        }
        if (stateMachine.telegraphs[1].outerShape.gameObject.activeInHierarchy)
        {
            stateMachine.telegraphs[1].outerShape.gameObject.SetActive(false);
        }
        stateMachine.BossAnimator.SetBool("isDiving", false);
    }

    public override void ExitState()
    {
        // Turn off charge damage trigger
        stateMachine.BossAnimator.SetBool("isCharging", false);
        isCharging = false;
        agent.speed = stateMachine.BossSpeed;
        playerHit = false;
        chargeDMGTrigger.enabled = false;
        hasRun = false;
        coneIndicator = false;
        agent.isStopped = false;
        
    }

    public override void UpdateState()
    {
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (!isCharging && !hasRun && stateMachine.BossType == BossSpawner.TYPE.Chieftain)
        {
            StartCoroutine(WindUpAndCharge());

            hasRun = true;
        }
        if (!isCharging)
        {
            // Calculate the direction to the target
            Vector3 targetDirection = playerTransform.position - transform.position;
            targetDirection.y = 0f;
            // Calculate the rotation needed to face the target
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Smoothly interpolate the current rotation to the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    public IEnumerator StopCharging()
    {
        isCharging = false;
        agent.isStopped = true;
        chargeDMGTrigger.enabled = false;
        yield return new WaitForSeconds(waitDuration);
        agent.isStopped = false;
        agent.speed = stateMachine.BossSpeed;
        playerHit = false;
        stateMachine.ChangeState(attackState);
    }

    IEnumerator WindUpAndCharge()
    {
        agent.isStopped = true;
        agent.speed = chargeSpeed;
        stateMachine.BossAnimator.SetBool("isCharging", true);
        yield return new WaitForSeconds(windUpDuration);
        chargeDMGTrigger.enabled = true;
        agent.isStopped = false;
        isCharging = true;
        Vector3 newTarget = transform.position + transform.forward * chargeDistance;
        WalkTo(newTarget, stoppingDistance);
    }

    public float Damage
    {
        get { return damage; }
    }

    public float ChargePushForce
    {
        get { return chargePushForce; }
    }

    public float ChargePushDuration
    {
        get { return chargePushDuration; }
    }

    public bool PlayerHit
    {
        get { return playerHit; }
        set { playerHit = value; }
    }

    public bool ConeIdicator
    {
        get { return coneIndicator; }
        set { coneIndicator = value; }
    }
}
