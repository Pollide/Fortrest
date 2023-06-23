using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlow : MonoBehaviour
{
    public float radius = 5f; // Radius within which enemies are affected
    public float enemySpeedPercentage = 0.5f; // Amount to slow down enemies (0.5 represents 50% slower)

    private void Start()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to an enemy (you might want to change this based on your game)
        EnemyController enemy = other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.SlowSound);
            enemy.ApplySlow(enemySpeedPercentage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to an enemy 
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.RemoveSlow();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the editor to visualize the tower's radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
