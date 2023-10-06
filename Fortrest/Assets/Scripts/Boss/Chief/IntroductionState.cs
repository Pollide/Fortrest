using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionState : BossState
{
    public Transform targetEnemy; // The enemy you want to focus on
    public float introDuration = 3.0f; // Duration of the intro animation
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new(0, 2, -2); // Offset from the enemy's position during intro

    private float introTimer = 0.0f;
    private float waitBeforeStart = 3.0f;
    private bool introCompleted = false;
    private bool introStarted = false;
    [SerializeField] private IdleState idleState;

    public override void EnterState()
    {
        if (stateMachine.bossSpawner)
        {
            // Checks if the attack state is null
            if (idleState == null)
            {
                // Gets the connected attack state
                idleState = GetComponent<IdleState>();
            }

            LevelManager.global.HUD.SetActive(false);

            PlayerController.global.playerCanMove = false;
            PlayerController.global.CharacterAnimator.SetBool("Moving", false);

            CameraFollow.global.bossCam = true;

            ScreenShake.global.duration = 3f;

            if (stateMachine && !introStarted && stateMachine.BossType == BossSpawner.TYPE.Chieftain)
            {
                StartCoroutine(Intro());
            }
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
    }

    public override void UpdateState()
    {
        if (!introCompleted && !ScreenShake.global.shake)
        {
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
            stateMachine.ChangeState(idleState);
        }
    }


    public IEnumerator Intro()
    {
        yield return new WaitForSeconds(0.005f);
        stateMachine.bossSpawner.BossMusicBegin(true);
        stateMachine.BossAnimator.speed = 0;
        transform.position = initialSpawn;
        yield return new WaitForSeconds(waitBeforeStart);
        stateMachine.BossAnimator.speed = 1;
    }
}
