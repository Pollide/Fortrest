using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BirdBoss : MonoBehaviour
{
    public static BirdBoss global;

    private Transform playerTransform;
    private Animator animator;
    [HideInInspector] public bool awoken;
    private float arenaSize;
    public Vector3 startPosition;
    public bool retreating;
    private NavMeshAgent agent;
    private float damage;
    public bool dead;
    private float speed;
    private float stoppingDistance;
    private float angularSpeed;
    private float acceleration;
    public int stage;
    public bool startIntro;
    public float distanceToPlayer;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        playerTransform = PlayerController.global.transform;
        animator = GetComponent<Animator>();
        awoken = false;
        arenaSize = 50.0f;
        startPosition = transform.position;
        retreating = false;
        agent = animator.GetComponent<NavMeshAgent>();
        damage = 5.0f;
        stage = 1;

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

    public void Awaken()
    {
        startIntro = true;
        awoken = true;
        animator.SetTrigger("Awaking");
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(new Vector3(playerTransform.position.x, 0f, playerTransform.position.z), new Vector3(transform.position.x, 0f, transform.position.z));

        if (distanceToPlayer < 20.0f)
        {
            awoken = true;
        }

        if (awoken)
        {
            animator.SetTrigger("TakeOff");
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void StartFlying()
    {
        animator.SetBool("Flying", true);
    }
}