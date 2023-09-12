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
    [HideInInspector]
    public float health;
    private float maxHealth;
    public Image healthBarImage;
    private float HealthAppearTimer = -1;
    public Animation healthAnimation;

    // Booleans
    public bool chasing = false;
    public bool canBeDamaged = true;
    private bool distanceAdjusted = false;
    private bool attacking = false;

    // Others
    public Animator ActiveAnimator;
    [HideInInspector] public KnockBack knockBackScript;

    private float enemyDamage;

    public enum ENEMYTYPE
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

    public ENEMYTYPE currentEnemyType;

    private bool knockbackIncreased;

    public Animation flashingAnimation;
    public bool flashing;
    private bool dead;
    private void Awake()
    {
        if (GameManager.ReturnInMainMenu())
        {
            gameObject.SetActive(false);
            // Destroy(agent);
            //  enabled = false;
            return;
        }
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        noiseTimerMax = 2.5f;
        chaseTimerMax = 10.0f;
        agent.angularSpeed *= 2;
        knockBackScript = GetComponent<KnockBack>();

        SetEnemyParameters();

        playerPosition = PlayerController.global.transform;
        LevelManager.global.enemyList.Add(this); // Adding each object transform with this script attached to the enemy list
        if (agent.isOnNavMesh)
        {
            if (currentEnemyType != ENEMYTYPE.wolf) //wolves wild
            {
                Indicator.global.AddIndicator(transform, Color.red, currentEnemyType.ToString());
            }
        }

        if (currentEnemyType == ENEMYTYPE.ogre)
        {
            GameManager.global.SoundManager.PlaySound(ogreSpawnSound, 1.0f);
        }
    }

    void Update()
    {
        CheckHouse();
        if (house.transform.parent.GetComponent<Building>().DestroyedBool)
        {
            bestTarget = null;
            agent.SetDestination(transform.position);
        }
        else
        {
            Checks();
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
             if (building.GetComponent<Building>().resourceObject == Building.BuildingType.HouseNode)
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
        // Wolf goes for the player if they get close and stops chasing if they get too far or in the house
        else if (currentEnemyType == ENEMYTYPE.wolf)
        {
            if (bestTarget == null)
            {
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
                }
            }
        }
        // Spider goes for the player if they are not mounted, goblin goes for the player if it gets attacked by them
        else if (currentEnemyType == ENEMYTYPE.spider || currentEnemyType == ENEMYTYPE.goblin)
        {
            if (Boar.global.mounted == true || PlayerModeHandler.global.inTheFortress)
            {
                bestTarget = null;
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

                    chaseTimer += Time.deltaTime; // Goblin stops chasing the player after 10s or when the player gets too far

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
                if (!bestTarget || bestTarget == playerPosition || (Boar.global && bestTarget == Boar.global.transform)) // If the enemy does not have a current target
                {
                    LevelManager.ProcessBuildingList((building) =>
                    {

                        float compare = Vector3.Distance(transform.position, building.position); // Distance from enemy to each target

                        if (compare < shortestDistance) // Only true if a new shorter distance is found
                        {
                            shortestDistance = compare; // New shortest distance is assigned
                            bestTarget = building; // Enemy's target is now the closest item in the list
                        }

                    });
                }
            }
        }

        // Once a target is set, adjust stopping distance, check distance, attack when reaching target
        if (bestTarget)
        {
            if (bestTarget == playerPosition)
            {
                if ((Boar.global && Boar.global.mounted == true) && currentEnemyType == ENEMYTYPE.wolf)
                {
                    bestTarget = Boar.global.transform;
                }
                agent.stoppingDistance = stoppingDist;
                distanceAdjusted = false;
            }
            if (Boar.global && bestTarget == Boar.global.transform)
            {
                agent.stoppingDistance = stoppingDist + 1.0f;
                if (Boar.global.mounted == false)
                {
                    bestTarget = playerPosition;
                }
            }
            if (bestTarget != playerPosition && (Boar.global && bestTarget != Boar.global.transform) && bestTarget != house.transform)
            {
                if (distanceAdjusted == false)
                {
                    agent.stoppingDistance = stoppingDist + 2.5f;
                    distanceAdjusted = true;
                }
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
                if (!dead)
                {
                    agent.SetDestination(bestTarget.position); // Makes the enemy move
                }
                else
                {
                    agent.SetDestination(transform.position); // Avoids them moving while dead
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
        else
        {
            if (currentEnemyType == ENEMYTYPE.wolf)
            {
                agent.SetDestination(transform.position);
                ActiveAnimator.SetBool("Moving", false);
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
        if (HealthAppearTimer == -1)
        {
            GameManager.PlayAnimation(healthAnimation, "Health Appear");
        }

        HealthAppearTimer = 0;

        GameManager.PlayAnimation(healthAnimation, "Health Hit");
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);

        if (health <= 0)
        {
            dead = true;
            agent.SetDestination(transform.position);
            healthAnimation.gameObject.SetActive(false);
            if (currentEnemyType != ENEMYTYPE.ogre && currentEnemyType != ENEMYTYPE.goblin) // remove once we got anims
            {
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            PlayerController.global.cursorNearEnemy = true;

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
            if (!other.GetComponent<Arrow>().singleHit || other.GetComponent<Arrow>().secondHit)
            {
                other.GetComponent<Arrow>().singleHit = true;
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
                PickSound(hitSound, hitSound2, 1.0f);
                if (!PlayerController.global.upgradedBow || other.GetComponent<Arrow>().secondHit)
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Arrow")
        {
            other.GetComponent<Arrow>().secondHit = true;
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
            PickSound(attackSound, attackSound2, 1.0f);
        }

        if (bestTarget == playerPosition || bestTarget == Boar.global.transform)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerHitSound, 0.2f, true, 0, false, playerPosition);
            if (PlayerController.global.playerCanBeDamaged)
            {
                if (currentEnemyType == ENEMYTYPE.goblin)
                {
                    PlayerController.global.TakeDamage(enemyDamage, false);
                }
                else
                {
                    PlayerController.global.TakeDamage(enemyDamage, true);
                }
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
                if (building.resourceObject == Building.BuildingType.HouseNode)
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

    private void Death()
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
    }

    public IEnumerator StopAnimation(float _time)
    {
        ActiveAnimator.StartPlayback();
        yield return new WaitForSeconds(_time);
        ActiveAnimator.StopPlayback();
    }
}