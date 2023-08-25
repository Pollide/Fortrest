using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoarPushBack : MonoBehaviour
{
    public float pushForce = 5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();
            Vector3 direction = (enemyAgent.transform.position - transform.position).normalized;
            enemyAgent.velocity = direction * pushForce;
        }
    }
}
