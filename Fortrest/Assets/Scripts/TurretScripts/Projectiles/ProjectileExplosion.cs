using UnityEngine;
using UnityEngine.AI;

public class ProjectileExplosion : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float damage = 0.1f;
    public float pushForce = 5f;
    public GameObject explosionEffect;
    public float speed = 10f;    // Speed at which the bullet moves
    public float lifetime = 2f;  // Time in seconds before the bullet is destroyed
    private float timer;        // Timer to track the bullet's lifetime


    private void Start()
    {
        timer = lifetime;       // Initialize the timer to the bullet's lifetime
    }


    // Update is called once per frame
    void Update()
    {
        // Move the bullet forward along the Z-axis
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
        timer -= Time.deltaTime; // Decrease the timer based on the elapsed time

        // Destroy the bullet if the timer reaches or goes below zero
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

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
        U_Cannon uCannon = GetComponentInParent<U_Cannon>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<EnemyController>())
            {
                if (uCannon.isInstantKillPercent)
                {
                    float randomRange = Random.Range(0f, 100f);
                    if (randomRange <= uCannon.instantKillPercent)
                    {
                        collider.GetComponent<EnemyController>().Damaged(5000);
                    }
                    else
                    {
                        collider.GetComponent<EnemyController>().Damaged(damage);
                    }
                }
                else
                {
                    collider.GetComponent<EnemyController>().Damaged(damage);
                }
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
    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
