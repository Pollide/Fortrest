using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTurret : MonoBehaviour
{
    public Animator animator;
    private float turnSpeed = 10.0f;
    private float shootingRange = 10.0f;
    private float shootTimer = 0.0f;
    private float resetShoot = 0.05f;
    private bool shooting = false;
    [HideInInspector] public float damage = 1.0f;
    public LayerMask targetLayer;

    private Transform target;
    private Transform currentTarget;
    private bool targetAcquired;
    private bool spawnComplete;
    public bool attackStarted;

    private void Start()
    {
        GameManager.PlayAnimation(GetComponent<Animation>(), "MiniTurretSpawn");        
    }

    private void StartBehaving()
    {
        spawnComplete = true;
        animator.speed = 1.5f;
    }

    private void Update()
    {
        if (spawnComplete)
        {           
            if (target != null)
            {
                if (currentTarget)
                {
                    if (currentTarget != target)
                    {
                        targetAcquired = false;
                    }
                }
                
                if (!targetAcquired)
                {
                    currentTarget = target;
                    targetAcquired = true;
                }

                Vector3 targetPos = new(target.transform.position.x, transform.position.y, target.transform.position.z);

                Vector3 direction = targetPos - transform.position;

                Quaternion lookRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, target.transform.position) < shootingRange && !shooting)
                {
                    Attack();
                }

                if (Vector3.Distance(transform.position, target.position) > shootingRange && !attackStarted)
                {
                    if (!GetComponentInChildren<MiniTurretEvents>().boltActive)
                    {
                        target = null;
                    }
                }

                if (target && target.GetComponent<EnemyController>().health <= 0f)
                {                  
                    target = null;
                }
            }
            else
            {
                FindTarget();
                animator.SetBool("isAttacking", false);
            }

            if (shooting)
            {
                shootTimer += Time.deltaTime;

                if (shootTimer >= resetShoot)
                {
                    shooting = false;
                }
            }

            if (PlayerController.global.turretEnd)
            {
                PlayerController.global.turretEnd = false;
                GameManager.PlayAnimation(GetComponent<Animation>(), "MiniTurretDisappear");
            }
        }       
    }

    // Called with above anim
    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void Attack()
    {
        attackStarted = true;
        shooting = true;
        animator.SetBool("isAttacking", true);
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
}
