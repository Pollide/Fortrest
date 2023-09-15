using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamState : BossState
{
    // Holds states
    private IdleState idleState;
    private AttackState attackState;
    // Slam wait time
    private float slamWaitTime = 0f;
    [SerializeField] private float slamDuration = 5f;
    [SerializeField] private GameObject telegraph;

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
        // Checks if state is populated
        if (idleState != null)
        {
            // Sets state to null
            idleState = null;
        }
        // Checks if state is populated
        if (attackState != null)
        {
            // Sets state to null
            attackState = null;
        }
    }

    public override void UpdateState()
    {
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (slamWaitTime < slamDuration)
        {
            slamWaitTime += Time.deltaTime;
        }
    }

    public float SlamDuration
    {
        get { return slamDuration; }
    }

    public float SlamWaitTime
    {
        get { return slamWaitTime; }
    }
}