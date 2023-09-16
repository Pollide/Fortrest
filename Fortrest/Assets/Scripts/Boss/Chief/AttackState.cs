using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BossState
{
    // Timer for attacks
    private float attackTimer = 0f;
    // Timer for checking the random number to decide attack states
    [SerializeField] private float randomCheckTimer = 0f;
    [SerializeField] private float randomCheckDuration = 5f;
    // Damage for attack
    [SerializeField] private float damage = 0f;
    // The speed of attacks 
    [SerializeField] private float attackSpeed = 0f;
    // The distance the enemy can attack from
    [SerializeField] private float attackDistance = 0f;
    // Attack percentages
    [SerializeField] private float attackChance = 0.6f;
    [SerializeField] private float chargeChance = 0.2f;
    [SerializeField] private float slamChance = 0.2f;
    private float randValue = 0f;
    // The attack state
    private bool isAttacking = false;
    // Holds states
    private IdleState idleState;
    private ChargeState chargeState;
    private SlamState slamState;

    public override void EnterState()
    {
        // Checks if the state is null
        if (idleState == null)
        {
            // Gets the connected state
            idleState = GetComponent<IdleState>();
        }
        // Checks if the state is null
        if (chargeState == null)
        {
            // Gets the connected state
            chargeState = GetComponent<ChargeState>();
        }
        // Checks if the state is null
        if (slamState == null)
        {
            // Gets the connected state
            slamState = GetComponent<SlamState>();
        }
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
        //if (chargeState != null)
        //{
        //    // Sets state to null
        //    chargeState = null;
        //}
        //// Checks if state is populated
        //if (slamState != null)
        //{
        //    // Sets state to null
        //    slamState = null;
        //}
    }

    public override void UpdateState()
    {
        // Set agent destination
        WalkTo(playerTransform.position);
        // Boss phasses
        PhaseOne();
        PhaseTwo();
        PhaseThree();
        // Switch to Idle if player is outside of arena
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }
    }

    // Run checks to see if the attack can be called
    private bool CanAttack()
    {
        // Return true if the distance between the player and boss is less then or equal to the attack distance
        return Vector3.Distance(transform.position, playerTransform.position) <= attackDistance;
    }

    // Run the normal attack logic 
    private void Attack()
    {
        if (!isAttacking)
        {
            // Check if the enemy can attack
            if (CanAttack())
            {
                // Apply damage to the player or target
                ApplyDamageToTarget(damage);
                // Set the attack timer to control attack speed
                attackTimer = 1f / attackSpeed;
                // Stop agent
                agent.isStopped = true;
                // Set attack state to true to prevent calling multiple times
                isAttacking = true;
            }
            else
            {
                // Countdown for random number 
                if (randomCheckTimer > 0f)
                {
                    randomCheckTimer -= Time.deltaTime;
                }
            }
        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                // Reset the attack timer
                isAttacking = false;
                // Start agent
                agent.isStopped = false;
            }
        }
      
    }

    private void PhaseOne()
    {
        if (stateMachine.CurrentPhase == BossStateMachine.BossPhase.One)
        {
            Attack();
        }
    }

    private void PhaseTwo()
    {
        if (stateMachine.CurrentPhase == BossStateMachine.BossPhase.Two)
        {
            if (randomCheckTimer <= 0f)
            {
                CheckAttackState();
                randomCheckTimer = randomCheckDuration;
            }

            if (randValue < attackChance + slamChance)
            {
                Attack();
            }
            else
            {
                stateMachine.ChangeState(chargeState);
            }
        }
    }

    private void PhaseThree()
    {
        if (stateMachine.CurrentPhase == BossStateMachine.BossPhase.Three)
        {
            if (randomCheckTimer <= 0f)
            {
                CheckAttackState();
                randomCheckTimer = randomCheckDuration;
            }

            if (randValue <= attackChance)
            {
                Attack();
            }
            else if (randValue <= attackChance + chargeChance)
            {
                stateMachine.ChangeState(chargeState);
            }
            else 
            {
                stateMachine.ChangeState(slamState);
            }
        }
    }

    // Return a random float
    private void CheckAttackState()
    {
        randValue = Random.Range(0f, 1f);
    }
}
