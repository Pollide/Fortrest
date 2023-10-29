using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdBoss : MonoBehaviour
{
    public static BirdBoss global;

    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public bool dead;
    [HideInInspector] public Vector3 directionToPlayer;
    [HideInInspector] public float distanceToPlayer;
    private float speed = 15f;
    private float screenOffset = 50f;
    [HideInInspector] public bool playerReached = true;
    [HideInInspector] public bool targetReached = false;
    public bool altitudeReached;
    public bool flying;
    public bool diving;
    public bool vulnerable;
    public bool crashed;
    public bool retreating;
    public int hitReceived;
    [HideInInspector] public bool normalAttack = true;
    [HideInInspector] public bool circleAttackIndicator;
    public GameObject telegraphedCircle;
    private bool stopMoving;
    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public Vector3 targetDirection;
    [HideInInspector] public bool flyAnimOver = true;
    [HideInInspector] public bool crashAnimOver = true;
    [HideInInspector] public BoxCollider boxCollider;
    [HideInInspector] public CapsuleCollider capsuleCollider;
    public SphereCollider telegraphCollider;
    public GameObject rockObject;
    public GameObject displayedRock;
    [HideInInspector] public bool hitOnce;
    [HideInInspector] public bool lastWasNormal;

    [HideInInspector] public BossSpawner bossSpawner;
    Vector3 initialPosition;
    Vector3 initialDirection;
    private void Awake()
    {
        global = this;
        initialPosition = transform.position;
        initialDirection = transform.forward;
    }

    void Start()
    {
        playerTransform = PlayerController.global.transform;
        startPosition = transform.position;
        retreating = false;
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {

        if (bossSpawner.bossAwakened && !bossSpawner.bossEncountered)
        {
            hitReceived = 0;
            crashed = false;
            altitudeReached = false;
            bossSpawner.bossAnimator.SetBool("Attack1", false);
            bossSpawner.bossAnimator.SetBool("Attack2", false);
            bossSpawner.bossAnimator.SetBool("Attack3", false);
            // bossSpawner.bossAnimator.SetBool("Flying", false);
            //   flying = false;
            MoveToTarget(initialPosition, initialDirection);
        }
        else
        {
            directionToPlayer = (new Vector3(playerTransform.position.x, 0f, playerTransform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
            distanceToPlayer = Vector3.Distance(new Vector3(playerTransform.position.x, 0f, playerTransform.position.z), new Vector3(transform.position.x, 0f, transform.position.z));
            capsuleCollider.enabled = crashed ? true : false;
            boxCollider.enabled = crashed ? false : true;
            telegraphCollider.enabled = boxCollider.enabled ? false : true;

            // Boss flies up at the start
            if (flying && !altitudeReached)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 7.5f, transform.position.z), Time.deltaTime * (speed / 2.0f));
                if (transform.position.y >= 7.5f)
                {
                    altitudeReached = true;
                }
            }

            // Boss crashes after being hit 3 times
            if (hitReceived >= 3)
            {
                bossSpawner.bossAnimator.ResetTrigger("Crash");
                bossSpawner.bossAnimator.SetTrigger("Crash");
                hitReceived = 0;
            }

        }
    }

    // Used to orientate and move the boss
    public void MoveToTarget(Vector3 targetPosition, Vector3 targetDirection)
    {
        LookAt(targetDirection);
        MoveTowards(targetPosition);
    }

    // Direction the boss is looking at
    public void LookAt(Vector3 targetDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
    }

    // Location the boss is moving to
    public void MoveTowards(Vector3 targetPosition)
    {
        if (!stopMoving)
        {
            if (flying)
            {
                targetPosition = new Vector3(targetPosition.x, 7.5f, targetPosition.z);
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }

    // Checks if the boss can be seen on the screen
    public bool IsOutOfScreen()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.transform.position.z);
        Vector3 screenHeight = new Vector3(Screen.width / 2, Screen.height, Camera.main.transform.position.z);
        Vector3 screenWidth = new Vector3(Screen.width, Screen.height / 2, Camera.main.transform.position.z);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        float distanceX = Vector3.Distance(new Vector3(Screen.width / 2, 0f, 0f), new Vector3(screenPos.x, 0f, 0f));
        float distanceY = Vector3.Distance(new Vector3(0f, Screen.height / 2, 0f), new Vector3(0f, screenPos.y, 0f));

        if (distanceX - screenOffset > Screen.width / 2 || distanceY - screenOffset > Screen.height / 2)
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
            bossSpawner.bossAnimator.SetTrigger("Dead");
            StopAllCoroutines();
            MoveToTarget(transform.position, transform.position);
            dead = true;
            Time.timeScale = 1;
            GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossDeadSound);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && bossSpawner.canBeDamaged && PlayerController.global.damageEnemy && crashed)
            {
                GameObject tempVFX = Instantiate(PlayerController.global.swordVFX.gameObject, ((PlayerController.global.transform.position + transform.position) / 2) + PlayerController.global.transform.forward, Quaternion.identity);
                tempVFX.GetComponent<VisualEffect>().Play();
                Destroy(tempVFX, 1.0f);
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.BirdBossHit1Sound : GameManager.global.BirdBossHit2Sound, 1f, true, 0, false, transform);
                bossSpawner.canBeDamaged = false;
                StopAllCoroutines();
                ScreenShake.global.ShakeScreen();
                Damaged(PlayerController.global.attackDamage);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (other.gameObject.tag == "Arrow" && other.GetComponent<ArrowTrigger>() && (crashed || vulnerable))
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                if (vulnerable)
                {
                    hitReceived++;
                }
                GameManager.global.SoundManager.PlaySound(Random.Range(1, 3) == 1 ? GameManager.global.BirdBossHit1Sound : GameManager.global.BirdBossHit2Sound, 1f, true, 0, false, transform);
                other.GetComponent<ArrowTrigger>().singleHit = true;
                Damaged(PlayerController.global.bowDamage);
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
                }
            }
        }
        if (other.gameObject == PlayerController.global.gameObject && vulnerable && !hitOnce)
        {
            PlayerController.global.TakeDamage(15.0f);
            hitOnce = true;
        }
    }

    // Boss starts flying through this anim event and then only uses triggers
    private void StartFlyingAnimEvent()
    {
        hitReceived = 0;
        crashed = false;
        altitudeReached = false;
        bossSpawner.bossAnimator.SetBool("Flying", true);
        flying = true;
    }

    private void StopMovementAnimEvent()
    {
        stopMoving = true;
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossPreAttack1Sound);
    }

    private void StartMovementAnimEvent()
    {
        stopMoving = false;
        flying = false;
        diving = true;
    }

    private void RecoverSoundAnimEvent()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossEncounterSound, 1f, true, 0, false, transform);
    }

    private void Wing1SoundAnimEvent()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossWing1Sound);
    }

    private void Wing2SoundAnimEvent()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossWing2Sound);
    }
}