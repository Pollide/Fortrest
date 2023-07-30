using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    public float turn_speed;
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float damage = 10;
    public float explosionRadius = 5;
    public LayerMask targetLayer;

    private Transform target;
    private float fireCountdown = 0f;
    float nextRotationChangeTime;
    private Quaternion targetRotation;
    public bool IsCannon;
    public bool attackStarted;

    [HideInInspector]
    public int CurrentLevel;

    public GameObject ProjectilePrefab;
    public Transform FirePoint;

    private void Start()
    {
        ReturnAnimator();
    }

    public Animator ReturnAnimator()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();

        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].gameObject.SetActive(CurrentLevel == i);
        }

        return animators[CurrentLevel];
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
                Attack();
            }

            if (Vector3.Distance(transform.position, target.position) > shootingRange && !attackStarted)
            {
                target = null;
            }

            if (target && target.GetComponent<EnemyController>().health <= 0f)
            {
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
        // Check if it's time to fire
        if (fireCountdown <= 0f)
        {
            attackStarted = true;
            ReturnAnimator().ResetTrigger("Fire");
            ReturnAnimator().SetTrigger("Fire");
            ProjectileEvent();

            if (IsCannon)
                GameManager.global.SoundManager.PlaySound(GameManager.global.CannonSound);
            else
                GameManager.global.SoundManager.PlaySound(GameManager.global.TurretShootSound);

            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    public void ProjectileEvent()
    {
        GameObject projectile = Instantiate(ProjectilePrefab, FirePoint);

        if (IsCannon)
        {
            // Spawn a projectile
            ProjectileExplosion explosion = projectile.GetComponent<ProjectileExplosion>();
            explosion.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            explosion.explosionRadius = explosionRadius;
            explosion.damage = damage;
        }
        else
        {
            U_Turret uTurret = GetComponent<U_Turret>();
            BoltScript boltScript = projectile.GetComponent<BoltScript>();

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

            //   turret.animController.SetBool("isAttacking", false);
            boltScript.SetDamage(damage);
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
