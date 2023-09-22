using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camp : MonoBehaviour
{
    private float health;
    private float maxHealth;
    public enum CAMPTYPE
    {
        goblinCamp = 1,
        spiderCamp
    };
    public CAMPTYPE campType;
    public bool canBeDamaged = true;
    public Image healthBarImage;
    private float HealthAppearTimer = -1;
    public Animation healthAnimation;

    void Start()
    {
        LevelManager.global.campList.Add(this);
        maxHealth = 15.0f;
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
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1f);

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
            PlayerController.global.cursorNearEnemy = true;

            if (PlayerController.global.attacking && canBeDamaged && PlayerController.global.damageEnemy)
            {
                canBeDamaged = false;
                TakeDamage(PlayerController.global.attackDamage);
            }
        }
    }
}
