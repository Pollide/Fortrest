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
    private float speed = 10.0f;
    private float rotationSpeed = 10.0f;
    public float stoppingDistance;
    public bool outOfScreen;
    public float offset;

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

        if (distanceToPlayer < 20.0f)
        {
            awoken = true;
        }

        if (awoken)
        {
            animator.SetTrigger("TakeOff");
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
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void StartFlying()
    {
        animator.SetBool("Flying", true);
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
}