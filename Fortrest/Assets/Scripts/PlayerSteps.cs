using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    void Attack()
    {
        if (!PlayerController.global.evading)
        {
            PlayerController.global.AttackEffects();
        }       
    }

    void Gather()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode && !PlayerController.global.evading)
        {
            PlayerController.global.GatheringEffects();
        }        
    }

    void DamageEnemy()
    {
        if (!PlayerController.global.evading)
        {
            PlayerController.global.damageEnemy = true;
        }
    }

    void Evading()
    {
        PlayerController.global.evading = false;
    }

    void StepOne()
    {
        PlayerController.global.FirstStep();
    }

    void StepTwo()
    {
        PlayerController.global.SecondStep();
    }
}
