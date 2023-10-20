using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Agent components
    private NavMeshAgent agent; // Nav mesh agent component

    // Transforms
    private Transform bestTarget; // Target that the enemy will go towards
    private Transform playerPosition;
    public GameObject house;

    // Parameters
    private float offset;
    private float speed;
    private float stoppingDist;
    private float enemyDamage;

    // Timers
    private float attackTimer;
    private float attackTimerMax;
    private float noiseTimer;
    private float noiseTimerMax;
    private float chaseTimer;
    private float chaseTimerMax;

    // Health
    [HideInInspector]
    public float health;
    private float maxHealth;
    public Image healthBarImage;
    private float HealthAppearTimer = -1;
    public Animation healthAnimation;
    private bool dead;

    // Booleans
    public bool chasing = false;
    public bool canBeDamaged = true;
    private bool attacking = false;
    public bool canBeDamagedByBoar = true;
    private bool knockbackIncreased;
    private bool waveEnemy;
    // Used for lycan boss
    public bool isMob = false;
    public PhaseOneLycan bossScriptOne;
    public PhaseTwoLycan bossScriptTwo;


    // Others
    public Animator ActiveAnimator;
    [HideInInspector] public KnockBack knockBackScript;

    public enum ENEMYTYPE
    {
        goblin = 1,
        spider,
        wolf,
        ogre
    };
    public ENEMYTYPE currentEnemyType;

    public enum ENEMYSTATE
    {
        IDLE = 1,
        PATROL,
        CHASING
    };
    private ENEMYSTATE currentEnemyState;

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

    // Stagger
    public Animation flashingAnimation;
    public bool flashing;

    // Patrol
    private Vector3 startPosition;
    private Vector3 destination;
    private bool newDestination = true;
    private bool isInPosition = true;
    private float previousDistance;
    private float patrolCooldown;
    private float patrolThreshold;
    bool slowed;

    private void Awake()
    {
        if (GameManager.ReturnInMainMenu())
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Start()
    {
        noiseTimerMax = 2.5f;
        chaseTimerMax = 10.0f;
        agent = GetComponent<NavMeshAgent>();
        knockBackScript = GetComponent<KnockBack>();
        playerPosition = PlayerController.global.transform;
        startPosition = transform.position;
        SetEnemyParameters();

        LevelManager.global.enemyList.Add(this); // Adding each object transform with this script attached to the enemy list
        if (agent.isOnNavMesh)
        {
            if (currentEnemyType != ENEMYTYPE.wolf)
            {
                Indicator.global.AddIndicator(transform, Color.red, LevelManager.global.enemiesCount < 10 ? currentEnemyType.ToString() : "");
            }
        }
        if (currentEnemyType == ENEMYTYPE.ogre)
        {
            GameManager.global.SoundManager.PlaySound(ogreSpawnSound, 1.0f);
        }

        if (currentEnemyType == ENEMYTYPE.goblin)
        {
            waveEnemy = true;
        }
    }

    void Update()
    {
        Checks();
        CheckHouse();

        if (!LevelManager.global.enabled) //if level manager isnt enabled, its because the house was destroyed and its game over
        {
            bestTarget = null;
            agent.SetDestination(transform.position);
        }
        else
        {
            MakeNoise();
            Process();
            ResetAttack();
        }

        if (PlayerController.global.upgradedMelee && !knockbackIncreased)
        {
            knockBackScript.strength *= 1.25f;
            knockbackIncreased = true;
        }
        if (HealthAppearTimer != -1)
        {
            HealthAppearTimer += Time.deltaTime;

            if (HealthAppearTimer > 5)
            {
                HealthAppearTimer = -1;
                GameManager.PlayAnimation(healthAnimation, "Health Appear", false);
            }
        }
    }

    void CheckHouse()
    {
        float closest = 9999;

        LevelManager.ProcessBuildingList((building) =>
         {
             if (building.GetComponent<Building>().buildingObject == Building.BuildingType.HouseNode)
             {
                 float distance = Vector3.Distance(transform.position, building.position);

                 if (distance < closest)
                 {
                     closest = distance;
                     house = building.gameObject;
                 }
             }
         });
    }

    void Process()
    {
        // Default value
        float shortestDistance = 9999;

        // Ogre goes for the house
        if (currentEnemyType == ENEMYTYPE.ogre)
        {
            if (bestTarget == null)
            {
                bestTarget = house.transform;
            }
        }

        // Wolf goes for the player if they get too close, and stops chasing if they get too far or go in the house
        if (currentEnemyType == ENEMYTYPE.wolf)
        {
            if (bestTarget == null)
            {
                Patrol();
                if (Vector3.Distance(transform.position, PlayerController.global.transform.position) <= 17.5f)
                {
                    bestTarget = playerPosition;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, PlayerController.global.transform.position) >= 35.0f || PlayerModeHandler.global.inTheFortress)
                {
                    bestTarget = null;
                    isInPosition = false;
                }
            }
        }

        // Spiders roam their biome and target the player when they get too close. Wave spiders target the player if they are reachable.
        if (currentEnemyType == ENEMYTYPE.spider)
        {
            if (waveEnemy) // Spiders that spawn during waves
            {
                if (Boar.global.mounted == true || PlayerModeHandler.global.inTheFortress || Vector3.Distance(transform.position, PlayerController.global.transform.position) > 30f) // If the player is unreachable
                {
                    bestTarget = null;
                }
                else
                {
                    bestTarget = playerPosition;
                }
            }
            else // Spiders that roam their biome
            {
                if (bestTarget == null)
                {
                    Patrol();
                    if (Vector3.Distance(transform.position, PlayerController.global.transform.position) <= 15f)
                    {
                        bestTarget = playerPosition;
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, PlayerController.global.transform.position) >= 50.0f)
                    {
                        bestTarget = null;
                        isInPosition = false;
                    }
                }
            }
        }
        // Goblin goes for the player if it gets attacked by them
        if (currentEnemyType == ENEMYTYPE.goblin)
        {
            if (chasing)
            {
                bestTarget = playerPosition;

                // Goblin stops chasing the player after 10s or when the player gets too far or when the player is mounted or when the player is in the fortress
                float distance = Vector3.Distance(PlayerController.global.transform.position, transform.position);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= chaseTimerMax || distance >= 10.0f || Boar.global.mounted == true || PlayerModeHandler.global.inTheFortress)
                {
                    bestTarget = null;
                    chasing = false;
                    chaseTimer = 0;
                }
            }
        }

        if (bestTarget == null && waveEnemy) // If the enemy does not have a current target
        {
            LevelManager.ProcessBuildingList((building) =>
            {
                if (building.transform.localScale == Vector3.one) // To avoid targeting turrets spawning
                {
                    float compare = Vector3.Distance(transform.position, building.position); // Distance from enemy to each target

                    if (compare < shortestDistance) // Only true if a new shorter distance is found
                    {
                        shortestDistance = compare; // New shortest distance is assigned
                        bestTarget = building; // Enemy's target is now the closest item in the list
                    }
                }
            });
        }

        // Once a target is set, adjust stopping distance, check distance, attack when reaching target
        if (bestTarget)
        {
            // If the player mounts while being targeted
            if (bestTarget == playerPosition)
            {
                agent.stoppingDistance = stoppingDist;
                // Boar becomes the target
                if (Boar.global && Boar.global.mounted == true)
                {
                    bestTarget = Boar.global.transform;
                }
            }

            // Adjust stopping distance to mount, and reverse to player if they dismount
            if (Boar.global && bestTarget == Boar.global.transform)
            {
                agent.stoppingDistance = stoppingDist + 1.0f;
                if (Boar.global.mounted == false)
                {
                    bestTarget = playerPosition;
                }
            }

            // Stopping distance for turret
            if (bestTarget.gameObject.GetComponent<Building>())
            {
                if (bestTarget.gameObject.GetComponent<Building>().buildingObject == Building.BuildingType.Ballista || bestTarget.gameObject.GetComponent<Building>().buildingObject == Building.BuildingType.Slow)
                {
                    agent.stoppingDistance = stoppingDist + 2.5f;
                }
                else if (bestTarget.gameObject.GetComponent<Building>().buildingObject == Building.BuildingType.Scatter || bestTarget.gameObject.GetComponent<Building>().buildingObject == Building.BuildingType.Cannon)
                {
                    agent.stoppingDistance = stoppingDist + 2.0f;
                }
            }

            // Enemy Attacks (works differently for the house
            if (!house || bestTarget != house.transform)
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
                if (!dead)
                {
                    agent.SetDestination(bestTarget.position); // Makes the enemy move
                }
                else
                {
                    agent.SetDestination(transform.position); // Avoids them moving while dead
                }
            }
        }

        if (agent.velocity != Vector3.zero)
        {
            ActiveAnimator.SetBool("Moving", true);
        }
        else
        {
            ActiveAnimator.SetBool("Moving", false);
        }
    }

    void Patrol()
    {
        patrolCooldown += Time.deltaTime;

        if (patrolCooldown > patrolThreshold) //prevents wolves jittering and gives them time to move properly
        {
            patrolCooldown = 0;
            patrolThreshold = Random.Range(1, 3);

            float distance = Vector3.Distance(transform.position, destination);

            if (previousDistance != 0 && previousDistance == distance) //prevents wolf getting stuck and standing still
            {
                newDestination = true;
            }
            previousDistance = distance;

            if (!isInPosition)
            {
                agent.SetDestination(startPosition);
                if (distance < 1.0f)
                {
                    isInPosition = true;
                }
            }
            else
            {
                if (newDestination)
                {
                    float x = Random.Range(-20f, 20f);
                    float z = Random.Range(-20f, 20f);
                    destination = transform.position + new Vector3(x, 0f, z);
                    newDestination = false;
                }
                agent.SetDestination(destination);

                if (Vector3.Distance(transform.position, startPosition) > 50.0f || Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), destination) < 1.0f)
                {
                    newDestination = true;
                }
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
            gameObject.SetActive(false);
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

        if (health <= 0)
        {
            healthAnimation.gameObject.SetActive(false);
            dead = true;
            if (currentEnemyType != ENEMYTYPE.ogre && currentEnemyType != ENEMYTYPE.goblin) // remove once we got anims
            {
                agent.SetDestination(transform.position);
                ActiveAnimator.SetTrigger("Death");
            }
            else
            {
                Death();
            }
            StopAllCoroutines();
            if (currentEnemyType != ENEMYTYPE.ogre)
            {
                PickSound(deathSound, deathSound2, 1.0f);
            }
            else
            {
                GameManager.global.SoundManager.PlaySound(deathSound, 1.0f);
            }

            Time.timeScale = 1;
        }
        else
        {
            if (HealthAppearTimer == -1)
            {
                GameManager.PlayAnimation(healthAnimation, "Health Appear");
            }

            HealthAppearTimer = 0;

            GameManager.PlayAnimation(healthAnimation, "Health Hit");
            healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);
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
        if (!slowed)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.SlowShootSound);
            slowed = true;
            agent.speed *= _slowPercent;
        }
    }

    public void RemoveSlow()
    {
        slowed = false;
        agent.speed = speed;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && canBeDamaged && PlayerController.global.damageEnemy)
            {
                StopAllCoroutines();
                if (currentEnemyType == ENEMYTYPE.goblin)
                {
                    chaseTimer = 0;
                    chasing = true;
                }
                canBeDamaged = false;
                PickSound(hitSound, hitSound2, 1.0f);
                if (currentEnemyType != ENEMYTYPE.ogre && currentEnemyType != ENEMYTYPE.goblin && !flashing) // remove goblin once we have the anim
                {
                    ActiveAnimator.ResetTrigger("Hit1");
                    ActiveAnimator.ResetTrigger("Hit2");
                    ActiveAnimator.ResetTrigger("Hit3");
                    int random = Random.Range(1, 4);
                    if (random == 1)
                    {
                        ActiveAnimator.SetTrigger("Hit1");
                    }
                    else if (random == 2)
                    {
                        ActiveAnimator.SetTrigger("Hit2");
                    }
                    else
                    {
                        ActiveAnimator.SetTrigger("Hit3");
                    }
                    GameManager.PlayAnimation(flashingAnimation, "Flashing");
                    flashing = true;
                }
                knockBackScript.knock = true;
                ScreenShake.global.shake = true;
                Damaged(PlayerController.global.attackDamage);
                PlayerController.global.StartCoroutine(PlayerController.global.FreezeTime());
                if (PlayerController.global.upgradedMelee && health > 0)
                {
                    StartCoroutine(SlowedDown());
                }
            }
        }
        if (house && bestTarget == house.transform)
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
        if (other.gameObject.tag == "Arrow")
        {
            if (!other.GetComponent<ArrowTrigger>().singleHit)
            {
                other.GetComponent<ArrowTrigger>().singleHit = true;
                if (currentEnemyType == ENEMYTYPE.goblin)
                {
                    chaseTimer = 0;
                    chasing = true;
                }
                else if (currentEnemyType == ENEMYTYPE.wolf)
                {
                    bestTarget = playerPosition;
                }
                Damaged(PlayerController.global.bowDamage);
                if (currentEnemyType == ENEMYTYPE.goblin)
                {
                    PickSound(hitSound, hitSound2, 0.6f);
                }
                else
                {
                    PickSound(hitSound, hitSound2, 1.0f);
                }
                if (!PlayerController.global.upgradedBow || other.GetComponent<ArrowTrigger>().hitSecondEnemy)
                {
                    Destroy(other.gameObject.transform.parent.gameObject);
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
            enemyDamage = 3.0f;
            knockBackScript.strength = 50.0f;
        }
        else if (currentEnemyType == ENEMYTYPE.spider)
        {
            agent.speed = 4.0f;
            agent.acceleration = 50.0f;
            agent.angularSpeed = 200.0f;
            maxHealth = 4.0f;
            attackTimerMax = 2.0f;
            agent.stoppingDistance = 2.5f;
            offset = 0.3f;
            enemyDamage = 4.0f;
            knockBackScript.strength = 45.0f;
        }
        else if (currentEnemyType == ENEMYTYPE.wolf)
        {
            agent.speed = 8.5f;
            agent.acceleration = 40.0f;
            agent.angularSpeed = 130.0f;
            maxHealth = 6.0f;
            attackTimerMax = 2.75f;
            agent.stoppingDistance = 6.5f;
            offset = 0.2f;
            enemyDamage = 8.0f;
            knockBackScript.strength = 20.0f;
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
            enemyDamage = 10.0f;
            knockBackScript.strength = 0.0f;
        }
        health = maxHealth;
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
            if (currentEnemyType == ENEMYTYPE.goblin)
            {
                PickSound(attackSound, attackSound2, 0.6f);
            }
            else
            {
                PickSound(attackSound, attackSound2, 1.0f);
            }
        }

        if (bestTarget == playerPosition || (Boar.global && bestTarget == Boar.global.transform))
        {
            int randomInt = Random.Range(0, 3);
            AudioClip temp = null;
            switch (randomInt)
            {
                case 0:
                    temp = GameManager.global.PlayerHit1Sound;
                    break;
                case 1:
                    temp = GameManager.global.PlayerHit2Sound;
                    break;
                case 2:
                    temp = GameManager.global.PlayerHit3Sound;
                    break;
                default:
                    break;
            }
            GameManager.global.SoundManager.PlaySound(temp, 0.9f);
            if (PlayerController.global.playerCanBeDamaged)
            {
                PlayerController.global.TakeDamage(enemyDamage, false);
            }
        }
        else if (bestTarget)
        {
            if (currentEnemyType == ENEMYTYPE.ogre)
            {
                GameManager.global.SoundManager.PlaySound(ogreAttackSound, 1.0f);
            }

            Building building = bestTarget.GetComponent<Building>();
            if (building)
            {
                if (building.buildingObject == Building.BuildingType.HouseNode)
                {
                    building = building.transform.parent.GetComponent<Building>();
                }
                building.TakeDamage(1f);
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

    private IEnumerator SlowedDown()
    {
        agent.speed = speed / 1.25f;
        yield return new WaitForSeconds(2.5f);
        agent.speed = speed;
    }

    public void Death()
    {
        agent.enabled = false;
        if (currentEnemyType != ENEMYTYPE.wolf)
        {
            LevelManager.global.enemyList.Remove(this);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false); //wolves spawn in the scene on start so they need to stay in memory
        }

        if (isMob)
        {
            if (bossScriptOne)
            {
                bossScriptOne.EnemyList.Remove(gameObject);
            }
            else if (bossScriptTwo)
            {
                bossScriptTwo.EnemyList.Remove(gameObject);
            }
        }
    }

    public IEnumerator BoarKnockEffects()
    {
        float temp = agent.angularSpeed;
        agent.angularSpeed = 0;
        ActiveAnimator.StartPlayback();
        yield return new WaitForSeconds(0.75f);
        agent.angularSpeed = temp;
        ActiveAnimator.StopPlayback();
    }
}