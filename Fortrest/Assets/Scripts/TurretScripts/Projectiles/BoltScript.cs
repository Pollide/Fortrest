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
    public TurretShooting turretShootingScript;

    [HideInInspector]
    public Transform ActiveTarget;

    private bool instakill;
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
            U_Turret uTurret = turretShootingScript.GetComponent<U_Turret>();

            if (other.CompareTag("Enemy"))
            {
                Debug.Log("yoza");
                // Retrieve the Enemy component from the collided object
                EnemyController enemy = other.GetComponent<EnemyController>();
                NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();
                if (enemy != null)
                {
                    if (!turretShootingScript.MiniTurret)
                    {
                        if (uTurret.isKnockBackActive)
                        {
                            float randomRange = Random.Range(0f, 100f);
                            if (randomRange <= uTurret.knockBackPercentage)
                            {
                                Vector3 direction = (enemyAgent.transform.position - transform.position).normalized;
                                enemyAgent.velocity = direction * pushForce;
                            }
                        }

                        if (uTurret.isInstantKillPercent)
                        {
                            instakill = true;
                            float randomRange = Random.Range(0f, 100f);
                            if (randomRange <= uTurret.instantKillPercent)
                            {
                                enemy.Damaged(5000); // Apply damage to the enemy
                                damageDealt = true;
                            }
                            else
                            {
                                enemy.Damaged(damage); // Apply damage to the enemy
                                damageDealt = true;
                            }

                        }
                    }

                    if (!instakill)
                    {
                        enemy.Damaged(damage); // Apply damage to the enemy
                        damageDealt = true;
                    }

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
