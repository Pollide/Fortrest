using UnityEngine;
using System.Collections.Generic;

public class Defence : MonoBehaviour
{
    public List<Sprite> spriteTierList = new List<Sprite>();
    public float turn_speed;
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float damage = 10;

    public TurretStats.Tier changeTier = new TurretStats.Tier();



    public LayerMask targetLayer;

    private Transform target;
    float fireCountdown = 0f;
    float nextRotationChangeTime;
    public bool attackStarted;

    [Header("Cannon")]
    public float explosionRadius = 5;

    [Header("Slow")]
    public float enemySpeedPercentage = 0.5f; // Amount to slow down enemies (0.5 represents 50% slower)

    [Header("Scatter Shot")]
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f; // Bullet lifetime in seconds
    public float bulletDamage = 0.2f; // Bullet Damage
    public Transform[] spawnPositions; // Array of designated spawn positions
    public float cooldownTime = 0.5f; // Cooldown time in seconds
    private float cooldownTimer = 0f;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public Transform FirePoint;

    [Header("Other")]
    public Transform ModelHolder;

    [HideInInspector]
    public int CurrentLevel;

    private Quaternion targetRotation;
    public bool MiniTurret;
    public Animator MiniTurretAnimator;
    Building building;
    private void Start()
    {
        building = GetComponent<Building>();
        ReturnAnimator();
    }

    public Animator ReturnAnimator()
    {
        for (int i = 0; i < ModelHolder.childCount; i++)
        {
            ModelHolder.GetChild(i).gameObject.SetActive(!MiniTurret && CurrentLevel == i);
        }


        Animator animator = ModelHolder.GetChild(CurrentLevel).GetComponent<Animator>();
        if (MiniTurret)
        {
            animator = MiniTurretAnimator;
            MiniTurretAnimator.gameObject.SetActive(true);
        }

        if (animator)
        {
            animator.speed = fireRate;
        }

        return animator;
    }

    private void Update()
    {
        if (building.buildingObject == Building.BuildingType.Scatter)
        {
            // Rotate the GameObject around its up axis
            transform.Rotate(turn_speed * Time.deltaTime * Vector3.up);

            // Update the cooldown timer
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                ShootBulletFromNextSpawnPoint();
                cooldownTimer = cooldownTime;
            }

        }
        else if (target != null)
        {
            Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

            Vector3 direction = targetPos - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turn_speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.transform.position) <= shootingRange)
            {

                if (building.buildingObject == Building.BuildingType.Slow)
                {
                    target.GetComponent<EnemyController>().ApplySlow(enemySpeedPercentage);
                }
                else
                {
                    Attack();
                }
            }

            if (Vector3.Distance(transform.position, target.position) > shootingRange && !attackStarted || target.GetComponent<EnemyController>().health <= 0f)
            {
                if (building.buildingObject == Building.BuildingType.Slow)
                {
                    target.GetComponent<EnemyController>().RemoveSlow();
                }

                target = null;
            }
        }
        else
        {
            FindTarget();

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

    private void ShootBulletFromNextSpawnPoint()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            // Get the next designated spawn position
            Vector3 spawnPosition = spawnPositions[i].position;

            // Instantiate the bullet at the designated spawn position
            GameObject bullet = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);

            // Calculate the direction for the bullet to travel
            Vector3 spawnDirection = (spawnPosition - transform.position).normalized;

            // Shoot the bullet in the spawn direction
            bullet.GetComponent<Rigidbody>().velocity = spawnDirection * bulletSpeed;

            bullet.transform.localScale = new(0.3f, 0.3f, 0.3f);

            bullet.transform.SetParent(LevelManager.global.transform);
            bullet.GetComponent<ScattershotBullet>().SetDamage(bulletDamage);

            // Destroy the bullet after the specified lifetime
            Destroy(bullet, bulletLifetime);
        }
    }
    private void Attack()
    {
        if (target.GetComponent<EnemyController>())
        {
            if (target.GetComponent<EnemyController>().health - damage <= 0)
            {
                target.gameObject.layer = 0; //so it doesnt get detected
            }
        }
        Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

        Vector3 direction = targetPos - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Check if it's time to fire
        if (fireCountdown <= 0f && Mathf.Abs(Quaternion.Dot(transform.rotation, lookRotation)) > 0.8f)
        {
            attackStarted = true;
            ReturnAnimator().ResetTrigger("Fire");
            ReturnAnimator().SetTrigger("Fire");

            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    public void ProjectileEvent() //CALLS ON THE ANIMATOR
    {
        GameObject projectile = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
        //       GameObject projectile = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);


        BoltScript boltScript = projectile.GetComponent<BoltScript>();

        if (boltScript)
        {
            boltScript.turretShootingScript = this;
            boltScript.ActiveTarget = target;
            boltScript.SetDamage(damage);
        }

        if (building.buildingObject == Building.BuildingType.Cannon)
        {

            GameManager.global.SoundManager.PlaySound(GameManager.global.CannonShootSound, 0.3f, true, 0, false, transform);
            // Spawn a projectile
            ProjectileExplosion explosion = projectile.GetComponent<ProjectileExplosion>();
            //  explosion.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            explosion.explosionRadius = explosionRadius;
            explosion.damage = damage;
            explosion.uCannon = GetComponent<U_Cannon>();
        }
        else
        {
            if (!MiniTurret)
            {
                projectile.transform.localScale *= 2.5f;
            }
            // Debug.Log(projectile);
            GameManager.global.SoundManager.PlaySound(GameManager.global.BallistaShootSound, 0.3f, true, 0, false, transform);
            U_Turret uTurret = GetComponent<U_Turret>();

            if (uTurret && uTurret.isMultiShotActive)
            {
                float range = Random.Range(0f, 101);
                if (range <= uTurret.multiShotPercentage)
                {
                    GameObject bolt2 = Instantiate(ProjectilePrefab, FirePoint);
                    bolt2.GetComponent<BoltScript>().SetDamage(damage / 2f);
                    bolt2.transform.Rotate(new Vector3(0, 25, 0));
                    GameObject bolt3 = Instantiate(ProjectilePrefab, FirePoint);
                    bolt3.GetComponent<BoltScript>().SetDamage(damage / 2f);
                    bolt3.transform.Rotate(new Vector3(0, -25, 0));
                }
            }
        }
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
