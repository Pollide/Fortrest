using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    void Attack()
    {
        if (!PlayerController.global.evading && !PlayerController.global.cancelEffects)
        {
            PlayerController.global.lunge = true;
            PlayerController.global.AttackEffects();        
        }       
    }

    void Gather()
    {
        if (PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode && !PlayerController.global.evading && !PlayerController.global.cancelEffects)
        {
            PlayerController.global.GatheringEffects();
        }        
    }

    void DamageEnemy()
    {
        if (!PlayerController.global.evading && !PlayerController.global.cancelEffects)
        {
            PlayerController.global.damageEnemy = true;
        }
    }

    void Evading()
    {
        PlayerController.global.evading = false;
    }

    void LungeEnd()
    {
        PlayerController.global.lunge = false;
    }

    void StepOne()
    {
        if (!PlayerController.global.running)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound, 0.05f);
        }       
    }

    void StepTwo()
    {
        if (!PlayerController.global.running)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound2, 0.05f);
        }       
    }

    void RunningStepOne()
    {
        if (PlayerController.global.running)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound, 0.05f);
        }        
    }

    void RunningStepTwo()
    {
        if (PlayerController.global.running)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound2, 0.05f);
        }        
    }

    void Death()
    {
        PlayerController.global.playerDead = true;
    }
}