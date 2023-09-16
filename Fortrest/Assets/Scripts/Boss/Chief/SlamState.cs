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
    [SerializeField] private float slamRadius = 5f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private GameObject telegraph;
    private bool damageDone = false;


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

        telegraph.SetActive(true);
        agent.isStopped = true;
    }

    public override void ExitState()
    {
        //// Checks if state is populated
        //if (idleState != null)
        //{
        //    // Sets state to null
        //    idleState = null;
        //}
        //// Checks if state is populated
        //if (attackState != null)
        //{
        //    // Sets state to null
        //    attackState = null;
        //}
        telegraph.SetActive(false);
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

            telegraph.transform.position = playerTransform.position;
        }

        if (slamWaitTime >= slamDuration && !damageDone)
        {
            telegraph.GetComponent<BossTelegraph>().DoSlamDamage();
            damageDone = true;
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
}