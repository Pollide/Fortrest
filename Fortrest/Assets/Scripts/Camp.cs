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
        wolfCamp
        
    };

    public CAMPTYPE campType;
    public bool canBeDamaged = true;
    public HealthBar healthBar;
    private float HealthAppearTimer = -1;
    public Animation healthAnimation;

    void Start()
    {
        LevelManager.global.campList.Add(this);
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
