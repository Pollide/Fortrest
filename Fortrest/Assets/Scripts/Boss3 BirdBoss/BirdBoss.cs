using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBoss : MonoBehaviour
{
    public static BirdBoss global;

    public Transform playerTransform;
    private Animator animator;
    [HideInInspector] public bool awoken;
    public Vector3 startPosition;
    public bool retreating;
    private float damage;
    public bool dead;
    public int stage;
    public bool startIntro;
    public Vector3 directionToPlayer;
    public float distanceToPlayer;
    public Vector3 directionToPlayerNoY;
    public float distanceToPlayerNoY;
    private float speed = 15f;
    private float rotationSpeed = 10.0f;
    public float stoppingDistance;
    public bool outOfScreen;
    public float offset;
    public float attackRange = 10.0f;
    [HideInInspector] public bool playerReached = true;
    [HideInInspector] public bool targetReached = false;
    public int hitReceived;
    public bool vulnerable;
    public bool crashed;
    public bool flying;
    public bool altitudeReached;
    public bool normalAttack = true;
    public bool normalAttackIndicator;
    public bool rockAttackIndicator;
    public GameObject telegraphedRectangle;

    [HideInInspector]
    public BossSpawner bossSpawner;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        playerTransform = PlayerController.global.transform;
        animator = GetComponent<Animator>();
        awoken = false;
        startPosition = transform.position;
        retreating = false;
        damage = 5.0f;
        stage = 1;
        stoppingDistance = 5.0f;
    }

    public void Awaken()
    {
        startIntro = true;
        awoken = true;
        animator.SetTrigger("Awaking");
    }

    void Update()
    {
        directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayerNoY = (new Vector3(playerTransform.position.x, 0f, playerTransform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        distanceToPlayerNoY = Vector3.Distance(new Vector3(playerTransform.position.x, 0f, playerTransform.position.z), new Vector3(transform.position.x, 0f, transform.position.z));
        outOfScreen = IsOutOfScreen();

        if (distanceToPlayer < 20.0f && !awoken)
        {
            awoken = true;
            animator.SetTrigger("TakeOff");
        }

        if (hitReceived >= 3)
        {
            animator.ResetTrigger("Crash");
            animator.SetTrigger("Crash");
        }      
        
        if (flying && !altitudeReached)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(startPosition.x, 5f, startPosition.z), Time.deltaTime * (speed / 2.0f));
            if (transform.position.y >= 5.0f)
            {
                altitudeReached = true;
            }
        }
    }

    public void MoveToTarget(Vector3 targetPosition, Vector3 targetDirection)
    {
        LookAt(targetDirection);
        MoveTowards(targetPosition);
    }

    public void LookAt(Vector3 targetDirection)
    {       
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);    
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        if (flying)
        {
            targetPosition = new Vector3(targetPosition.x, 5f, targetPosition.z);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
    }

    private void StartFlying()
    {
        animator.SetBool("Flying", true);
        flying = true;
    }

    private bool IsOutOfScreen()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.transform.position.z);
        Vector3 screenHeight = new Vector3(Screen.width / 2, Screen.height, Camera.main.transform.position.z);
        Vector3 screenWidth = new Vector3(Screen.width, Screen.height / 2, Camera.main.transform.position.z);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        float distanceX = Vector3.Distance(new Vector3(Screen.width / 2, 0f, 0f), new Vector3(screenPos.x, 0f, 0f));
        float distanceY = Vector3.Distance(new Vector3(0f, Screen.height / 2, 0f), new Vector3(0f, screenPos.y, 0f));

        if (distanceX - offset > Screen.width / 2 || distanceY - offset > Screen.height / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Damaged(float amount)
    {
        bossSpawner.UpdateHealth(-amount);

        if (bossSpawner.health <= 0)
        {
            StopAllCoroutines();
            MoveToTarget(transform.position, transform.position);
            dead = true;
            Time.timeScale = 1;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && bossSpawner.canBeDamaged && PlayerController.global.damageEnemy && crashed)
            {
                //GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.SpiderBossHit1Sound : GameManager.global.SpiderBossHit2Sound, 1f, true, 0, false, transform);
                bossSpawner.canBeDamaged = false;
                StopAllCoroutines();
                ScreenShake.global.shake = true;
                Damaged(PlayerController.global.attackDamage);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (other.gameObject.tag == "Arrow" && other.GetComponent<ArrowTrigger>() && vulnerable)
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.SpiderBossHit1Sound : GameManager.global.SpiderBossHit2Sound, 1f, true, 0, false, transform);
                other.GetComponent<ArrowTrigger>().singleHit = true;
                Damaged(PlayerController.global.bowDamage);
                hitReceived++;
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
            }
        }
    }

    private void SlidingAnimEvent()
    {
        flying = false;        
    }
}