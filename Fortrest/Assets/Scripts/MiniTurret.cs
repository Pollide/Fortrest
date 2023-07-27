using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTurret : MonoBehaviour
{
    public Animator animator;
    public float turnSpeed = 20.0f;
    public float shootingRange = 7.5f;
    private float shootTimer = 0.0f;
    private float resetShoot = 0.05f;
    private bool shooting = false;
    public float damage = 1.0f;
    public LayerMask targetLayer;
    private float turningValue;

    private Transform target;
    private Transform currentTarget;
    private bool targetAcquired;
    private bool spawnComplete;

    private void Start()
    {
        GameManager.PlayAnimation(GetComponent<Animation>(), "Turret Shoot");        
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
            FindTarget();

            if (target != null)
            {
                if (currentTarget)
                {
                    if (currentTarget != target)
                    {
                        targetAcquired = false;
                        turningValue = 0;
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

                turningValue += Time.deltaTime / 20.0f;

                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turnSpeed * turningValue);

                if (Vector3.Distance(transform.position, target.transform.position) <= shootingRange)
                {
                    if (!shooting)
                    {
                        Attack();
                    }
                }

            }
            else
            {
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
                Destroy(gameObject);
            }
        }       
    }

    private void Attack()
    {
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
