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
    float nextRotationChangeTime;
    private Quaternion targetRotation;


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


            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 30 * Time.deltaTime);

            // Check if it's time to change the target rotation
            if (Time.time >= nextRotationChangeTime)
            {
                // Update the target rotation and set the next change time
                float randomRotationY = Random.Range(-180f, 180f); // You can adjust the range as needed
                targetRotation = Quaternion.Euler(0f, randomRotationY, 0f);

                nextRotationChangeTime = Time.time + Random.Range(2, 5);

            }

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
