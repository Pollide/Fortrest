using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.Text.RegularExpressions;
using System;

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
    private float healthAppearTimer = -1;
    public int enemySpawn = 2;
    public Animation healthAnimation;

    public int GetIndex(CAMPTYPE campType)
    {
        CAMPTYPE[] values = (CAMPTYPE[])Enum.GetValues(typeof(CAMPTYPE));
        return Array.IndexOf(values, campType);
    }

    public CAMPTYPE GetCampType(int index)
    {
        CAMPTYPE[] values = (CAMPTYPE[])Enum.GetValues(typeof(CAMPTYPE));
        if (index >= 0 && index < values.Length)
        {
            return values[index];
        }
        // Handle invalid index
        return CAMPTYPE.goblinCamp; // or throw an exception
    }

    void Start()
    {
        LevelManager.global.campList.Add(this);
        Indicator.global.AddIndicator(transform, Color.red, Regex.Replace(campType.ToString(), "(\\B[A-Z0-9])", " $1"), false);

        if (LevelManager.global.goblinSpawnable)
        {
            switch (campType)
            {
                case CAMPTYPE.goblinCamp:
                    if (LevelManager.global.spawnEntries[0] != null)
                    {
                        LevelManager.global.spawnEntries[0].spawnPercentage += CampSpawner.global.goblinCampPercent;
                    }

                    campPrefabs[0].SetActive(true);

                    SpawnEnemyOnCamp(enemySpawn, LevelManager.global.goblin.objectToSpawn);

                    break;
                case CAMPTYPE.snakesCamp:
                    if (LevelManager.global.spawnEntries[1] != null)
                    {
                        LevelManager.global.spawnEntries[1].spawnPercentage += CampSpawner.global.snakegoblinCampPercent;
                    }
                    campPrefabs[1].SetActive(true);

                    SpawnEnemyOnCamp(enemySpawn, LevelManager.global.snake.objectToSpawn);

                    break;
                case CAMPTYPE.spiderCamp:
                    if (LevelManager.global.spawnEntries[3] != null)
                    {
                        LevelManager.global.spawnEntries[3].spawnPercentage += CampSpawner.global.spidergoblinCampPercent;
                    }
                    campPrefabs[3].SetActive(true);

                    SpawnEnemyOnCamp(enemySpawn, LevelManager.global.spider.objectToSpawn);

                    break;
                case CAMPTYPE.wolfCamp:
                    if (LevelManager.global.spawnEntries[2] != null)
                    {
                        LevelManager.global.spawnEntries[2].spawnPercentage += CampSpawner.global.wolfgoblinCampPercents;
                    }
                    campPrefabs[2].SetActive(true);

                    SpawnEnemyOnCamp(enemySpawn, LevelManager.global.wolf.objectToSpawn);

                    break;
                case CAMPTYPE.lavaCamp:
                    if (LevelManager.global.spawnEntries[4] != null)
                    {
                        LevelManager.global.spawnEntries[4].spawnPercentage += CampSpawner.global.lavagoblinCampPercent;
                    }
                    campPrefabs[4].SetActive(true);

                    SpawnEnemyOnCamp(enemySpawn, LevelManager.global.lava.objectToSpawn);

                    break;
                default:
                    break;
            }
        }

        //Indicator.global.AddIndicator(transform)
        health = maxHealth;
    }

    private void SpawnEnemyOnCamp(int amount, GameObject gameObject)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 enemySpawnPos = new(transform.position.x + UnityEngine.Random.Range(-2f, 2f), transform.position.y, transform.position.z + UnityEngine.Random.Range(-2f, 2f));

            GameObject enemy = Instantiate(gameObject, enemySpawnPos, Quaternion.identity);

            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            enemyController.addIndicator = false;
        }
    }

    private void Update()
    {
        if (healthAppearTimer != -1)
        {
            healthAppearTimer += Time.deltaTime;

            if (healthAppearTimer > 5)
            {
                healthAppearTimer = -1;
                GameManager.PlayAnimation(healthAnimation, "Health Appear", false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (healthAppearTimer == -1)
        {
            GameManager.PlayAnimation(healthAnimation, "Health Appear");
        }
        healthAppearTimer = 0;

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
                        LevelManager.global.spawnEntries[3].spawnPercentage -= CampSpawner.global.spidergoblinCampPercent;
                        break;
                    case CAMPTYPE.wolfCamp:
                        LevelManager.global.spawnEntries[2].spawnPercentage -= CampSpawner.global.wolfgoblinCampPercents;
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
                GameObject tempVFX = Instantiate(PlayerController.global.swordVFX.gameObject, ((PlayerController.global.transform.position + transform.position) / 2) + PlayerController.global.transform.forward, Quaternion.identity);
                if (tempVFX.transform.position.y < 0)
                {
                    tempVFX.transform.position = new Vector3(tempVFX.transform.position.x, PlayerController.global.transform.position.y, tempVFX.transform.position.z);
                }
                tempVFX.GetComponent<VisualEffect>().Play();
                Destroy(tempVFX, 1.0f);
                canBeDamaged = false;
                TakeDamage(PlayerController.global.attackDamage);
            }
        }
    }
}
