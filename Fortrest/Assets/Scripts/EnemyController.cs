using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // public float lookRadius = 10.0f; // Distance that the enemy will start chasing you

    NavMeshAgent agent; // Nav mesh agent component
    [SerializeField] Transform bestTarget; // Target that the enemy will go towards
    public bool chasing; // Enemy chases the player mode
    public bool attacked;
    public bool isInTurretRange = false;
    public bool isDead = false;
    private Transform playerPosition;
    public List<GameObject> turrets = new List<GameObject>();

    public float attackTimer;
    public float attackTimerMax;

    public float health;
    private float maxHealth = 3.0f;
    public Image healthBarImage;
    AnimationState HealthAnimationState;

    public float noiseTimer;
    public float noiseTimerMax;

    public Animator ActiveAnimator;
    void Start()
    {
        noiseTimerMax = 250;
        attackTimer = 200;
        health = maxHealth;
        playerPosition = PlayerController.global.transform;
        agent = GetComponent<NavMeshAgent>(); // Finds the component by itself on the object the script is attached to
        PlayerController.global.enemyList.Add(transform); // Adding each object transform with this script attached to the enemy list
        GameManager.ChangeAnimationLayers(healthBarImage.transform.parent.parent.GetComponent<Animation>());
    }

    void Update()
    {
        float shortestDistance = 9999; // Assign huge default value

        MakeNoise();

        if (LevelManager.global.BuildingList.Count == 0) // Enemies will chase the player if no structure is left around
        {
            chasing = true;
        }

        if (chasing)
        {
            bestTarget = playerPosition;

            if (LevelManager.global.BuildingList.Count != 0) // If there are still targets other than the player
            {
                Invoke("ChasePlayerTimer", 5.0f); // Enemy stops chasing the player after 5s

                float chaseDistance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position); // TEMPORARY EASY CODE

                if (chaseDistance >= 10.0f) // Enemy stops chasing when the player gets too far
                {
                    bestTarget = null;
                    chasing = false;
                }
            }

            if (bestTarget != null)
            {
                agent.SetDestination(bestTarget.position); // Makes the AI move

                if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + 0.3f) // Checks if enemy reached target
                {
                    FaceTarget();
                }
            }
            if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + 0.6f) // Checks if enemy reached target
            {
                attackTimer++;
                if (bestTarget == playerPosition)
                {
                    if (attackTimer >= attackTimerMax)
                    {
                        Attack();
                        GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerHitSound, 0.5f, true, 0, false, playerPosition);
                        playerPosition.GetComponent<PlayerController>().playerEnergy -= 5;
                    }
                }
            }
        }
        else
        {
            if (!bestTarget) // If the enemy does not have a current target
            {
                for (int i = 0; i < LevelManager.global.BuildingList.Count; i++) // Goes through the list of targets
                {
                    if (LevelManager.global.BuildingList[i] != null)
                    {
                        float compare = Vector3.Distance(transform.position, LevelManager.global.BuildingList[i].transform.position); // Distance from enemy to each target

                        if (compare < shortestDistance) // Only true if a new shorter distance is found
                        {
                            shortestDistance = compare; // New shortest distance is assigned
                            bestTarget = LevelManager.global.BuildingList[i].transform; // Enemy's target is now the closest item in the list
                        }
                    }
                }
            }

            else
            {
                agent.SetDestination(bestTarget.position); // Sets the nav mesh agent destination

                if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + 0.6f) // Checks if enemy reached target
                {
                    attackTimer++;
                    Building building = bestTarget.GetComponent<Building>();
                    if (building)
                    {
                        if (building.GetHealth() > 0 && attackTimer >= attackTimerMax)
                        {
                            Attack();
                            building.healthBarImage.fillAmount = Mathf.Clamp(building.GetHealth() / building.maxHealth, 0, 1f);
                            building.TakeDamage(1f);

                        }
                        else if (building.GetHealth() == 0)
                        {
                            RemoveTarget();
                            building.DestroyBuilding();
                        }
                    }
                }
            }
        }

        ActiveAnimator.SetBool("Moving", Vector3.Distance(transform.position, bestTarget.position) > agent.stoppingDistance + 0.6f);
    }

    void Attack()
    {
        ActiveAnimator.ResetTrigger("Swing");
        ActiveAnimator.SetTrigger("Swing");
        attackTimer = 0;
        GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyAttack1Sound : GameManager.global.EnemyAttack2Sound, 1, true, 0, false, transform);
    }

    //private void OnDestroy()
    //{
    //    if (isInTurretRange)
    //    {
    //        isDead = true;
    //        for (int i = 0; i < turrets.Count; i++)
    //        {
    //            turrets[i].GetComponent<TurretShooting>().RemoveFromList();
    //        }
    //    }
    //}

    private void FaceTarget() // Making sure the enemy always faces what it is attacking
    {
        Vector3 direction = (bestTarget.position - transform.position).normalized; // Gets a direction using a normalized vector
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Obtaining a rotation angle
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); // Smoothly rotating towards target
    }

    private void ChasePlayerTimer()
    {
        bestTarget = null;
        chasing = false;
    }

    public void RemoveTarget()
    {
        LevelManager.global.BuildingList.Remove(bestTarget); // Removes target from list
    }

    public void Damaged()
    {
        Animation animation = healthBarImage.transform.parent.parent.GetComponent<Animation>();
        health -= 1;
        if (HealthAnimationState != null && HealthAnimationState.enabled)
        {
            HealthAnimationState.time = 1;
            GameManager.PlayAnimation(animation, "Health Hit");
        }
        else
        {
            HealthAnimationState = GameManager.PlayAnimation(animation, "Health Appear");
        }
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
        if (health <= 0)
        {
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyDead1Sound : GameManager.global.EnemyDead2Sound, 1, true, 0, false, transform);
            PlayerController.global.enemyList.Remove(transform);
            agent.enabled = false;

            if (isInTurretRange)
            {
                isDead = true;
                for (int i = 0; i < turrets.Count; i++)
                {
                    turrets[i].GetComponent<TurretShooting>().RemoveFromList();
                }
            }

            Destroy(gameObject);
        }
    }

    private void MakeNoise()
    {
        noiseTimer++;
        if (noiseTimer >= noiseTimerMax)
        {
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.Enemy1Sound : GameManager.global.Enemy2Sound, 1, true, 0, false, transform);
            noiseTimer = 0;
            noiseTimerMax = Random.Range(500, 1000);
        }
    }
}
