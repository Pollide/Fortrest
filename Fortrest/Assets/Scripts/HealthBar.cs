using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image playerHealthBarImage;

    public void SetMaxHealth(float health)
    {
        if (playerHealthBarImage)
            playerHealthBarImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, health / PlayerController.global.maxHealth);
    }

    public void SetHealth(float health)
    {
        if (playerHealthBarImage)
            playerHealthBarImage.fillAmount = Mathf.Lerp(0.0f, 0.5f, health / PlayerController.global.maxHealth);
    }
}
