using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossHandler : MonoBehaviour
{
    #region Veriables
    enum BossStates
    {
        Chase,
        Attack,
        Charge,
        Idle,
        Start
    }

    [SerializeField] private BossStates states;

    [SerializeField] private NavMeshAgent agent; // Nav mesh agent component

    private Transform target; // Holds target transform

    [SerializeField] private Transform initialSpawn; // Holds start position

    [SerializeField] private float speed; // Agent current speed
    [SerializeField] private float arenaRadius; // Arena radius
    [SerializeField] private float chargeRadius; // Charge radius
    [SerializeField] private float stoppingDist; // Distance for agent to stop before destination
    [SerializeField] private bool encountered = false;
    #endregion


    #region Main
    private void Awake()
    {
        agent.speed = speed;
    }

    private void Start()
    {
        SwitchToStart();
        // Populate target transform for targeting
        target = PlayerController.global.transform;
    }

    private void Update()
    {
        switch (states)
        {
            case BossStates.Chase:
                ChaseState();
                break;
            case BossStates.Attack:
                AttackState();
                break;
            case BossStates.Charge:
                ChargeState();
                break;
            case BossStates.Idle:
                IdleState();
                break;
            case BossStates.Start:
                StartState();
                break;
            default:
                break;
        }
    }
    #endregion

    #region StateFunctions
    private void ChaseState()
    {
        if (playerInArena())
        {
            WalkTo(target.position, agent.radius + stoppingDist);

            if (agent.remainingDistance <= agent.radius + stoppingDist)
            {
                SwitchToAttack();
            }
            else if (agent.remainingDistance >= chargeRadius)
            {
                SwitchToCharge();
            }
        }
        else
        {
            SwitchToIdle();
        }
    }

    private void AttackState()
    {
        if (playerInArena())
        {
            if (agent.remainingDistance >= chargeRadius)
            {
                SwitchToCharge();
            }
        }
        else
        {
            SwitchToIdle();
        }
        
    }

    private void StartState()
    {
        if (playerInArena())
        {
            SwitchToChase();
        }
        else
        {
            SwitchToIdle();
        }
    }

    private void IdleState()
    {
        if (playerInArena())
        {
            SwitchToChase();
        }
        else
        {
            WalkTo(initialSpawn.position);
        }
    }

    private void ChargeState()
    {
        if (playerInArena())
        {
            Debug.Log("Charge");
        }
        else
        {
            SwitchToIdle();
        }
    }
    #endregion

    #region Misc
    private bool playerInArena()
    {
        return Vector3.Distance(initialSpawn.position, target.position) < arenaRadius;
    }

    private void WalkTo(Vector3 targetPos, float stopingDistance = 0)
    {
        agent.stoppingDistance = stopingDistance;
        agent.SetDestination(targetPos);
    }
    #endregion


    #region SwitchFunctions
    public void SwitchToChase()
    {
        states = BossStates.Chase;
        Debug.Log(states);
    }
    public void SwitchToAttack()
    {
        states = BossStates.Attack;
        Debug.Log(states);
    }
    public void SwitchToCharge()
    {
        states = BossStates.Charge;
        Debug.Log(states);
    }
    public void SwitchToIdle()
    {
        states = BossStates.Idle;
        Debug.Log(states);
    }
    public void SwitchToStart()
    {
        states = BossStates.Start;
        Debug.Log(states);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(initialSpawn.position, arenaRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chargeRadius);
    }
    #endregion
}
