using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent; // Nav mesh agent component
    private Transform bestTarget; // Target that the enemy will go towards
    public bool chasing;
    private Transform playerPosition;

    private float attackTimer;
    private float attackTimerMax;
    private float speed;

    private float health;
    private float maxHealth = 3.0f;
    public Image healthBarImage;
    AnimationState HealthAnimationState;

    private float noiseTimer;
    private float noiseTimerMax;
    private float chaseTimer = 0.0f;
    private float chaseTimerMax;

    public Animator ActiveAnimator;
    public bool canBeDamaged = true;

    KnockBack knockBackScript;
    private float offset = 0.3f;

    public bool firstAttack = true;

    void Start()
    {
        noiseTimerMax = 2.5f;
        attackTimerMax = 1.75f;
        chaseTimerMax = 10.0f;
        health = maxHealth;
        playerPosition = PlayerController.global.transform;
        agent = GetComponent<NavMeshAgent>(); // Finds the component by itself on the object the script is attached to
        speed = agent.speed;
        PlayerController.global.enemyList.Add(transform); // Adding each object transform with this script attached to the enemy list
        Indicator.global.AddIndicator(transform);
        GameManager.ChangeAnimationLayers(healthBarImage.transform.parent.parent.GetComponent<Animation>());
        knockBackScript = GetComponent<KnockBack>();
    }

    void Update()
    {
        Checks();
        MakeNoise();
        Process();
    }

    void Process()
    {
        // Default value
        float shortestDistance = 9999;            

        // If enemy is chasing the player, they become its target
        if (chasing)
        {
            bestTarget = playerPosition; // Player set as target

            // Distance between enemy and player
            float distance = Vector3.Distance(PlayerController.global.transform.position, transform.position);

            chaseTimer += Time.deltaTime; // Enemy stops chasing the player after 10s or when the player gets too far

            if (chaseTimer >= chaseTimerMax || distance >= 10.0f)
            {
                bestTarget = null;
                chasing = false;
                firstAttack = true;
                chaseTimer = 0;
            } 
        }
        else // Player is not the target, finds different target
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
        }

        // Once a target is set, adjust stopping distance, check distance, attack when reaching target
        if (bestTarget)
        {
            if (bestTarget == playerPosition)
            {
                agent.stoppingDistance = 2.0f;
            }
            else
            {
                agent.stoppingDistance = 4.5f;
            }
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(bestTarget.position); // Makes the enemy move
            }

            if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + offset) // Checks if enemy reached target
            {
                FaceTarget(); // Makes the enemy face the player
                if (firstAttack)
                {
                    attackTimer += 1.5f;
                    firstAttack = false;
                }
                attackTimer += Time.deltaTime;

                if (attackTimer >= attackTimerMax)
                {
                    Attack();                   
                }
            }
            ActiveAnimator.SetBool("Moving", Vector3.Distance(transform.position, bestTarget.position) > agent.stoppingDistance + offset);

            if (Vector3.Distance(transform.position, bestTarget.position) >= 5.0f)
            {
                firstAttack = true;
            }
        }        
    }

    void Checks()
    {
        if (!knockBackScript)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            PlayerController.global.enemyList.Remove(transform);
            gameObject.SetActive(false);
            return;
        }
    }

    void Attack()
    {
        ActiveAnimator.ResetTrigger("Swing");
        ActiveAnimator.SetTrigger("Swing");
        GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyAttack1Sound : GameManager.global.EnemyAttack2Sound, 1, true, 0, false, transform);
        attackTimer = 0;

        if (bestTarget == playerPosition)
        {            
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerHitSound, 0.5f, true, 0, false, playerPosition);
            playerPosition.GetComponent<PlayerController>().playerEnergy -= 5;
        }
        else
        {
            Building building = bestTarget.GetComponent<Building>();
            if (building.GetHealth() > 0)
            {
                building.healthBarImage.fillAmount = Mathf.Clamp(building.GetHealth() / building.maxHealth, 0, 1f);
                building.TakeDamage(1f);
            }
            else
            {
                LevelManager.global.BuildingList.Remove(bestTarget); // Removes target from list
                building.DestroyBuilding();
            }
        }
    }

    private void FaceTarget() // Making sure the enemy always faces what it is attacking
    {
        Vector3 direction = (bestTarget.position - transform.position).normalized; // Gets a direction using a normalized vector
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Obtaining a rotation angle
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); // Smoothly rotating towards target
    }

    public void Damaged(float amount)
    {
        Animation animation = healthBarImage.transform.parent.parent.GetComponent<Animation>();
        health -= amount;
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
            Time.timeScale = 1;
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyDead1Sound : GameManager.global.EnemyDead2Sound, 1, true, 0, false, transform);
            PlayerController.global.enemyList.Remove(transform);
            agent.enabled = false;
            LevelManager.global.enemyList.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void MakeNoise()
    {
        noiseTimer += Time.deltaTime;

        if (noiseTimer >= noiseTimerMax)
        {
            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.Enemy1Sound : GameManager.global.Enemy2Sound, 1, true, 0, false, transform);
            noiseTimer = 0;
            noiseTimerMax = Random.Range(5.0f, 10.0f);
        }
    }

    public void ApplySlow(float _slowPercent)
    {
        agent.speed *= _slowPercent;
    }

    public void RemoveSlow()
    {
        agent.speed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && canBeDamaged && PlayerController.global.attackTimer > 0.2f && PlayerController.global.attackTimer < 0.7f)
            {
                StopAllCoroutines();
                knockBackScript.knock = true;
                canBeDamaged = false;               
                ScreenShake.global.shake = true;
                chasing = true;
                Damaged(PlayerController.global.attackDamage);
                GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.EnemyHit1Sound : GameManager.global.EnemyHit2Sound);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
    }
}