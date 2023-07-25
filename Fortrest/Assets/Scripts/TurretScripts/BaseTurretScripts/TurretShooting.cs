using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    public Animator animController;
    public float turn_speed;
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float damage = 10;
    public LayerMask targetLayer;

    private Transform target;
    private float fireCountdown = 0f;

    private void Update()
    {
        FindTarget();

        if (target != null)
        {
            Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

            Vector3 direction = targetPos - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turn_speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.transform.position) <= shootingRange)
            {
                Attack();
            }

        }
        else
        {
            animController.SetBool("isAttacking", false);
        }
    }

    private void Attack()
    {
        // Check if it's time to fire
        if (fireCountdown <= 0f)
        {
            animController.SetBool("isAttacking", true);
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, shootingRange, targetLayer);
        float closestDistance = shootingRange;
        Transform closestTarget = null;

        foreach (Collider collider in colliders)
        {
            Transform target = collider.transform;
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        target = closestTarget;
    }


    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
