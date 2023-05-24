using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltScript : MonoBehaviour
{
    public float speed = 10f;    // Speed at which the bullet moves
    public float lifetime = 2f;  // Time in seconds before the bullet is destroyed
    public int damage = 10;     // Amount of damage the bullet applies to enemies

    private float timer;        // Timer to track the bullet's lifetime

    private void Start()
    {
        timer = lifetime;       // Initialize the timer to the bullet's lifetime
    }

    // Update is called once per frame
    void Update()
    {
        // Move the bullet forward along the Z-axis
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        timer -= Time.deltaTime; // Decrease the timer based on the elapsed time

        // Destroy the bullet if the timer reaches or goes below zero
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Retrieve the Enemy component from the collided object
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Damaged(damage); // Apply damage to the enemy
            }

            Destroy(gameObject); // Destroy the bullet
        }
    }
}
