using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    private float timer = 0f;
    private bool initialStep;

    private void Update()
    {
        if (PlayerController.global && PlayerController.global.playerisMoving)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            initialStep = false;
        }
    }

    void Attack()
    {
        if (!PlayerController.global.evading && !PlayerController.global.cancelEffects && !PlayerController.global.characterAnimator.GetBool("Swapping"))
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

    void SwingStart()
    {
        if (!PlayerController.global.evading && !PlayerController.global.cancelEffects)
        {
            PlayerController.global.damageEnemy = true;
            PlayerController.global.attackTimer = 0;
            CancelInvoke("SwingEnding");
            Invoke("SwingEnding", 0.5f);
        }
    }

    void SwingEnding()
    {
        PlayerController.global.damageEnemy = false;
    }

    void SwingEnd()
    {
        //not in use anymore
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
        if (!PlayerController.global.running && !PlayerController.global.aiming && (timer >= 0.3f || !initialStep))
        {
            initialStep = true;
            timer = 0f;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound, 0.05f);
        }
    }

    void StepTwo()
    {
        if (!PlayerController.global.running && !PlayerController.global.aiming && timer >= 0.3f)
        {
            timer = 0f;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStep2Sound, 0.05f);
        }
    }

    void RunningStepOne()
    {
        if (PlayerController.global.running && !PlayerController.global.aiming && (timer >= 0.2f || !initialStep))
        {
            initialStep = true;
            timer = 0f;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound, 0.05f);
        }
    }

    void RunningStepTwo()
    {
        if (PlayerController.global.running && !PlayerController.global.aiming && timer >= 0.2f)
        {
            timer = 0f;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStep2Sound, 0.05f);
        }
    }

    void AimingStepOne()
    {
        if (!PlayerController.global.running && PlayerController.global.aiming && (timer >= 0.2f || !initialStep))
        {
            initialStep = true;
            timer = 0f;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound, 0.05f);
        }
    }

    void AimingStepTwo()
    {
        if (!PlayerController.global.running && PlayerController.global.aiming && timer >= 0.2f)
        {
            timer = 0f;
            GameManager.global.SoundManager.PlaySound(GameManager.global.PlayerStepSound, 0.05f);
        }
    }

    void Death()
    {
        PlayerController.global.playerDead = true;
    }
}