using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManagerState : BossState
{
    // Timer for attacks
    [SerializeField] private float attackTimer = 0f;
    public float attackTime = 0f;
    public float attackDuration = 5f;

    // The speed of attacks 
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private float rotationSpeed = 5.0f;
    public int attackCounter = 0;
    public float attackRadius = 10f;

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
    [SerializeField] private bool canChangeState = false;
    [SerializeField] private bool canAttack = false;
    [SerializeField] private GameObject telegraph;

    // Holds states
    private IdleState idleState;
    private PhaseTwoAttack phaseTwoAttack;
    private PhaseThreeAttack phaseThreeAttack;

    public override void EnterState()
    {
        if (idleState == null)
        {
            // Gets the connected state
            idleState = GetComponent<IdleState>();
        }
        // Checks if the state is null
        if (phaseTwoAttack == null)
        {
            // Gets the connected state
            phaseTwoAttack = GetComponent<PhaseTwoAttack>();
        }
        if (phaseThreeAttack == null)
        {
            // Gets the connected state
            phaseThreeAttack = GetComponent<PhaseThreeAttack>();
        }

        canChangeState = true;
        canAttack = true;
        randValue = 0f;

        if (GetComponent<TelegraphCircle>())
        {
            GetComponent<TelegraphCircle>().isAttack = true;
        }

        randomCheckTimer = randomCheckDuration;
        isAttacking = false;
    }

    public override void ExitState()
    {
        stateMachine.BossAnimator.ResetTrigger("isAttacking");
        telegraph.SetActive(false);
        isAttacking = false;
        attackTime = 0f;
    }

    public override void UpdateState()
    {
        // Switch to Idle if player is outside of arena
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (!stateMachine.PhaseTwoRan && stateMachine.CurrentPhase == BossStateMachine.BossPhase.Two && canChangeState)
        {
            stateMachine.PhaseTwoRan = true;
            stateMachine.ChangeState(phaseTwoAttack);
        }
        if (!stateMachine.PhaseThreeRan && stateMachine.CurrentPhase == BossStateMachine.BossPhase.Three && canChangeState)
        {
            stateMachine.PhaseThreeRan = true;
            stateMachine.ChangeState(phaseThreeAttack);
        }

        // Set agent destination
        if (!stateMachine.BossAnimator.GetBool("isTired") && !isAttacking && stateMachine.BossType == BossSpawner.TYPE.Chieftain)
        {
            WalkTo(playerTransform.position, stoppingDistance);
        }

        // Boss phasses
        PhaseOne();
        PhaseTwo();
        PhaseThree();
    }

    // Run checks to see if the attack can be called
    private bool CanAttack(bool isTired)
    {
        Vector3 directionToTarget = (playerTransform.position - transform.position);
        directionToTarget.y = 0; // Set the Y component to 0
        directionToTarget.Normalize(); // Normalize the vector to make it so player can get close

        float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

        float threshold = 0.9f;
        if (dotProduct > threshold && Vector3.Distance(transform.position, playerTransform.position) <= attackDistance && stateMachine.BossAnimator.GetBool("isTired") == isTired)
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

        if (stateMachine.BossAnimator.GetBool("isTired"))
        {
            SetTelegraph(false);
            canChangeState = false;
        }
        if (!isAttacking)
        {
            // Check if the enemy can attack
            if (CanAttack(false))
            {
                stateMachine.BossAnimator.ResetTrigger("isAttacking");
                stateMachine.BossAnimator.SetTrigger("isAttacking");
                SetTelegraph(true);
                isAttacking = true;
                canAttack = true;
                // Stop agent
                agent.isStopped = true;
            }
            else if (!CanAttack(true))
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
            canChangeState = false;

            if (attackTime < attackDuration && canAttack && !stateMachine.BossAnimator.GetBool("isTired"))
            {

                attackTime += Time.deltaTime;

                telegraph.transform.position = transform.position;

            }

            if (attackTime >= attackDuration && !stateMachine.BossAnimator.GetBool("isTired"))
            {
                stateMachine.BossAnimator.speed = 1f;
                attackTime = 0f;
                canAttack = false;

                if (telegraph.activeSelf)
                {
                    StartCoroutine(GetComponent<TelegraphCircle>().DoAreaDamage(0.1f, Damage, attackRadius));
                }

            }
        }
    }

    public void SetTelegraph(bool isActive)
    {
        telegraph.SetActive(isActive);
        GetComponent<TelegraphCircle>().outer.SetActive(isActive);
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
            if (randomCheckTimer <= 0f && canChangeState)
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
                stateMachine.ChangeState(phaseTwoAttack);
            }
        }
    }

    private void PhaseThree()
    {
        if (stateMachine.CurrentPhase == BossStateMachine.BossPhase.Three)
        {
            if (randomCheckTimer <= 0f && canChangeState)
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
                stateMachine.ChangeState(phaseTwoAttack);
            }
            else
            {
                stateMachine.ChangeState(phaseThreeAttack);
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

    public bool CanChangeState
    {
        get { return canChangeState; }
        set { canChangeState = value; }
    }

    public GameObject Telegraph
    {
        get { return telegraph; }
    }
}
