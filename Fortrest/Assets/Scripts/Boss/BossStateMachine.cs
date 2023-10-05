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
    [SerializeField] private GameObject BossCanvas;

    // Holds the bosses max health
    [SerializeField] private float maxHealth = 100f;
    // Arena radius
    [SerializeField] private float arenaRadius;
    // Agent current speed
    [SerializeField] private float speed;

    // Bool to see if in defensive state
    [SerializeField] private bool inDefence = false;
    [SerializeField] private bool phase2Ran = false;
    [SerializeField] private bool phase3Ran = false;

    // Bool to see if in death state
    [SerializeField] private bool isDead = false;
    [SerializeField] public Animator bossAnimator;

    [HideInInspector]
    public BossSpawner bossSpawner;
    // Start is called before the first frame update
    void Start()
    {
        InitializeStateMachine();
        SetAgentStats();
        maxHealth = bossSpawner.health;

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
            Debug.LogError("Initial state not set in BossStateMachine.");
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
                bossSpawner.health -= (damage / 2f);
                UpdateHealth();
            }
            else
            {
                bossSpawner.health -= damage;
                UpdateHealth();
            }
        }

        if (bossSpawner.health <= maxHealth / 3f)
        {
            currentPhase = BossPhase.Three;
        }
        else if (bossSpawner.health <= maxHealth * 2f / 3f)
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

    public void UpdateHealth()
    {
        BossCanvas.GetComponentInChildren<HealthBar>().SetHealth(bossSpawner.health, maxHealth);
    }

    private IEnumerator DeadState()
    {
        isDead = true;
        LevelManager.global.dayPaused = false;
        bossAnimator.SetBool("isDead", true);
        yield return new WaitForSeconds(2);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arenaRadius);
    }

    public GameObject HealthBar
    {
        get { return BossCanvas; }
    }

    public float ArenaSize
    {
        get { return arenaRadius; }
        set { arenaRadius = value; }
    }

    public float CurrentHealth
    {
        get { return bossSpawner.health; }
        set { bossSpawner.health = value; }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
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
        get { return bossAnimator; }
    }
}
