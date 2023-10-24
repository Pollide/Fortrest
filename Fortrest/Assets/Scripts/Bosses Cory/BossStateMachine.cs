using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossStateMachine : MonoBehaviour
{
    // Enum for boss phases
    public enum BossPhase
    {
        One,
        Two,
        Three
    }

    // Holds the bosses current phase
    [SerializeField] private BossPhase currentPhase;

    // Holds the current state
    [SerializeField] private BossState currentState;
    // Holds the initial state the boss starts with
    [SerializeField] private BossState initialState;

    // Agent current speed
    [SerializeField] private float speed;

    // Bool to see if in defensive state
    [SerializeField] private bool inDefence = false;
    [SerializeField] private bool phase2Ran = false;
    [SerializeField] private bool phase3Ran = false;

    // Bool to see if in death state
    [SerializeField] private bool isDead = false;

    [HideInInspector]
    public BossSpawner bossSpawner;

    // Start is called before the first frame update
    void Start()
    {
        InitializeStateMachine();
        SetAgentStats();

        currentPhase = BossPhase.One;
        inDefence = true;
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
            // Debug.LogError("Initial state not set in BossStateMachine.");
        }
    }

    public void SetAgentStats()
    {
        if (GetComponent<NavMeshAgent>())
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
        }
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

    public void TakeDamage(float damage)
    {
        if (bossSpawner.health > 0)
        {
            if (inDefence)
            {
                bossSpawner.UpdateHealth(-(damage / 2f));
            }
            else
            {
                bossSpawner.UpdateHealth(-damage);
            }
        }

        if (bossSpawner.health <= bossSpawner.maxHealth / 3f)
        {
            currentPhase = BossPhase.Three;
        }
        else if (bossSpawner.health <= bossSpawner.maxHealth * 2f / 3f)
        {
            currentPhase = BossPhase.Two;
        }
        else
        {
            currentPhase = BossPhase.One;
        }

        if (bossSpawner.health <= 0f && !isDead)
        {
            StartCoroutine(DeadState());
        }
    }

    private IEnumerator DeadState()
    {
        isDead = true;
        BossAnimator.SetBool("isDead", true);
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false); //i dont want bosses to be destroyed so i can save their HP as zero thanks
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (transform)
            Gizmos.DrawWireSphere(transform.position, ArenaSize);
    }

    public GameObject HealthBar
    {
        get { return bossSpawner.BossCanvas; }
    }

    public float ArenaSize
    {
        get { return bossSpawner.Arenasize; }
        set { bossSpawner.Arenasize = value; }
    }

    public BossPhase CurrentPhase
    {
        get { return currentPhase; }
        set { currentPhase = value; }
    }

    public BossSpawner.TYPE BossType
    {
        get { return bossSpawner.bossType; }
        set { bossSpawner.bossType = value; }
    }

    public float BossSpeed
    {
        get { return speed; }
    }

    public bool InDefence
    {
        get { return inDefence; }
        set { inDefence = value; }
    }

    public bool CanBeDamaged
    {
        get { return bossSpawner.canBeDamaged; }
        set { bossSpawner.canBeDamaged = value; }
    }
    public bool PhaseTwoRan
    {
        get { return phase2Ran; }
        set { phase2Ran = value; }
    }
    public bool PhaseThreeRan
    {
        get { return phase3Ran; }
        set { phase3Ran = value; }
    }

    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    public Animator BossAnimator
    {
        get { return bossSpawner.bossAnimator; }
    }
}
