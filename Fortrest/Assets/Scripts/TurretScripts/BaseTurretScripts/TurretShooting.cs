using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    public float turn_speed;
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float damage = 10;
    public LayerMask targetLayer;

    private Transform target;
    float fireCountdown = 0f;
    float nextRotationChangeTime;
    public bool attackStarted;

    [Header("Cannon Defense")]
    public bool IsCannon;
    public float explosionRadius = 5;

    [Header("Slow Defense")]
    public bool IsSlow;
    public float enemySpeedPercentage = 0.5f; // Amount to slow down enemies (0.5 represents 50% slower)

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

    private void Start()
    {
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
        if (target != null)
        {
            Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

            Vector3 direction = targetPos - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turn_speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.transform.position) <= shootingRange)
            {

                if (IsSlow)
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
                if (IsSlow)
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

        if (IsCannon)
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
            Debug.Log(projectile);
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
