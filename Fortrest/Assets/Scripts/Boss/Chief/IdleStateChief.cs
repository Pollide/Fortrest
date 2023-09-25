using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateChief : BossState
{
    // Holds next state
    private AttackStateChief attackState;
    private float resetTimer = 0f;
    [SerializeField] private float resetTimerDuration;


    public override void EnterState()
    {
        // Checks if the attack state is null
        if (attackState == null)
        {
            // Gets the connected attack state
            attackState = GetComponent<AttackStateChief>();
        }

        stateMachine.HealthBar.SetActive(true);

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
            stateMachine.HealthBar.SetActive(true);
            // changes to attack state
            stateMachine.ChangeState(attackState);
        }
        else
        {
            if (resetTimer <= 0f)
            {
                stateMachine.CurrentHealth = stateMachine.MaxHealth;
                stateMachine.HealthBar.SetActive(false);
                stateMachine.UpdateHealth();
            }

            WalkTo(initialSpawn.position);

            resetTimer -= Time.deltaTime;
        }
    }
}
