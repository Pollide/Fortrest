using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileExplosion : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float damage = 0.1f;
    public float pushForce = 5f;
    public GameObject explosionEffect;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Explode();
            Destroy(gameObject);
        }
        
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<EnemyController>())
            {
                collider.GetComponent<EnemyController>().Damaged(damage);
            }
            
            Rigidbody enemyRigidbody = collider.GetComponent<Rigidbody>();
            NavMeshAgent enemyAgent = collider.GetComponent<NavMeshAgent>();

            if (enemyRigidbody != null)
            {
                Vector3 direction = (enemyRigidbody.transform.position - transform.position).normalized;
                enemyRigidbody.AddForce(direction * pushForce, ForceMode.Impulse);
            }

            if (enemyAgent != null)
            {
                Vector3 direction = (enemyAgent.transform.position - transform.position).normalized;
                enemyAgent.velocity = direction * pushForce;
            }

        }

        // Instantiate explosion effect if specified
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }
}
