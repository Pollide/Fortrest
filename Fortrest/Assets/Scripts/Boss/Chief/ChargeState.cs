using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeState : BossState
{
    // Holds states
    private IdleState idleState;
    private AttackState attackState;

    // Duration of the wind-up phase
    [SerializeField] private float windUpDuration = 5f;
    // Holds Wheather the boss is charging
    [SerializeField] private bool isCharging = false;
    [SerializeField] private bool playerHit = false;
    [SerializeField] private bool hasRun = false;
    // Agent charge speed
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float maxDistFromPlayer = 3f;
    [SerializeField] private float chargePushForce = 5f;
    [SerializeField] private float chargePushDuration = 1f;
    // Damage for attack
    [SerializeField] private float damage = 0f;
    // Holds charge trigger
    [SerializeField] private BoxCollider chargeDMGTrigger;

    public override void EnterState()
    {
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
            attackState = GetComponent<AttackState>();
        }
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
    }

    public override void UpdateState()
    {
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (!isCharging && !hasRun)
        {
            StartCoroutine(WindUpAndCharge());
            hasRun = true;
        }
    }


    public IEnumerator StopCharging()
    {
        isCharging = false;
        agent.isStopped = true;
        chargeDMGTrigger.enabled = false;
        yield return new WaitForSeconds(1);
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
        stateMachine.BossAnimator.SetBool("attacking", false);
        yield return new WaitForSeconds(windUpDuration);
        chargeDMGTrigger.enabled = true;
        agent.isStopped = false;
        isCharging = true;
        WalkTo(playerTransform.position);
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
}
