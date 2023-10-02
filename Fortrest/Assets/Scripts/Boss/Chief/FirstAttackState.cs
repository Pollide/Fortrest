using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAttackState : BossState
{
    // Timer for attacks
    [SerializeField] private float attackTimer = 0f;
    
    // The speed of attacks 
    [SerializeField] private float attackSpeed = 0f;
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private  float rotationSpeed = 5.0f;

    // Timer for checking the random number to decide attack states
    [SerializeField] private float randomCheckTimer = 0f;
    [SerializeField] private float randomCheckDuration = 5f;
    
    // Damage for attack
    [SerializeField] private float damage = 0f;
    
    // The distance the enemy can attack from
    [SerializeField] private float attackDistance = 0f;
    
    // Attack percentages
    [SerializeField] private float firstAttackChance = 0.6f;
    [SerializeField] private float secondAttackChance = 0.2f;
    [SerializeField] private float thirdAttackChance = 0.2f;
    
    // The value to determine the attack used
    private float randValue = 0f;
    
    // Holds the attack 
    [SerializeField] private bool isAttacking = false;
    
    // Holds states
    [SerializeField] private IdleState idleState;
    [SerializeField] private SecondAttackState attackState2;
    [SerializeField] private ThirdAttackState attackState3;

    public override void EnterState()
    {
        randValue = 0f;

        randomCheckTimer = randomCheckDuration;
    }

    public override void ExitState()
    {
        stateMachine.BossAnimator.SetBool("attacking", false);
    }

    public override void UpdateState()
    {
        // Switch to Idle if player is outside of arena
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (!stateMachine.Phase2Ran && stateMachine.CurrentPhase == BossStateMachine.BossPhase.Two)
        {
            stateMachine.Phase2Ran = true;
            stateMachine.ChangeState(attackState2);
        }
        if (!stateMachine.Phase3Ran && stateMachine.CurrentPhase == BossStateMachine.BossPhase.Three)
        {
            stateMachine.Phase3Ran = true;
            stateMachine.ChangeState(attackState3);
        }

        // Set agent destination
        WalkTo(playerTransform.position, stoppingDistance);
        // Boss phasses
        PhaseOne();
        PhaseTwo();
        PhaseThree();
    }

    // Run checks to see if the attack can be called
    private bool CanAttack()
    {

        Vector3 directionToTarget = (playerTransform.position - transform.position);
        directionToTarget.y = 0; // Set the Y component to 0
        directionToTarget.Normalize(); // Normalize the vector to make it so player can get close

        float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

        float threshold = 0.9f;
        if (dotProduct > threshold && Vector3.Distance(transform.position, playerTransform.position) <= attackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Run the normal attack logic 
    private void Attack()
    {
        // Countdown for random number 
        if (randomCheckTimer > 0f)
        {
            randomCheckTimer -= Time.deltaTime;
        }

        switch (stateMachine.BossType)
        {
            case BossStateMachine.TYPE.Chieftain:
                AttackChief();
                break;
            case BossStateMachine.TYPE.Basilisk:
                AttackSnake();
                break;
            case BossStateMachine.TYPE.SpiderQueen:
                AttackSpider();
                break;
            case BossStateMachine.TYPE.Bird:
                AttackBird();
                break;
            case BossStateMachine.TYPE.Werewolf:
                AttackWarewolf();
                break;
            case BossStateMachine.TYPE.Fire:
                AttackFire();
                break;
            default:
                break;
        }
      

    }

    private void AttackChief()
    {
        if (!isAttacking)
        {
            // Check if the enemy can attack
            if (CanAttack())
            {
                stateMachine.BossAnimator.ResetTrigger("attacking");
                stateMachine.BossAnimator.SetTrigger("attacking");
                isAttacking = true;
                // Set the attack timer to control attack speed
                attackTimer = 1f / attackSpeed;
                // Stop agent
                agent.isStopped = true;
                // Set attack state to true to prevent calling multiple times
            }
            else
            {
                // Calculate the direction to the target
                Vector3 targetDirection = playerTransform.position - transform.position;

                // Calculate the rotation needed to face the target
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                // Smoothly interpolate the current rotation to the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                // Reset the attack timer
                stateMachine.BossAnimator.ResetTrigger("attacking");
                isAttacking = false;
                // Start agent
                agent.isStopped = false;
            }
        }
    }

    private void AttackSnake()
    {
        if (!isAttacking)
        {
          
        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    private void AttackSpider()
    {
        if (!isAttacking)
        {

        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    private void AttackBird()
    {
        if (!isAttacking)
        {

        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    private void AttackWarewolf()
    {
        if (!isAttacking)
        {

        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    private void AttackFire()
    {
        if (!isAttacking)
        {

        }
        else
        {
            // Count down the attack timer
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
    }

    public void PlaySlash(int _index)
    {
        if (_index == 0)
        {
            LevelManager.global.VFXBossSlash.transform.position = transform.position;
            LevelManager.global.VFXBossSlash.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 90.0f, transform.eulerAngles.z);
            LevelManager.global.VFXBossSlashReversed.transform.localScale = new Vector3(1.25f, 0.65f, 1.25f);
            LevelManager.global.VFXBossSlash.Play();
        }
        if (_index == 1)
        {
            LevelManager.global.VFXBossSlashReversed.transform.position = transform.position;
            LevelManager.global.VFXBossSlashReversed.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 90.0f, transform.eulerAngles.z);
            LevelManager.global.VFXBossSlashReversed.transform.localScale = new Vector3(1.25f, 0.65f, 1.25f);
            LevelManager.global.VFXBossSlashReversed.Play();
            
        }
        if (_index == 2)
        {
            LevelManager.global.VFXBossSlashReversed.transform.position = transform.position;
            LevelManager.global.VFXBossSlashReversed.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 90.0f, transform.eulerAngles.z - 90.0f);
            LevelManager.global.VFXBossSlashReversed.transform.localScale = new Vector3(0.8f, 0.65f, 0.8f);
            LevelManager.global.VFXBossSlashReversed.Play();
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

            if (randValue < firstAttackChance + thirdAttackChance)
            {
                Attack();
            }
            else
            {
                stateMachine.ChangeState(attackState2);
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

            if (randValue <= firstAttackChance)
            {
                Attack();
            }
            else if (randValue <= firstAttackChance + secondAttackChance)
            {
                stateMachine.ChangeState(attackState2);
            }
            else
            {
                stateMachine.ChangeState(attackState3);
            }
        }
    }

    // Return a random float
    private void CheckAttackState()
    {
        randValue = Random.Range(0f, 1f);
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public bool IsAttacking
    {
        get { return isAttacking; }
        set { isAttacking = value; }
    }
}
