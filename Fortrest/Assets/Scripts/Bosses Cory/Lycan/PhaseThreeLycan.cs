using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseThreeLycan : BossState
{
    [SerializeField] private BossState idleState;
    [SerializeField] private BossState nextState;
    [SerializeField] private float nextAttackTime = 0f;
    public bool telegraph;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    public Vector3 DirectionToPlayer { get; private set; }

    public override void EnterState()
    {
        stateMachine.CurrentPhase = BossStateMachine.BossPhase.Two;
    }

    public override void ExitState()
    {
        GetComponent<IdleState>().lastState = this;
    }

    public override void UpdateState()
    {
        DirectionToPlayer = (playerTransform.position - transform.position).normalized;

        // Switch to Idle if player is outside of arena
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }
        else
        {
            WalkTo(playerTransform.position, 3);
            Attack();
        }
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                telegraph = true;
                // Play the attack animation
                stateMachine.BossAnimator.ResetTrigger("Attack");
                stateMachine.BossAnimator.SetTrigger("Attack");

                // Set the cooldown timer
                nextAttackTime = Time.time + attackCooldown;
            }
        }

        if (stateMachine.bossSpawner.health <= stateMachine.bossSpawner.maxHealth / 2f)
        {
            stateMachine.ChangeState(nextState);
        }
    }

}
