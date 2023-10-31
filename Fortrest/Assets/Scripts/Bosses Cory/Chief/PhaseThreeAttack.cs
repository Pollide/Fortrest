using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseThreeAttack : BossState
{
    // Holds states
    private IdleState idleState;
    private AttackManagerState attackState;
    // Slam wait time
    private float slamWaitTime = 0f;
    [SerializeField] private float slamDuration = 5f;
    [SerializeField] private float slamRadius = 5f;
    //  [SerializeField] private float slamWaitAfterIndicator = 2f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private GameObject telegraph;
    [SerializeField] private TakeDamageTrigger trigger;
    [SerializeField] private bool damageDone = false;
    [SerializeField] private bool hasJumped = false;
    public bool telegraphBool = false;


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
            attackState = GetComponent<AttackManagerState>();
        }

        agent.isStopped = true;
        stateMachine.BossAnimator.SetBool("isJumping", true);
        slamWaitTime = 0f;
        damageDone = false;
        telegraphBool = true;
    }

    public override void ExitState()
    {
        damageDone = true;
        slamWaitTime = 0f;
        stateMachine.BossAnimator.SetBool("isJumping", false);
        stateMachine.BossAnimator.SetBool("isDiving", true);
        hasJumped = false;
    }

    public override void UpdateState()
    {
        if (stateMachine.IsDead)
        {
            telegraphBool = false;
            return;
        }
        if (!PlayerInArena(stateMachine.ArenaSize) && !stateMachine.BossAnimator.GetBool("isJumping") && !stateMachine.BossAnimator.GetBool("isDiving"))
        {
            stateMachine.ChangeState(idleState);
        }

        if (slamWaitTime < slamDuration)
        {
            slamWaitTime += Time.deltaTime;

            telegraph.transform.position = new(playerTransform.position.x, playerTransform.position.y - 1, playerTransform.position.z);
        }

        if (hasJumped)
        {
            transform.position = telegraph.transform.position;
        }

        if (slamWaitTime >= slamDuration && !damageDone)
        {
            stateMachine.BossAnimator.SetBool("isJumping", false);
            stateMachine.BossAnimator.SetBool("isDiving", true);
            if (telegraph.activeSelf)
            {
                damageDone = true;
            }
        }

        if (trigger)
        {
            if (stateMachine.BossAnimator.GetBool("isJumping") && trigger.enabled)
            {
                trigger.enabled = false;
            }
            else if (!stateMachine.BossAnimator.GetBool("isJumping") && !trigger.enabled)
            {
                trigger.enabled = true;
            }
        }

        if (damageDone)
        {
            stateMachine.ChangeState(idleState);
        }
    }

    public float SlamDuration
    {
        get { return slamDuration; }
    }

    public float SlamRadius
    {
        get { return slamRadius; }
    }

    public float Damage
    {
        get { return damage; }
    }

    public float SlamWaitTime
    {
        get { return slamWaitTime; }
    }

    public AttackManagerState StateAttack
    {
        get { return attackState; }
    }

    public bool HasJumped
    {
        get { return hasJumped; }
        set { hasJumped = value; }
    }
}