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
            if (GetComponentInParent<Boar>().IsMoving() == true)
            {
               
                NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();
                EnemyController enemyController = other.GetComponent<EnemyController>();
                float sideThreshold = 0.0f;
                Vector3 direction = (enemyAgent.transform.position - transform.position).normalized;
                Vector3 rightVector = transform.right;
                float dotProduct = Vector3.Dot(direction, rightVector);

                if (dotProduct > sideThreshold)
                {
                    direction = (-enemyAgent.transform.forward + enemyAgent.transform.right).normalized;
                    enemyAgent.velocity = direction * pushForce;
                    StartCoroutine(enemyController.StopAnimation(1));
                }
                else if (dotProduct < -sideThreshold)
                {
                    direction = (-enemyAgent.transform.forward - enemyAgent.transform.right).normalized;
                    enemyAgent.velocity = direction * pushForce;
                    StartCoroutine(enemyController.StopAnimation(1));
                }
                else
                {
                    enemyAgent.velocity = direction * pushForce;
                    StartCoroutine(enemyController.StopAnimation(1));
                }
            } 
        }
    }
}
