using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoltScript : MonoBehaviour
{
    public float speed = 10f;       // Speed at which the bullet moves
    public float lifetime = 2f;     // Time in seconds before the bullet is destroyed
    public float pushForce = 5f;    // Time in seconds before the bullet is destroyed

    private float damage = 0f;      // Amount of damage the bullet applies to enemies
    private float timer = 0f;       // Timer to track the bullet's lifetime

    public MiniTurretEvents miniTurretEventsScript;
    public MiniTurret miniTurretScript;
    public TurretShooting turretShootingScript;

    private void Start()
    {
        miniTurretEventsScript = transform.parent.parent.GetChild(0).GetComponent<MiniTurretEvents>();
        miniTurretScript = transform.parent.parent.GetComponent<MiniTurret>();
        turretShootingScript = transform.parent.parent.GetComponent<TurretShooting>();
        timer = lifetime;           // Initialize the timer to the bullet's lifetime
    }

    public bool mini;
    private bool instakill;
    private bool damageDealt;

    // Update is called once per frame
    void Update()
    {
        // Move the bullet forward along the Z-axis
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        timer -= Time.deltaTime;    // Decrease the timer based on the elapsed time

        // Destroy the bullet if the timer reaches or goes below zero
        if (timer <= 0f)
        {
            if (mini)
            {
                miniTurretEventsScript.boltActive = false;
                miniTurretScript.attackStarted = false;
                turretShootingScript.attackStarted = false;
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!damageDealt)
        {
            U_Turret uTurret = GetComponentInParent<U_Turret>();

            if (other.CompareTag("Enemy"))
            {
                // Retrieve the Enemy component from the collided object
                EnemyController enemy = other.GetComponent<EnemyController>();
                NavMeshAgent enemyAgent = other.GetComponent<NavMeshAgent>();
                if (enemy != null)
                {
                    if (!mini)
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

                if (mini)
                {
                    miniTurretEventsScript.boltActive = false;
                    miniTurretScript.attackStarted = false;
                    turretShootingScript.attackStarted = false;
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
