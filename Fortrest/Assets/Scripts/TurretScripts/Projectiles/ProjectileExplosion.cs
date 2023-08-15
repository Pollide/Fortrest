using UnityEngine;
using UnityEngine.AI;

public class ProjectileExplosion : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float damage = 0.1f;
    public float pushForce = 5f;
    public GameObject explosionEffect;
    public U_Cannon uCannon;

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
