using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Agent components
    private NavMeshAgent agent; // Nav mesh agent component
    private Transform bestTarget; // Target that the enemy will go towards
    private Transform playerPosition;

    // Parameters
    private float offset;
    private float speed;

    // Timers
    private float attackTimer;
    private float attackTimerMax;
    private float noiseTimer;
    private float noiseTimerMax;
    private float chaseTimer;
    private float chaseTimerMax;

    // Health
    private float health;
    private float maxHealth;
    public Image healthBarImage;
    AnimationState HealthAnimationState;

    // Booleans
    public bool chasing = false;
    public bool canBeDamaged = true;
    private bool firstAttack = true;

    // Others
    public Animator ActiveAnimator; 
    KnockBack knockBackScript;

    private enum ENEMYTYPE
    {
        goblin = 1,
        spider,
        wolf
    };

    // Audio
    public AudioClip hitSound;
    public AudioClip hitSound2;
    public AudioClip attackSound;
    public AudioClip attackSound2;
    public AudioClip deathSound;
    public AudioClip deathSound2;
    public AudioClip noiseSound;
    public AudioClip noiseSound2;

    [SerializeField] ENEMYTYPE currentEnemyType;

    void Start()
    {
        noiseTimerMax = 2.5f;
        chaseTimerMax = 10.0f;

        agent = GetComponent<NavMeshAgent>(); 
        SetEnemyParameters();
        
        playerPosition = PlayerController.global.transform;
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

        chasing = true;
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
                    attackTimer = attackTimerMax - (attackTimerMax / 10.0f);
                    firstAttack = false;
                }
                attackTimer += Time.deltaTime;

                if (attackTimer >= attackTimerMax)
                {
                    Attack();                   
                }
            }
            ActiveAnimator.SetBool("Moving", Vector3.Distance(transform.position, bestTarget.position) > agent.stoppingDistance + offset);

            if (Vector3.Distance(transform.position, bestTarget.position) >= 2.5f)
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
        ActiveAnimator.ResetTrigger("Attack");
        ActiveAnimator.SetTrigger("Attack");
        if (currentEnemyType == ENEMYTYPE.goblin) // Temporary
            PickSound(attackSound, attackSound2);
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
            if (currentEnemyType == ENEMYTYPE.goblin) // Temporary
                PickSound(deathSound, deathSound);

            Time.timeScale = 1;
            
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
            if (currentEnemyType == ENEMYTYPE.goblin) // Temporary
                PickSound(noiseSound, noiseSound2);
   
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
                if (currentEnemyType == ENEMYTYPE.goblin) // Temporary
                    PickSound(hitSound, hitSound2);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
    }

    private void SetEnemyParameters()
    {
        if (currentEnemyType == ENEMYTYPE.goblin)
        {
            agent.speed = 3.0f;
            agent.acceleration = 20.0f;
            agent.angularSpeed = 120.0f;
            maxHealth = 3.0f;
            attackTimerMax = 1.75f;
            offset = 0.3f;
        }
        else if (currentEnemyType == ENEMYTYPE.spider)
        {
            agent.speed = 4.0f;
            agent.acceleration = 50.0f;
            agent.angularSpeed = 200.0f;
            maxHealth = 2.0f;
            attackTimerMax = 2.5f;
            offset = 0.5f;
        }
        else if (currentEnemyType == ENEMYTYPE.wolf)
        {
            agent.speed = 5.0f;
            agent.acceleration = 40.0f;
            agent.angularSpeed = 100.0f;
            maxHealth = 4.0f;
            attackTimerMax = 2.5f;
            offset = 0.75f;
        }
        health = maxHealth;
        speed = agent.speed;
    }

    private void PickSound(AudioClip name1, AudioClip name2)
    {
        GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? name1 : name2, 1, true, 0, false, transform);
    }
}