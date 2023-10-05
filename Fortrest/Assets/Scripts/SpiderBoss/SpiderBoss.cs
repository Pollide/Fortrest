using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBoss : MonoBehaviour
{
    public static SpiderBoss global;

    private Transform playerTransform;
    private float awakeRange;
    private Animator animator;
    private bool awoken;
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
    private float timer;
    private float specialAttackCD;
    private float randomChance;
    private float poisonAttackChance;
    private float webAttackChance;
    public GameObject poisonProjectile;
    private float poisonSpeed;
    private Rigidbody rb;
    private bool jump;
    public bool jumpAttackIndicator;
    public bool webAttackIndicator;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        playerTransform = PlayerController.global.transform;
        awakeRange = 20.0f;
        animator = GetComponent<Animator>();
        awoken = false;
        arenaSize = 50.0f;
        startPosition = transform.position;
        retreating = false;
        agent = animator.GetComponent<NavMeshAgent>();
        damage = 5.0f;
        health = 100.0f;
        maxHealth = health;
        canBeDamaged = true;
        stage = 1;
        specialAttackCD = 10.0f;
        poisonAttackChance = 0.5f;
        webAttackChance = 0.3f;
        poisonSpeed = 20.0f;
        rb = GetComponent<Rigidbody>();

        speed = 5.0f;
        stoppingDistance = 3.5f;
        angularSpeed = 180.0f;
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
        // Spider awakes when player gets close to it
        if (Vector3.Distance(playerTransform.position, transform.position) <= awakeRange && !awoken)
        {
            animator.SetTrigger("Awaking");
            awoken = true;
            healthCanvas.SetActive(true);
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
            SpecialAttack();
        }     

        if (jump)
        {
            agent.Move(-transform.forward * (Time.deltaTime * 10.0f));
        }

        // Death state
        if (dead)
        {
            animator.SetTrigger("Death");
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
    }

    private void CircleAnimEvent()
    {
        jumpAttackIndicator = true;
    }

    private void ConeAnimEvent()
    {
        webAttackIndicator = true;
    }

    private void WebAttackAnimEvent()
    {
        
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
                animator.SetTrigger("WebAttack");
                break;
            case 2:
                randomChance = Random.Range(0f, 1f);
                if (randomChance <= poisonAttackChance || randomChance > poisonAttackChance + webAttackChance)
                {
                    animator.SetTrigger("PoisonAttack");
                }
                else if (randomChance <= poisonAttackChance + webAttackChance)
                {
                    animator.SetTrigger("WebAttack");
                }               
                break;
            case 3:
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
                break;
            default:
                break;
        }         
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            Debug.Log("yoza");
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
        if (other.gameObject.tag == "Arrow")
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