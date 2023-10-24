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
        if (!damageDealt && turretShootingScript && !GetComponent<ProjectileExplosion>())
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

                }

                Destroy(gameObject); // Destroy the bullet
            }
        }
    }

    // Sets the damage value
    public void SetDamage(float _damageValue)
    {
        damage = _damageValue;
    }
}
