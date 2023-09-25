using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateBird : BossState
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
    private IdleStateChief idleState;
    private ChargeStateChief chargeState;
    private SlamStateChief slamState;

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
