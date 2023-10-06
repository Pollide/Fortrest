using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    // Update is called once per frame
    public GameObject DeathVFX;
    float idleThreshold;
    float idleTimer;

    void Update()
    {
        float distance = PlayerController.global ? Vector3.Distance(transform.position, PlayerController.global.transform.position) : 999999;
        idleTimer += Time.deltaTime;
        bool run = distance < 12;

        if (idleTimer > idleThreshold || run)
        {
            idleThreshold = Random.Range(2, 5);
            idleTimer = 0;

            float range = 5;
            float x = Random.Range(-range, range);
            float z = Random.Range(-range, range);

            Vector3 destination = transform.position + new Vector3(x, 0f, z);

            if (run)
            {
                destination += transform.position - PlayerController.global.transform.position;
            }

            navMeshAgent.SetDestination(destination);
        }



        animator.SetBool("Run", 1 < Vector3.Distance(transform.position, navMeshAgent.destination));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject || other.gameObject.tag == "Arrow")
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.ArrowHitBuildingSound, 1.0f);
            DeathVFX.transform.SetParent(null);
            DeathVFX.SetActive(true);

            Destroy(gameObject);
        }
    }
}