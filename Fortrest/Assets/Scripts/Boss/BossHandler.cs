using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossHandler : MonoBehaviour
{
    #region Veriables
    // Enum for each boss state
    public enum BossStates
    {
        Chase,
        Attack,
        Charge,
        Idle,
        Start
    }
    // Holds the current boss atate
    [SerializeField] private BossStates states;
    // Nav mesh agent component
    [SerializeField] private NavMeshAgent agent;
    // Holds target transform
    private Transform target;
    // Holds charge trigger
    private BoxCollider chargeDMGTrigger;
    // Holds start position
    [SerializeField] private Transform initialSpawn;
    // Agent current speed
    [SerializeField] private float speed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    // Agent charge speed
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 1;
    [SerializeField] private float chargeTimer = 0;
    [SerializeField] private float chargeMaxTimer = 5;
    [SerializeField] private float chargePushForce = 5;
    [SerializeField] private float chargePushDuration = 5;
    // Charge radius
    [SerializeField] private float chargeRadius;
    // Duration of the wind-up phase
    [SerializeField] private float windUpDuration = 5f;
    // Arena radius
    [SerializeField] private float arenaRadius;

    // Distance for agent to stop before destination
    [SerializeField] private float stoppingDist;
    // Holds wheather the boss has been activated
    [SerializeField] private bool encountered = false;
    // Holds Wheather the boss state can be changed
    [SerializeField] private bool stateSwitchable = true;
    // Holds Wheather the boss is charging
    [SerializeField] private bool isCharging = false;
    [SerializeField] private bool playerHit = false;
    #endregion

    #region Main
    private void Awake()
    {
        // Sets agent speed to an initial speed to come back to if ever changed
        agent.speed = speed;
        // Get box charge trigger
        chargeDMGTrigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        // Switch states to start
        SwitchState(BossStates.Start);
        // Populate target transform for targeting
        target = PlayerController.global.transform;
        // Turn off charge damage trigger
        chargeDMGTrigger.enabled = false;
    }

    private void Update()
    {
        // Run through current state and execute
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
        // Check if player is in the boss area
        if (PlayerInArena())
        {
            // Make boss walt to target
            WalkTo(target.position, agent.radius + stoppingDist);
            // Check if remaining distance to target if target is within given range switch to attack
            if (agent.remainingDistance <= agent.radius + stoppingDist)
            {
                SwitchState(BossStates.Attack);
            }
            // Check if remaining distance to target if target is within given range switch to charge
            else if (agent.remainingDistance >= chargeRadius)
            {
                SwitchState(BossStates.Charge);
            }
        }
        else
        {
            // If target is not in area then reset
            SwitchState(BossStates.Idle);
        }
    }

    private void AttackState()
    {
        if (PlayerInArena())
        {
            if (agent.remainingDistance >= chargeRadius)
            {
                SwitchState(BossStates.Charge);
            }
        }
        else
        {
            SwitchState(BossStates.Idle);
        }
        
    }

    private void StartState()
    {
        if (PlayerInArena())
        {
            SwitchState(BossStates.Chase);
        }
        else
        {
            SwitchState(BossStates.Idle);
        }
    }

    private void IdleState()
    {
        if (PlayerInArena())
        {
            SwitchState(BossStates.Chase);
        }
        else
        {
            WalkTo(initialSpawn.position);
        }
    }

    private void ChargeState()
    {
        if (PlayerInArena())
        {
            if (isCharging)
            {
                chargeTimer += Time.deltaTime;
            }
            else
            {
                chargeTimer = 0;
            }

            if (!isCharging && Vector3.Distance(transform.position, target.position) >= chargeRadius)
            {
                StartCoroutine(WindUpAndCharge());
            }
            else if (!isCharging && Vector3.Distance(transform.position, target.position) < chargeRadius)
            {
                SwitchState(BossStates.Attack);
            }

            if (isCharging && chargeTimer > chargeMaxTimer)
            {
                StartCoroutine(StopCharging());
            }
            if (isCharging && Vector3.Distance(transform.position, target.position) <= chargeDistance)
            {
                // Stop charging when close to the target
                StartCoroutine(StopCharging());
            }
        }
        else if (!PlayerInArena() && stateSwitchable)
        {
            SwitchState(BossStates.Idle);
            if (isCharging)
            {
                isCharging = false;
            }
        }
    }
    #endregion

    #region Misc
    private bool PlayerInArena()
    {
        return Vector3.Distance(initialSpawn.position, target.position) < arenaRadius;
    }

    private void WalkTo(Vector3 targetPos, float stopingDistance = 0)
    {
        if (agent.stoppingDistance != stopingDistance)
        {
            agent.stoppingDistance = stopingDistance;
        }
        
        agent.SetDestination(targetPos);
    }

    IEnumerator StopCharging()
    {
        Debug.Log("Stopping");
        isCharging = false;
        agent.speed = speed;
        agent.isStopped = true;
        chargeDMGTrigger.enabled = false;
        yield return new WaitForSeconds(5);
        agent.isStopped = false;
        playerHit = false;
        stateSwitchable = true;
        //SwitchState(BossStates.Attack);
    }

    IEnumerator WindUpAndCharge()
    {
        Debug.Log("Wind-up phase");
        stateSwitchable = false;
        agent.isStopped = true;
        agent.speed = chargeSpeed;
        yield return new WaitForSeconds(windUpDuration);
        chargeDMGTrigger.enabled = true;
        agent.isStopped = false;
        WalkTo(target.position, stoppingDist);
        isCharging = true;
        Debug.Log("Charging!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerHit)
        {
            playerHit = true;
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(1, true);
            Vector3 pushDirection = target.position - transform.position;
            float angle = Vector3.Angle(pushDirection, player.playerCC.transform.position - transform.position);
            pushDirection = Quaternion.Euler(0f, angle, 0f) * pushDirection;
            player.SetPushDirection(pushDirection, chargePushForce);
            StartCoroutine(player.PushPlayer(chargePushDuration));
        }
    }
    #endregion


    #region SwitchFunctions
    public void SwitchState(BossStates _state)
    {
        if (stateSwitchable)
        {
            states = _state;
        }
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
