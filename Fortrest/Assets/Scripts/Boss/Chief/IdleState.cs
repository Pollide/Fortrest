using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BossState
{
    // Holds next state
    private AttackManagerState attackState;
    private float resetTimer = 0f;
    [SerializeField] private float resetTimerDuration;
    [SerializeField] private float stoppingDistance = 3f;

    public override void EnterState()
    {
        // Checks if the attack state is null
        if (attackState == null)
        {
            // Gets the connected attack state
            attackState = GetComponent<AttackManagerState>();
        }

        stateMachine.bossSpawner.BossEncountered(true);

        resetTimer = resetTimerDuration;
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        // checks if player is in the arena 
        if (PlayerInArena(stateMachine.ArenaSize))
        {
            StateMachine.bossSpawner.BossEncountered(true);
            // changes to attack state
            stateMachine.ChangeState(attackState);
        }
        else
        {
            if (resetTimer <= 0f)
            {
                stateMachine.bossSpawner.Awake();
                StateMachine.bossSpawner.BossEncountered(false);
                stateMachine.bossSpawner.UpdateHealth();
            }

            if (stateMachine.BossType == BossSpawner.TYPE.Chieftain)
            {
                WalkTo(initialSpawn, stoppingDistance);
            }

            resetTimer -= Time.deltaTime;
        }
    }
}
