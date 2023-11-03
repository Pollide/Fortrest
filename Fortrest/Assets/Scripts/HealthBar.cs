using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;

    public void SetHealth(float health, float maxHealth)
    {
        if (healthBarImage)
        {
            healthBarImage.fillAmount = health / maxHealth;
        }
    }
}