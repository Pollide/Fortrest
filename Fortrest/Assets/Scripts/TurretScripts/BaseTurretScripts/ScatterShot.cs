using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterShot : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f; // Bullet lifetime in seconds
    public float bulletDamage = 0.2f; // Bullet Damage
    public Transform[] spawnPositions; // Array of designated spawn positions
    public float cooldownTime = 0.5f; // Cooldown time in seconds
    private float cooldownTimer = 0f;
    Building BuildingScript;
    private void Start()
    {
        BuildingScript = GetComponentInParent<Building>();
    }

    private void Update()
    {
        if (BuildingScript)
        {
            // Rotate the GameObject around its up axis
            transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);

            // Update the cooldown timer
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                ShootBulletFromNextSpawnPoint();
                cooldownTimer = cooldownTime;
            }
        }
    }

    private void ShootBulletFromNextSpawnPoint()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            // Get the next designated spawn position
            Vector3 spawnPosition = spawnPositions[i].position;

            // Instantiate the bullet at the designated spawn position
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // Calculate the direction for the bullet to travel
            Vector3 spawnDirection = (spawnPosition - transform.position).normalized;

            // Shoot the bullet in the spawn direction
            bullet.GetComponent<Rigidbody>().velocity = spawnDirection * bulletSpeed;

            bullet.transform.localScale = new(0.3f, 0.3f, 0.3f);

            bullet.GetComponent<ScattershotBullet>().SetParent(transform.parent.gameObject);
            bullet.GetComponent<ScattershotBullet>().SetDamage(bulletDamage);

            // Destroy the bullet after the specified lifetime
            Destroy(bullet, bulletLifetime);
        }
    }
}
