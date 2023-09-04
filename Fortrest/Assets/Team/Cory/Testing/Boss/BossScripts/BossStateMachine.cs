using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : MonoBehaviour
{
    // Holds the current state
    [SerializeField] private BossState currentState;
    // Holds the initial state the boss starts with
    [SerializeField] private BossState initialState;
    // Arena radius
    [SerializeField] private float arenaRadius;
    // Agent current speed
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        InitializeStateMachine();
        SetAgentStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    private void InitializeStateMachine()
    {
        if (initialState != null)
        {
            ChangeState(initialState);
        }
        else
        {
            Debug.LogError("Initial state not set in BossStateMachine.");
        }
    }

    public void SetAgentStats()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }


    public void ChangeState(BossState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;
        currentState.Initialize(this); // Pass a reference to the state machine
        currentState.EnterState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetArenaSize());
    }

    public float GetArenaSize() { return arenaRadius; }
}
