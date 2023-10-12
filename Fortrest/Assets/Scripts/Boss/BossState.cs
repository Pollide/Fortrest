using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossState : MonoBehaviour
{
    // Reference to the state machine 
    protected BossStateMachine stateMachine;
    // Holds target transform
    protected Transform playerTransform;
    // Holds the initial spawn position of the boss
    protected Vector3 initialSpawn;
    // Nav mesh agent component
    protected NavMeshAgent agent;

    private void Start()
    {
        // Grabs the target transform for targeting
        playerTransform = PlayerController.global.transform;
        // Grabs the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        initialSpawn = transform.position;
        
    }

    // Sets agent destination and stopping distance
    protected void WalkTo(Vector3 targetPos, float stoppingDist)
    {
        if (agent.stoppingDistance != stoppingDist)
        {
            agent.stoppingDistance = stoppingDist;
        }

        agent.SetDestination(targetPos);

        if (agent.velocity != Vector3.zero)
        {
            stateMachine.BossAnimator.SetBool("isMoving", true);
            stateMachine.BossAnimator.SetBool("isIdle", false);
        }
        else
        {
            stateMachine.BossAnimator.SetBool("isMoving", false);
            stateMachine.BossAnimator.SetBool("isIdle", true);
        }
    }

    // Takes the player and applies damage
    public void ApplyDamageToTarget(float damage)
    {
        PlayerController playerScript = playerTransform.GetComponent<PlayerController>();

        playerScript.TakeDamage(damage, true);
    }

    // Returns true if the target is within a radius set in the inspector
    protected bool PlayerInArena(float _radius)
    {
        return Vector3.Distance(initialSpawn, playerTransform.position) < _radius;
    }

    // Populate the state machine
    public void Initialize(BossStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    // Run when entering the state
    public abstract void EnterState();
    
    // Run during the update stage
    public abstract void UpdateState();
    
    // Run on exit to do cleanup
    public abstract void ExitState();

    public Transform PlayerTransform
    {
        get { return playerTransform; }
    } 
    
    public Vector3 InitialSpawn
    {
        get { return initialSpawn; }
    }

    public BossStateMachine StateMachine
    {
        get { return stateMachine; }
    }
}
