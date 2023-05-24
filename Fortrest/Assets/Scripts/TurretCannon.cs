using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretCannon : MonoBehaviour
{
    public Transform cannonHead;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireRate = 1f;
    public float shootingRange = 1f;
    public LayerMask targetLayer;

    private Transform target;
    private float fireCountdown = 0f;

    private void Update()
    {
        FindNearestTarget();

        // Check if it's time to fire
        if (fireCountdown <= 0f)
        {
            Fire();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    private void FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, shootingRange, targetLayer);
        float closestDistance = Mathf.Infinity;
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

    private void Fire()
    {
        // Check if there's a target
        if (target == null)
            return;

        // Calculate the direction to the target
        Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        Vector3 direction = targetPos - transform.position;

        // Set the cannon's rotation to aim at the target
        cannonHead.rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Spawn a projectile and set its velocity
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.velocity = firePoint.forward * projectileSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
