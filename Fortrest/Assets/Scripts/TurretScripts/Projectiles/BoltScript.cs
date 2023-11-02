using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoltScript : MonoBehaviour
{
    public float pushForce = 5f;    // Time in seconds before the bullet is destroyed
    public float speed = 2;
    private float damage = 0f;      // Amount of damage the bullet applies to enemies

    [HideInInspector]
    public Defence turretShootingScript;

    [HideInInspector]
    public Transform ActiveTarget;
    private bool damageDealt;

    public float explosionRadius = 0;
    public GameObject explosionEffect;


    // Update is called once per frame
    void Update()
    {
        // Move the bullet forward along the Z-axis
        if (ActiveTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, ActiveTarget.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(ActiveTarget.position - transform.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!damageDealt && turretShootingScript)
        {
            if (other.CompareTag("Enemy"))
            {
                // Retrieve the Enemy component from the collided object
                EnemyController enemy = other.GetComponent<EnemyController>();
                // NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();
                if (enemy != null)
                {
                    enemy.Damaged(damage); // Apply damage to the enemy
                    damageDealt = true;


                    if (explosionRadius > 0)
                    {
                        if (explosionEffect != null)
                        {
                            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 5);
                        }

                        Explode();
                    }


                    Destroy(gameObject); // Destroy the bullet
                }
            }
        }
    }

    // Sets the damage value
    public void SetDamage(float _damageValue)
    {
        damage = _damageValue;
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<EnemyController>())
            {
                collider.GetComponent<EnemyController>().Damaged(damage * 0.1f);
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

    }
    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
