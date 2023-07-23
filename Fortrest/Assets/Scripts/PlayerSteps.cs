using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    void Attack()
    {
        PlayerController.global.AttackEffects();
    }

    void Gather()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode)
        {
            PlayerController.global.GatheringEffects();
        }        
    }

    void DamageEnemy()
    {       
        PlayerController.global.damageEnemy = true;
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
