using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Agent components
    private NavMeshAgent agent; // Nav mesh agent component
    public Transform bestTarget; // Target that the enemy will go towards
    private Transform playerPosition;
    GameObject house;

    // Parameters
    private float offset;
    private float speed;
    private float stoppingDist;

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
    public Animation healthAnimation;

    // Booleans
    public bool chasing = false;
    public bool canBeDamaged = true;
    private bool distanceAdjusted = false;
    private bool distanceAdjusted2 = false;
    private bool attacking = false;

    // Others
    public Animator ActiveAnimator;
    KnockBack knockBackScript;

    private enum ENEMYTYPE
    {
        goblin = 1,
        spider,
        wolf,
        ogre
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
    public AudioClip stepSound;
    public AudioClip stepSound2;
    public AudioClip ogreSpawnSound;
    public AudioClip ogreAttackSound;

    [SerializeField] ENEMYTYPE currentEnemyType;

    void Start()
    {
        noiseTimerMax = 2.5f;
        chaseTimerMax = 10.0f;

        agent = GetComponent<NavMeshAgent>();
        SetEnemyParameters();

        playerPosition = PlayerController.global.transform;
        PlayerController.global.enemyList.Add(transform); // Adding each object transform with this script attached to the enemy list
        if (agent.isOnNavMesh)
        {
            Indicator.global.AddIndicator(transform, Color.red, currentEnemyType.ToString());
        }

        knockBackScript = GetComponent<KnockBack>();

        if (currentEnemyType == ENEMYTYPE.ogre)
        {
            GameManager.global.SoundManager.PlaySound(ogreSpawnSound, 1.0f);
        }
    }

    void Update()
    {
        CheckHouse();
        Checks();
        MakeNoise();
        Process();
        ResetAttack();
    }

    void CheckHouse()
    {
        float closest = 9999;

        for (int i = 0; i < LevelManager.global.BuildingList.Count; i++)
        {
            if (LevelManager.global.BuildingList[i].GetComponent<Building>().resourceObject == Building.BuildingType.HouseNode)
            {
                float distance = Vector3.Distance(transform.position, LevelManager.global.BuildingList[i].position);

                if (distance < closest)
                {
                    closest = distance;
                    house = LevelManager.global.BuildingList[i].gameObject;
                }
            }
        }
    }

    void Process()
    {
        // Default value
        float shortestDistance = 9999;

        if (currentEnemyType == ENEMYTYPE.ogre)
        {
            if (bestTarget == null)
            {
                bestTarget = house.transform;
            }
        }
        else if (currentEnemyType == ENEMYTYPE.wolf)
        {
            if (bestTarget == null)
            {
                if (Vector3.Distance(transform.position, PlayerController.global.transform.position) <= 20.0f)
                {
                    bestTarget = playerPosition;
                }
            }
        }
        else if (currentEnemyType == ENEMYTYPE.spider || currentEnemyType == ENEMYTYPE.goblin)
        {
            if (PlayerController.global.playerEnergy <= 0 || PlayerController.global.sleeping == true || Boar.global.mounted == true)
            {
                chasing = false;
            }
            else if (currentEnemyType == ENEMYTYPE.spider)
            {
                chasing = true;
            }

            if (chasing)
            {
                bestTarget = playerPosition; // Player set as target           

                if (currentEnemyType == ENEMYTYPE.goblin)
                {
                    // Distance between enemy and player
                    float distance = Vector3.Distance(PlayerController.global.transform.position, transform.position);

                    chaseTimer += Time.deltaTime; // Enemy stops chasing the player after 10s or when the player gets too far

                    if (chaseTimer >= chaseTimerMax || distance >= 10.0f)
                    {
                        bestTarget = null;
                        chasing = false;
                        chaseTimer = 0;
                    }
                }
            }
            else
            {
                if (!bestTarget || bestTarget == playerPosition || bestTarget == Boar.global.transform) // If the enemy does not have a current target
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
        }

        // Once a target is set, adjust stopping distance, check distance, attack when reaching target
        if (bestTarget)
        {
            if (bestTarget == playerPosition)
            {
                if (Boar.global.mounted == true && currentEnemyType == ENEMYTYPE.wolf)
                {
                    bestTarget = Boar.global.transform;
                }
                agent.stoppingDistance = stoppingDist;
                distanceAdjusted = false;
                distanceAdjusted2 = false;
            }
            if (bestTarget == Boar.global.transform)
            {
                agent.stoppingDistance = stoppingDist + 1.0f;
                if (Boar.global.mounted == false)
                {
                    bestTarget = playerPosition;
                }
            }
            if (bestTarget != playerPosition && bestTarget != Boar.global.transform && bestTarget != house.transform)
            {
                if (distanceAdjusted == false)
                {
                    agent.stoppingDistance = stoppingDist + 2.5f;
                    distanceAdjusted = true;
                }
                distanceAdjusted2 = false;
            }

            if (bestTarget != house.transform)
            {
                if (Vector3.Distance(transform.position, bestTarget.position) <= agent.stoppingDistance + offset) // Checks if enemy reached target
                {
                    FaceTarget(); // Makes the enemy face the player
                    if (!attacking)
                    {
                        Attack();
                    }
                }
            }

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(bestTarget.position); // Makes the enemy move
            }
            ActiveAnimator.SetBool("Moving", Vector3.Distance(transform.position, bestTarget.position) > agent.stoppingDistance + offset);
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
            Destroy(gameObject);
            return;
        }
    }

    void Attack()
    {
        attacking = true;
        attackTimer = 0;
        ActiveAnimator.ResetTrigger("Attack");
        ActiveAnimator.SetTrigger("Attack");
    }

    private void FaceTarget() // Making sure the enemy always faces what it is attacking
    {
        Vector3 direction = (bestTarget.position - transform.position).normalized; // Gets a direction using a normalized vector
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Obtaining a rotation angle
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); // Smoothly rotating towards target
    }

    public void Damaged(float amount)
    {
        health -= amount;
        if (HealthAnimationState != null && HealthAnimationState.enabled)
        {
            HealthAnimationState.time = 1;
            GameManager.PlayAnimation(healthAnimation, "Health Hit");
        }
        else
        {
            HealthAnimationState = GameManager.PlayAnimation(healthAnimation, "Health Appear");
        }
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
        if (health <= 0)
        {
            if (currentEnemyType != ENEMYTYPE.ogre)
            {
                PickSound(deathSound, deathSound2, 1.0f);
            }
            else
            {
                GameManager.global.SoundManager.PlaySound(deathSound, 1.0f);
            }
            
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
            PickSound(noiseSound, noiseSound2, 1.0f);

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
                chaseTimer = 0;
                if (currentEnemyType != ENEMYTYPE.ogre)
                {
                    knockBackScript.knock = true;
                }
                canBeDamaged = false;
                ScreenShake.global.shake = true;
                chasing = true;
                Damaged(PlayerController.global.attackDamage);
                PickSound(hitSound, hitSound2, 1.0f);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (bestTarget == house)
        {
            if (other.gameObject == house)
            {
                if (!attacking)
                {
                    Attack();
                }
                agent.stoppingDistance = Vector3.Distance(transform.position, house.transform.position);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && canBeDamaged && PlayerController.global.attackTimer > 0.2f && PlayerController.global.attackTimer < 0.7f)
            {
                StopAllCoroutines();
                chaseTimer = 0;
                if (currentEnemyType != ENEMYTYPE.ogre)
                {
                    knockBackScript.knock = true;
                }
                if (currentEnemyType == ENEMYTYPE.goblin)
                {
                    chasing = true;
                }
                canBeDamaged = false;
                ScreenShake.global.shake = true;
                Damaged(PlayerController.global.attackDamage);
                PickSound(hitSound, hitSound2, 1.0f);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
            }
        }
        if (bestTarget == house)
        {
            if (other.gameObject == house)
            {
                if (!attacking)
                {
                    Attack();
                }
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
            agent.stoppingDistance = 2.0f;
            offset = 0.25f;
        }
        else if (currentEnemyType == ENEMYTYPE.spider)
        {
            agent.speed = 4.0f;
            agent.acceleration = 50.0f;
            agent.angularSpeed = 200.0f;
            maxHealth = 2.0f;
            attackTimerMax = 2.5f;
            agent.stoppingDistance = 2.5f;
            offset = 0.3f;
        }
        else if (currentEnemyType == ENEMYTYPE.wolf)
        {
            agent.speed = 4.75f;
            agent.acceleration = 40.0f;
            agent.angularSpeed = 100.0f;
            maxHealth = 4.0f;
            attackTimerMax = 2.5f;
            agent.stoppingDistance = 6.5f;
            offset = 0.2f;
        }
        else if (currentEnemyType == ENEMYTYPE.ogre)
        {
            agent.speed = 2.0f;
            agent.acceleration = 20.0f;
            agent.angularSpeed = 80.0f;
            maxHealth = 10.0f;
            attackTimerMax = 6.0f;
            agent.stoppingDistance = 4.5f;
            offset = 0.2f;
        }
        health = maxHealth;
        speed = agent.speed;
        stoppingDist = agent.stoppingDistance;
    }

    private void PickSound(AudioClip name1, AudioClip name2, float volume)
    {
        GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? name1 : name2, volume, true, 0, false, transform);
    }

    private void ResetAttack()
    {
        if (attacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackTimerMax)
            {
                attacking = false;
            }
        }
    }

    public void AnimationAttack()
    {
        if (currentEnemyType != ENEMYTYPE.ogre)
        {
            PickSound(attackSound, attackSound2, 1.0f);
        }      

        if (bestTarget == playerPosition || bestTarget == Boar.global.transform)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerHitSound, 0.2f, true, 0, false, playerPosition);
            playerPosition.GetComponent<PlayerController>().playerEnergy -= 5;
        }
        else
        {
            if (currentEnemyType == ENEMYTYPE.ogre)
            {
                GameManager.global.SoundManager.PlaySound(ogreAttackSound, 1.0f);
            }
            Building building = bestTarget.GetComponent<Building>();

            if (building && building.resourceObject == Building.BuildingType.HouseNode)
            {
                building = building.transform.parent.GetComponent<Building>();
            }

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

    public void FirstStep()
    {
        if (currentEnemyType == ENEMYTYPE.spider)
        {
            AudioClip step = Random.Range(0, 2) == 0 ? stepSound : stepSound2;
            GameManager.global.SoundManager.PlaySound(step, 0.8f, true, 0, false, transform);
        }
        else
        {
            GameManager.global.SoundManager.PlaySound(stepSound, 0.4f, true, 0, false, transform);
        }
    }

    public void SecondStep()
    {
        GameManager.global.SoundManager.PlaySound(stepSound2, 0.4f, true, 0, false, transform);
    }

    public void OgreAttackSound()
    {
        AudioClip attack = Random.Range(0, 2) == 0 ? attackSound : attackSound2;
        GameManager.global.SoundManager.PlaySound(attack, 1.0f);
    }

    private void OgreStepOne()
    {
        GameManager.global.SoundManager.PlaySound(stepSound, 0.8f);
    }

    private void OgreStepTwo()
    {
        GameManager.global.SoundManager.PlaySound(stepSound2, 0.8f);
    }
}