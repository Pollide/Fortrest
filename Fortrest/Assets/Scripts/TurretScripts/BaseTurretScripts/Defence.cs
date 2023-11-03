using UnityEngine;
using System.Collections.Generic;

public class Defence : MonoBehaviour
{
    public List<Sprite> spriteTierList = new List<Sprite>();
    public float turn_speed;
    public float shootingRange;
    public float fireRate;
    public float damage;
    public int turretID;
    public TurretStats.Tier changeTier = new TurretStats.Tier();
    public GameObject rangeIndicator;

    public float ReturnDamage()
    {
        return damage + changeTier.damageTier;
    }

    public float ReturnHealth(bool forceMax = false)
    {
        return ((GetComponent<Building>().health == 0 || forceMax) ? GetComponent<Building>().maxHealth + changeTier.healthTier : GetComponent<Building>().health);
    }


    public float ReturnSpeed()
    {
        return fireRate + changeTier.rateTier;
    }

    public float ReturnRange()
    {
        return shootingRange + changeTier.rangeTier;
    }

    public LayerMask targetLayer;

    private Transform target;
    float fireCountdown = 0f;
    float nextRotationChangeTime;

    [Header("Cannon")]
    public float explosionRadius = 5;

    [Header("Slow")]
    public float enemySpeedPercentage = 0.5f; // Amount to slow down enemies (0.5 represents 50% slower)

    [Header("Scatter Shot")]
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f; // Bullet lifetime in seconds
    public Transform[] spawnPositions; // Array of designated spawn positions
    public float cooldownTime = 1f; // Cooldown time in seconds
    float cooldownTimer = 0f;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public Transform FirePoint;

    [Header("Other")]
    public Transform ModelHolder;

    //  [HideInInspector]
    public int CurrentTier;

    public Animator TierOneAnimator;
    public Animator TierTwoAnimator;

    private Quaternion targetRotation;
    public bool MiniTurret;
    public Animator MiniTurretAnimator;
    Building building;
    private void Start()
    {
        building = GetComponent<Building>();

        if (ReturnAnimator() && !MiniTurret)
            ReturnAnimator().SetTrigger("Deploy");
    }

    public Animator ReturnAnimator()
    {
        for (int i = 0; i < ModelHolder.childCount; i++)
        {
            ModelHolder.GetChild(i).gameObject.SetActive(false);
        }

        Animator animator = TierOneAnimator;

        if (TierTwoAnimator && CurrentTier > 0)
            animator = TierTwoAnimator;

        if (MiniTurret)
        {
            animator = MiniTurretAnimator;
        }

        if (animator)
        {
            animator.speed = ReturnSpeed();
        }

        animator.gameObject.SetActive(true);
        return animator;
    }

    private void Update()
    {
        rangeIndicator.transform.localScale = new Vector3(ReturnRange() / 5, ReturnRange() / 5, ReturnRange() / 5);

        if (PlayerModeHandler.global.inTheFortress)
        {
            rangeIndicator.SetActive(true);
        }
        else
        {
            rangeIndicator.SetActive(false);
        }

        if (building.buildingObject == Building.BuildingType.Scatter)
        {
            // Rotate the GameObject around its up axis
            transform.Rotate(turn_speed * Time.deltaTime * Vector3.up);

            // Update the cooldown timer
            cooldownTimer -= ReturnSpeed() * Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                ShootBulletFromNextSpawnPoint();
                cooldownTimer = cooldownTime;
            }

        }
        else if (target != null && target.GetComponent<EnemyController>())
        {
            Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

            Vector3 direction = targetPos - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turn_speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.transform.position) <= ReturnRange())
            {

                if (building.buildingObject == Building.BuildingType.Slow)
                {
                    target.GetComponent<EnemyController>().ApplySlow(enemySpeedPercentage, transform);
                }
                else
                {
                    Attack();
                }
            }

            if (Vector3.Distance(transform.position, target.position) > ReturnRange() || target.GetComponent<EnemyController>().health <= 0f)
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
            spawnDirection.y = 0;
            // Shoot the bullet in the spawn direction
            bullet.GetComponent<Rigidbody>().velocity = spawnDirection * bulletSpeed;

            bullet.transform.localScale = new(0.3f, 0.3f, 0.3f);

            bullet.transform.SetParent(LevelManager.global.transform);
            bullet.GetComponent<ScattershotBullet>().SetDamage(ReturnDamage());

            // Destroy the bullet after the specified lifetime
            Destroy(bullet, bulletLifetime);
        }
    }

    float lookingDot;
    private void Attack()
    {
        if (target.GetComponent<EnemyController>())
        {
            if (target.GetComponent<EnemyController>().health - ReturnDamage() <= 0)
            {
                target.gameObject.layer = 0; //so it doesnt get detected
            }
        }
        Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

        Vector3 direction = targetPos - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookingDot = Mathf.Abs(Quaternion.Dot(transform.rotation, lookRotation));
        // Check if it's time to fire


        if (fireCountdown >= 1.5f) //0.5f second gives time to actually shoot
        {
            if (lookingDot > 0.8f)
            {
                fireCountdown = 0;
                ReturnAnimator().ResetTrigger("Fire");
                ReturnAnimator().SetTrigger("Fire");
            }
        }
        fireCountdown += ReturnSpeed() * Time.deltaTime; ///fire rate internally is just called speed

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
            boltScript.SetDamage(ReturnDamage());
        }

        if (building.buildingObject == Building.BuildingType.Cannon)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.CannonShootSound, 0.3f, true, 0, false, transform);
            boltScript.explosionRadius = explosionRadius;
        }
        else
        {
            if (!MiniTurret)
            {
                projectile.transform.localScale *= 2.5f;
            }
            // Debug.Log(projectile);
            GameManager.global.SoundManager.PlaySound(GameManager.global.BallistaShootSound, 0.3f, true, 0, false, transform);

            if (CurrentTier > 1)
            {
                // float range = Random.Range(0f, 101);
                // // if (range <= 50)
                //   {
                GameObject bolt2 = Instantiate(ProjectilePrefab, FirePoint);
                bolt2.GetComponent<BoltScript>().SetDamage(ReturnDamage() / 2f);
                bolt2.transform.Rotate(new Vector3(0, 25, 0));
                GameObject bolt3 = Instantiate(ProjectilePrefab, FirePoint);
                bolt3.GetComponent<BoltScript>().SetDamage(ReturnDamage() / 2f);
                bolt3.transform.Rotate(new Vector3(0, -25, 0));
                //   }
                // }
            }
        }
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ReturnRange(), targetLayer);
        float closestDistance = ReturnRange();
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
        Gizmos.DrawWireSphere(transform.position, ReturnRange());
    }
}
