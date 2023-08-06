using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image playerHealthBarImage;

    public void SetMaxHealth(float health, bool house)
    {
        if (house)
        {
            slider.maxValue = health;
            slider.value = health;
        }
        else
        {
            if (playerHealthBarImage)
            {
                playerHealthBarImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, health / PlayerController.global.maxHealth);
            }                
        }      
    }

    public void SetHealth(float health, bool house)
    {
        if (house)
        {
            slider.value = health;
        }
        else
        {
            if (playerHealthBarImage)
            {
                playerHealthBarImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, health / PlayerController.global.maxHealth);
            }                   
        }        
    }
}