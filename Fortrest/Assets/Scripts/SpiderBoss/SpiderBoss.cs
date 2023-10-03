using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBoss : MonoBehaviour
{
    private Transform playerTransform;
    private float awakeRange;
    private Animator animator;
    private bool awoken;
    private float arenaSize;
    public Vector3 startPosition;
    public bool retreating;
    private NavMeshAgent agent;
    private float damage;

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
    }

    // Update is called once per frame
    void Update()
    {
        // Spider awakes when player gets close to it
        if (Vector3.Distance(playerTransform.position, transform.position) <= awakeRange && !awoken)
        {
            animator.SetTrigger("Awaking");
            awoken = true;
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
        animator.SetTrigger("Attack1");
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
}