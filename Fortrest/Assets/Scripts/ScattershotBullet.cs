using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotBullet : MonoBehaviour
{
    private float damage = 0f;      // Amount of damage the bullet applies to enemies

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Retrieve the Enemy component from the collided object
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Damaged(damage);
            }
        }

        if (other.isTrigger == false)
        {
            Destroy(gameObject); // Destroy the bullet
        }
    }

    // Sets the damage value
    public void SetDamage(float _damageValue)
    {
        damage = _damageValue;
    }

}
