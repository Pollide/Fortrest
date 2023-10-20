using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroToPhaseThree : BossState
{
    public Transform targetEnemy; // The enemy you want to focus on
    public float introDuration = 3.0f; // Duration of the intro animation
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new(0, 2, -2); // Offset from the enemy's position during intro

    private float introTimer = 0.0f;
    private bool introCompleted = false;
    [SerializeField] private BossState nextState;

    public override void EnterState()
    {
        stateMachine.BossAnimator.ResetTrigger("Roar");
        stateMachine.BossAnimator.SetTrigger("Roar");

        if (stateMachine.bossSpawner)
        {
            LevelManager.global.HUD.SetActive(false);

            PlayerController.global.playerCanMove = false;
            PlayerController.global.characterAnimator.SetBool("Moving", false);

            CameraFollow.global.bossCam = true;

            ScreenShake.global.duration = 3f;
        }
    }

    public override void ExitState()
    {
        if (stateMachine.bossSpawner)
        {
            CameraFollow.global.bossCam = false;
            PlayerController.global.playerCanMove = true;
            LevelManager.global.HUD.SetActive(true);
        }

        introTimer = 0f;
        introCompleted = false;
    }

    public override void UpdateState()
    {
        if (!introCompleted && !ScreenShake.global.shake)
        {
            if (introTimer == 0)
                stateMachine.bossSpawner.BossMusicBegin(true);

            introTimer += Time.deltaTime;

            // Calculate the interpolation factor
            float introProgress = Mathf.Clamp01(introTimer / introDuration);

            // Perform the intro animation
            Vector3 targetPosition = targetEnemy.position + introPositionOffset;
            Vector3 cameraPosition = Vector3.Lerp(LevelManager.global.SceneCamera.transform.position, targetPosition - LevelManager.global.SceneCamera.transform.forward, introProgress);
            LevelManager.global.SceneCamera.transform.position = cameraPosition;

            if (introProgress >= 1.0f)
            {
                introCompleted = true;
            }
        }

        if (introCompleted)
        {
            stateMachine.ChangeState(nextState);
        }
    }
}
