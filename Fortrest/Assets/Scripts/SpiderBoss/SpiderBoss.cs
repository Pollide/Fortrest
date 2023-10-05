using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpiderBoss : MonoBehaviour
{
    public static SpiderBoss global;

    private Transform playerTransform;
    public float awakeRange;
    private float webAttackRange;
    private Animator animator;
    [HideInInspector] public bool awoken;
    private float arenaSize;
    public Vector3 startPosition;
    public bool retreating;
    private NavMeshAgent agent;
    private float damage;
    private float health;
    private float maxHealth;
    public bool canBeDamaged;
    public bool dead;
    public GameObject healthCanvas;
    public GameObject healthBar;
    private float speed;
    private float stoppingDistance;
    private float angularSpeed;
    private float acceleration;
    public int stage;
    private float timer, timer2;
    private float specialAttackCD;
    private float randomChance;
    private float poisonAttackChance;
    private float webAttackChance;
    public GameObject poisonProjectile;
    private float poisonSpeed;
    private bool jump;
    public bool jumpAttackIndicator;
    public bool webAttackIndicator;
    public VisualEffect VFXWeb;
    public bool specialAttackReady;
    public float distanceToPlayer;
    public bool rootNow;
    public bool midAir;
    public bool slamNow;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        VFXWeb.Stop();
        playerTransform = PlayerController.global.transform;
        awakeRange = 20.0f;
        animator = GetComponent<Animator>();
        awoken = false;
        arenaSize = 50.0f;
        startPosition = transform.position;
        retreating = false;
        agent = animator.GetComponent<NavMeshAgent>();
        damage = 5.0f;
        health = 70.0f;
        maxHealth = health;
        canBeDamaged = true;
        stage = 1;
        specialAttackCD = 10.0f;
        poisonAttackChance = 0.4f;
        webAttackChance = 0.4f;
        poisonSpeed = 20.0f;
        webAttackRange = 5.0f;

        speed = 12f;
        stoppingDistance = 3.5f;
        angularSpeed = 200.0f;
        acceleration = 10.0f;
        SetAgentParameters(speed, acceleration, angularSpeed, stoppingDistance);
    }

    void SetAgentParameters(float _speed, float _acceleration, float _angular, float _stopping)
    {
        agent.speed = _speed;
        agent.acceleration = _acceleration;
        agent.angularSpeed = _angular;
        agent.stoppingDistance = _stopping;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        // Spider awakes when player gets close to it
        if (distanceToPlayer <= awakeRange && !awoken)
        {
            healthCanvas.SetActive(true);
            animator.SetTrigger("Awaking");
        }

        // Spider moves at all times
        if (awoken)
        {
            animator.SetBool("Moving", true);
        }

        // Spider retreats to its starting position if the player exits the arena
        if (Vector3.Distance(playerTransform.position, startPosition) > arenaSize)
        {
            retreating = true;
        }
        else
        {
            retreating = false;
        }

        if (retreating)
        {
            if (health < maxHealth)
            {
                health += Time.deltaTime * 3.0f;
            }
            else
            {
                health = maxHealth;
                healthCanvas.SetActive(false);
            }
            UpdateHealth();
        }
        else if (awoken)
        {
            healthCanvas.SetActive(true);
        }

        // Triggering different stages
        if (health < ((maxHealth / 3) * 2) && health > (health / 3))
        {
            stage = 2;
        }
        else if (health < (health / 3))
        {
            stage = 3;
        }

        // Special Attacks
        if (awoken)
        {
            timer += Time.deltaTime;
        }
        if (timer >= specialAttackCD)
        {
            specialAttackReady = true;
            SpecialAttack();
        }

        if (jump)
        {
            agent.Move(-transform.forward * (Time.deltaTime * 10.0f));
        }

        if (midAir)
        {
            timer2 += Time.deltaTime;
            if (timer2 > 3.5f)
            {
                midAir = false;
                animator.SetTrigger("Slam");
            }
        }

        // Death state
        if (dead)
        {
            healthCanvas.SetActive(false);           
            animator.SetTrigger("Dead");
            StartCoroutine(DestroyOnDeath());
            dead = false;
        }
    }

    public void LookAt(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized; // Gets a direction using a normalized vector
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Obtaining a rotation angle
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); // Smoothly rotating towards target
    }

    public void Attack()
    {
        LookAt(playerTransform);
        agent.SetDestination(agent.transform.position);
        animator.SetTrigger("Attack");      
    }

    public void NormalAttackAnimEvent()
    {
        int randomInt = Random.Range(0, 3);
        AudioClip temp = null;
        switch (randomInt)
        {
            case 0:
                temp = GameManager.global.PlayerHit1Sound;
                break;
            case 1:
                temp = GameManager.global.PlayerHit2Sound;
                break;
            case 2:
                temp = GameManager.global.PlayerHit3Sound;
                break;
            default:
                break;
        }
        GameManager.global.SoundManager.PlaySound(temp, 0.9f);
        if (PlayerController.global.playerCanBeDamaged)
        {
            PlayerController.global.TakeDamage(damage, true);
        }
    }

    private IEnumerator DestroyOnDeath()
    {
        yield return new WaitForSeconds(7f);
        agent.enabled = false;
        Destroy(gameObject);
    }

    private IEnumerator Intro()
    {
        LevelManager.global.HUD.SetActive(false);
        animator.SetTrigger("Awaking");
        yield return new WaitForSeconds(2f);
        LevelManager.global.HUD.SetActive(true);
        healthCanvas.SetActive(true);

    }

    private void PoisonAttackAnimEvent()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 direction = transform.forward;
            if (i == 1)
            {
                direction = transform.forward + (transform.right * 0.8f);
            }
            else if (i == 2)
            {
                direction = transform.forward - (transform.right * 0.8f);
            }
            else if (i == 3)
            {
                direction = transform.forward + (transform.right * 0.4f);
            }
            else if (i == 4)
            {
                direction = transform.forward - (transform.right * 0.4f);
            }
            GameObject projectile = Instantiate(poisonProjectile, transform.position + (transform.forward) + new Vector3(0f, 1f, 0f), Quaternion.identity);
            projectile.GetComponent<Rigidbody>().AddForce(direction * poisonSpeed, ForceMode.Impulse);
        }
        specialAttackReady = false;
    }
    
    private void WebAttackAnimEvent()
    {
        VFXWeb.Play();
        specialAttackReady = false;
        StartCoroutine(Rooting());
    }

    private void ConeAnimEvent()
    {
        webAttackIndicator = true;
    }

    private IEnumerator Rooting()
    {
        rootNow = true;
        yield return new WaitForSeconds(0.7f);
        rootNow = false;
    }

    private void JumpAttackAnimEvent()
    {
        specialAttackReady = false;
        StartCoroutine(Slamming());        
    }

    private void CircleAnimEvent()
    {
        timer2 = 0;
        midAir = true;
        jumpAttackIndicator = true;
    }

    private IEnumerator Slamming()
    {
        slamNow = true;
        yield return new WaitForFixedUpdate();
        slamNow = false;
    }

    private void StartJumpAnimEvent()
    {
        jump = true;
        animator.speed = 0.75f;
    }

    private void EndJumpAnimEvent()
    {
        jump = false;
        animator.speed = 1.0f;
    }

    private void Damaged(float amount)
    {
        health -= amount;
        UpdateHealth();

        if (health <= 0)
        {
            StopAllCoroutines();
            agent.SetDestination(transform.position);
            dead = true;
            Time.timeScale = 1;
        }
    }

    private void SpecialAttack()
    {      
        timer = 0;
        animator.ResetTrigger("PoisonAttack");
        animator.ResetTrigger("WebAttack");
        animator.ResetTrigger("JumpAttack");

        switch (stage)
        {
            case 1:
                animator.SetTrigger("PoisonAttack");
                break;
            case 2:
                if (distanceToPlayer < webAttackRange)
                {
                    randomChance = Random.Range(0f, 1f);
                    if (randomChance <= poisonAttackChance || randomChance > poisonAttackChance + webAttackChance)
                    {
                        animator.SetTrigger("PoisonAttack");
                    }
                    else if (randomChance <= poisonAttackChance + webAttackChance)
                    {
                        animator.SetTrigger("WebAttack");
                    }
                }
                else
                {
                    animator.SetTrigger("PoisonAttack");
                }
                break;
            case 3:
                if (distanceToPlayer < webAttackRange)
                {
                    randomChance = Random.Range(0f, 1f);
                    if (randomChance <= poisonAttackChance)
                    {
                        animator.SetTrigger("PoisonAttack");
                    }
                    else if (randomChance <= poisonAttackChance + webAttackChance)
                    {
                        animator.SetTrigger("WebAttack");
                    }
                    else
                    {
                        animator.SetTrigger("JumpAttack");
                    }
                }
                else
                {
                    int randomInt = Random.Range(1, 4);
                    if (randomInt == 3)
                    {
                        animator.SetTrigger("JumpAttack");
                    }
                    else
                    {
                        animator.SetTrigger("PoisonAttack");
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && canBeDamaged && PlayerController.global.damageEnemy)
            {
                canBeDamaged = false;
                StopAllCoroutines();
                //PickSound(hitSound, hitSound2, 1.0f);
                ScreenShake.global.shake = true;
                Damaged(PlayerController.global.attackDamage);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (other.gameObject.tag == "Arrow" && other.GetComponent<ArrowTrigger>())
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                other.GetComponent<ArrowTrigger>().singleHit = true;
                Damaged(PlayerController.global.bowDamage);
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
            }
        }
    }

    private void UpdateHealth()
    {
        healthBar.GetComponent<HealthBar>().SetHealth(health, maxHealth);
    }
}