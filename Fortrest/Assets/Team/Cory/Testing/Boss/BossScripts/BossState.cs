using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BossState : MonoBehaviour
{
    // Reference to the state machine 
    protected BossStateMachine stateMachine;
    // Holds target transform
    protected Transform target;
    // Holds the initial spawn position of the boss
    protected Transform initialSpawn;
    // Nav mesh agent component
    protected NavMeshAgent agent;
    // The distance that the agent will stop
    [SerializeField] protected float stoppingDistance;

    private void Start()
    {
        // Grabs the target transform for targeting
        target = PlayerController.global.transform;
        // Grabs the spawn position as the initial position
        initialSpawn = gameObject.transform.parent;
        // Grabs the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
    }
    // Sets agent destination and stopping distance
    protected void WalkTo(Vector3 targetPos, float stopingDistance = 0)
    {
        if (agent.stoppingDistance != stopingDistance)
        {
            agent.stoppingDistance = stopingDistance;
        }

        agent.SetDestination(targetPos);
    }
    // Returns true if the target is within a radius set in the inspector
    protected bool PlayerInArena(float _radius)
    {
        return Vector3.Distance(initialSpawn.position, target.position) < _radius;
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
}
