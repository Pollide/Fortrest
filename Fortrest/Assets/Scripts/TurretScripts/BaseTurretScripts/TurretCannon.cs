using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretCannon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float shootingRange = 1f;
    public LayerMask targetLayer;
    public float explosionRadius = 5f;
    public float damage = 0.1f;
    private Transform target;
    private float fireCountdown = 0f;
    float nextRotationChangeTime;
    private Quaternion targetRotation;

    private void Update()
    {
        FindNearestTarget();

        // Check if it's time to fire
        if (fireCountdown <= 0f)
        {
            Fire();
            fireCountdown = 1f / fireRate;
        }

        if (target)
        {
            // Calculate the direction to the target
            Vector3 targetPos = new(target.transform.position.x, target.transform.position.y, target.transform.position.z);

            Vector3 direction = targetPos - transform.position;

            // Set the cannon's rotation to aim at the target
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
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
        fireCountdown -= Time.deltaTime;
    }

    private void FindNearestTarget()
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

    private void Fire()
    {
        // Check if there's a target
        if (target == null)
            return;
        GameManager.global.SoundManager.PlaySound(GameManager.global.CannonSound);
        GameManager.PlayAnimation(GetComponent<Animation>(), "Turret Shoot");
        // Spawn a projectile
        ProjectileExplosion projectile = Instantiate(projectilePrefab, firePoint).GetComponent<ProjectileExplosion>();
        projectile.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        projectile.explosionRadius = explosionRadius;
        projectile.damage = damage;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
