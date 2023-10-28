using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camp : MonoBehaviour
{
    private float health;
    public float maxHealth = 15f;
    
    public enum CAMPTYPE
    {
        goblinCamp = 1,
        snakesCamp,
        spiderCamp,
        wolfCamp,
        lavaCamp
    };

    public CAMPTYPE campType;
    public GameObject[] campPrefabs;
    public bool canBeDamaged = true;
    public HealthBar healthBar;
    private float HealthAppearTimer = -1;
    public Animation healthAnimation;

    void Start()
    {
        LevelManager.global.campList.Add(this);

        if (LevelManager.global.spawnEntries.Count > 0)
        {
            switch (campType)
            {
                case CAMPTYPE.goblinCamp:
                    LevelManager.global.spawnEntries[0].spawnPercentage += CampSpawner.global.goblinCampPercent;
                    campPrefabs[0].SetActive(true);
                    break;
                case CAMPTYPE.snakesCamp:
                    LevelManager.global.spawnEntries[1].spawnPercentage += CampSpawner.global.snakegoblinCampPercent;
                    campPrefabs[1].SetActive(true);
                    break;
                case CAMPTYPE.spiderCamp:
                    LevelManager.global.spawnEntries[2].spawnPercentage += CampSpawner.global.spidergoblinCampPercent;
                    campPrefabs[2].SetActive(true);
                    break;
                case CAMPTYPE.wolfCamp:
                    LevelManager.global.spawnEntries[3].spawnPercentage += CampSpawner.global.wolfgoblinCampPercents;
                    campPrefabs[3].SetActive(true);
                    break;
                case CAMPTYPE.lavaCamp:
                    LevelManager.global.spawnEntries[4].spawnPercentage += CampSpawner.global.lavagoblinCampPercent;
                    campPrefabs[4].SetActive(true);
                    break;
                default:
                    break;
            }
        }

        //Indicator.global.AddIndicator(transform)
        health = maxHealth;
    }

    private void Update()
    {
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

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (HealthAppearTimer == -1)
        {
            GameManager.PlayAnimation(healthAnimation, "Health Appear");
        }
        HealthAppearTimer = 0;

        GameManager.PlayAnimation(healthAnimation, "Health Hit");
        healthBar.SetHealth(health, maxHealth);

        if (health <= 0)
        {

            if (LevelManager.global.spawnEntries.Count > 0)
            {
                switch (campType)
                {
                    case CAMPTYPE.goblinCamp:
                        LevelManager.global.spawnEntries[0].spawnPercentage -= CampSpawner.global.goblinCampPercent;
                        break;
                    case CAMPTYPE.snakesCamp:
                        LevelManager.global.spawnEntries[1].spawnPercentage -= CampSpawner.global.snakegoblinCampPercent;
                        break;
                    case CAMPTYPE.spiderCamp:
                        LevelManager.global.spawnEntries[2].spawnPercentage -= CampSpawner.global.spidergoblinCampPercent;
                        break;
                    case CAMPTYPE.wolfCamp:
                        LevelManager.global.spawnEntries[3].spawnPercentage -= CampSpawner.global.wolfgoblinCampPercents;
                        break;
                    case CAMPTYPE.lavaCamp:
                        LevelManager.global.spawnEntries[4].spawnPercentage -= CampSpawner.global.lavagoblinCampPercent;
                        break;
                    default:
                        break;
                }
            }
            healthAnimation.gameObject.SetActive(false);
            LevelManager.global.campList.Remove(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.global.SwordGameObject)
        {
            if (PlayerController.global.attacking && canBeDamaged && PlayerController.global.damageEnemy)
            {
                canBeDamaged = false;
                TakeDamage(PlayerController.global.attackDamage);
            }
        }
    }
}
