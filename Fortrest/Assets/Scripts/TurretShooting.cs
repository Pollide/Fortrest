using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    public float turn_speed;
    public Animator animController;
    public float shootingRange = 10f;

    private GameObject target;

    private void Update()
    {
        if (target == null || !target.activeSelf)
        {
            target = FindNearestEnemy();
        }

        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

            Vector3 direction = targetPos - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turn_speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.transform.position) <= shootingRange)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        animController.SetBool("isAttacking", true);
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }


    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
