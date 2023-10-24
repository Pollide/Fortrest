using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class SpiderBoss : MonoBehaviour
{
    public static SpiderBoss global;

    [HideInInspector] public Transform playerTransform;
    private float webAttackRange;
    public bool retreating;
    private NavMeshAgent agent;
    private float damage;
    public bool dead;
    private float speed;
    private float stoppingDistance;
    private float angularSpeed;
    private float acceleration;
    public int stage;
    private float timer, timer2, timer3;
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
    private int steps = 0;
    [HideInInspector] public float normalAttackCD;
    public bool normalAttackReady;
    private float attackRange;
    public bool attacking;

    [HideInInspector]
    public BossSpawner bossSpawner;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        VFXWeb.Stop();
        playerTransform = PlayerController.global.transform;
        retreating = false;
        agent = bossSpawner.bossAnimator.GetComponent<NavMeshAgent>();
        damage = 5.0f;
        stage = 1;
        specialAttackCD = 10.0f;
        poisonAttackChance = 0.4f;
        webAttackChance = 0.3f;
        poisonSpeed = 20.0f;
        webAttackRange = 5.0f;
        normalAttackCD = 4.0f;

        speed = 11f;
        stoppingDistance = 3.5f;
        angularSpeed = 180.0f;
        acceleration = 10.0f;
        SetAgentParameters(speed, acceleration, angularSpeed, stoppingDistance);
        attackRange = agent.stoppingDistance + 0.75f;
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
        // Spider retreats to its starting position if the player exits the arena
        retreating = !bossSpawner.CheckPlayerDistance();
        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (bossSpawner.bossAwakened && bossSpawner.introCompleted)
        {
            if (!attacking)
            {
                if (distanceToPlayer <= attackRange)
                {
                    bossSpawner.bossAnimator.SetBool("Moving", false);
                }
                else
                {
                    bossSpawner.bossAnimator.SetBool("Moving", true);
                }
            }

            if (retreating)
            {
                agent.SetDestination(bossSpawner.StartPosition);
                if (bossSpawner.health < bossSpawner.maxHealth)
                {
                    bossSpawner.UpdateHealth(Time.deltaTime * 3.0f);
                }
                else
                {
                    bossSpawner.UpdateHealth(bossSpawner.maxHealth);
                }
            }
            else
            {
                if (Vector3.Distance(playerTransform.position, agent.transform.position) <= attackRange && !specialAttackReady && normalAttackReady)
                {
                    Attack();
                }
                else
                {
                    agent.SetDestination(playerTransform.position);
                }
            }

            // Triggering different stages
            if (bossSpawner.health < ((bossSpawner.maxHealth / 3) * 2) && bossSpawner.health > (bossSpawner.maxHealth / 3))
            {
                stage = 2;
            }
            else if (bossSpawner.health < (bossSpawner.maxHealth / 3))
            {
                stage = 3;
            }

            // Special Attacks
            if (bossSpawner.bossAwakened && bossSpawner.introCompleted)
            {
                timer += Time.deltaTime;
            }
            if (timer >= specialAttackCD)
            {
                specialAttackReady = true;
                SpecialAttack();
            }

            if (!normalAttackReady)
            {
                timer3 += Time.deltaTime;
                if (timer3 > normalAttackCD)
                {
                    normalAttackReady = true;
                }
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
                    bossSpawner.bossAnimator.SetTrigger("Slam");
                }
            }

            // Death state
            if (dead)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossDeadSound, 1f, true, 0, false, transform);
                bossSpawner.bossAnimator.SetTrigger("Dead");
                StartCoroutine(DestroyOnDeath());
                dead = false;
            }
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
        attacking = true;
        bossSpawner.bossAnimator.ResetTrigger("Attack");
        bossSpawner.bossAnimator.SetTrigger("Attack");
        normalAttackReady = false;
        timer3 = 0f;
    }

    public void NormalAttackAnimEvent()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossAttackSound, 1f, true, 0, false, transform);
        if (distanceToPlayer < 8.0f)
        {
            PlayerController.global.TakeDamage(damage);
        }
    }

    private IEnumerator DestroyOnDeath()
    {
        yield return new WaitForSeconds(7f);
        agent.enabled = false;
        Destroy(gameObject);
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
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossWebAttackSound, 1f, true, 0, false, transform);
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
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossSlamSound, 1f, true, 0, false, transform);
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
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossJumpSound, 1f, true, 0, false, transform);
        jump = true;
        bossSpawner.bossAnimator.speed = 0.75f;
    }

    private void EndJumpAnimEvent()
    {
        jump = false;
        bossSpawner.bossAnimator.speed = 1.0f;
    }

    private void Damaged(float amount)
    {
        bossSpawner.UpdateHealth(-amount);

        if (bossSpawner.health <= 0)
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
        bossSpawner.bossAnimator.ResetTrigger("PoisonAttack");
        bossSpawner.bossAnimator.ResetTrigger("WebAttack");
        bossSpawner.bossAnimator.ResetTrigger("JumpAttack");

        switch (stage)
        {
            case 1:
                bossSpawner.bossAnimator.SetTrigger("PoisonAttack");
                break;
            case 2:
                if (distanceToPlayer < webAttackRange)
                {
                    randomChance = Random.Range(0f, 1f);
                    if (randomChance <= poisonAttackChance || randomChance > poisonAttackChance + webAttackChance)
                    {
                        bossSpawner.bossAnimator.SetTrigger("PoisonAttack");
                    }
                    else if (randomChance <= poisonAttackChance + webAttackChance)
                    {
                        bossSpawner.bossAnimator.SetTrigger("WebAttack");
                    }
                }
                else
                {
                    bossSpawner.bossAnimator.SetTrigger("PoisonAttack");
                }
                break;
            case 3:
                if (distanceToPlayer < webAttackRange)
                {
                    randomChance = Random.Range(0f, 1f);
                    if (randomChance <= poisonAttackChance)
                    {
                        bossSpawner.bossAnimator.SetTrigger("PoisonAttack");
                    }
                    else if (randomChance <= poisonAttackChance + webAttackChance)
                    {
                        bossSpawner.bossAnimator.SetTrigger("WebAttack");
                    }
                    else
                    {
                        bossSpawner.bossAnimator.SetTrigger("JumpAttack");
                    }
                }
                else
                {
                    int randomInt = Random.Range(1, 6);
                    if (randomInt == 1 || randomInt == 2)
                    {
                        bossSpawner.bossAnimator.SetTrigger("JumpAttack");
                    }
                    else
                    {
                        bossSpawner.bossAnimator.SetTrigger("PoisonAttack");
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
            if (PlayerController.global.attacking && bossSpawner.canBeDamaged && PlayerController.global.damageEnemy)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.SpiderBossHit1Sound : GameManager.global.SpiderBossHit2Sound, 1f, true, 0, false, transform);
                bossSpawner.canBeDamaged = false;
                StopAllCoroutines();
                ScreenShake.global.shake = true;
                Damaged(PlayerController.global.attackDamage);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (other.gameObject.tag == "Arrow" && other.GetComponent<ArrowTrigger>())
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.SpiderBossHit1Sound : GameManager.global.SpiderBossHit2Sound, 1f, true, 0, false, transform);
                other.GetComponent<ArrowTrigger>().singleHit = true;
                Damaged(PlayerController.global.bowDamage);
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
            }
        }
    }


    private void JumpSound()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossJumpAttackSound, 1f, true, 0, false, transform);
    }

    private void PoisonSound()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossPoisonAttackSound, 1f, true, 0, false, transform);
    }

    private void StepsSound()
    {
        steps++;
        if (steps > 4)
        {
            steps = 1;
        }
        switch (steps)
        {
            case 1:
                GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossStep1Sound, 0.6f, true, 0, false, transform);
                break;
            case 2:
                GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossStep2Sound, 0.6f, true, 0, false, transform);
                break;
            case 3:
                GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossStep3Sound, 0.6f, true, 0, false, transform);
                break;
            case 4:
                GameManager.global.SoundManager.PlaySound(GameManager.global.SpiderBossStep4Sound, 0.6f, true, 0, false, transform);
                break;
            default:
                break;

        }

    }

}