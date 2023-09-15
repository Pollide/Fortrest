using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BossState
{
    // Holds next state
    private AttackState attackState;
    private float resetTimer = 0f;
    [SerializeField] private float resetTimerDuration;


    public override void EnterState()
    {
        // Checks if the attack state is null
        if (attackState == null)
        {
            // Gets the connected attack state
            attackState = GetComponent<AttackState>();
        }

        resetTimer = resetTimerDuration;
    }

    public override void ExitState()
    {
        // Checks if attacks state is populated
        if (attackState != null)
        {
            // Sets state to null
            attackState = null;
        }
    }

    public override void UpdateState()
    {
        // checks if player is in the arena 
        if (PlayerInArena(stateMachine.ArenaSize))
        {
            // changes to attack state
            stateMachine.ChangeState(attackState);
        }
        else
        {
            if (resetTimer <= 0f)
            {
                stateMachine.CurrentHealth = stateMachine.MaxHealth;
            }

            WalkTo(initialSpawn.position);

            resetTimer -= Time.deltaTime;
        }
    }
}
