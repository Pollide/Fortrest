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
    // Agent charge speed
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float maxDistFromPlayer = 3f;
    [SerializeField] private float chargeTimer = 0f;
    [SerializeField] private float chargeMaxTimer = 5f;
    [SerializeField] private float chargePushForce = 5f;
    [SerializeField] private float chargePushDuration = 1f;
    // Damage for attack
    [SerializeField] private float damage = 0f;
    // Holds charge trigger
    [SerializeField] private BoxCollider chargeDMGTrigger;

    public override void EnterState()
    {
        chargeTimer = 0f;
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
        chargeDMGTrigger.enabled = false;
    }

    public override void UpdateState()
    {
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (!isCharging)
        {
            StartCoroutine(WindUpAndCharge());
        }
        else
        {
            chargeTimer += Time.deltaTime;
        }

        if (isCharging && chargeTimer >= chargeMaxTimer)
        {
            StartCoroutine(StopCharging());
        }
        if (isCharging && Vector3.Distance(transform.position, playerTransform.position) <= maxDistFromPlayer)
        {
            // Stop charging when close to the target
            StartCoroutine(StopCharging());
        }

    }


    IEnumerator StopCharging()
    {
        Debug.Log("Stopping");
        isCharging = false;
        agent.isStopped = true;
        chargeDMGTrigger.enabled = false;
        yield return new WaitForSeconds(5);
        agent.isStopped = false;
        agent.speed = stateMachine.BossSpeed;
        playerHit = false;
        stateMachine.ChangeState(attackState);
    }

    IEnumerator WindUpAndCharge()
    {
        Debug.Log("Wind-up phase");
        agent.isStopped = true;
        agent.speed = chargeSpeed;
        yield return new WaitForSeconds(windUpDuration);
        chargeDMGTrigger.enabled = true;
        agent.isStopped = false;
        WalkTo(playerTransform.position);
        isCharging = true;
        Debug.Log("Charging!");
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
