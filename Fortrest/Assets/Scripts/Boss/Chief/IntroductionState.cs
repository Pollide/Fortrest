using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionState : BossState
{
    public Transform targetEnemy; // The enemy you want to focus on
    public float introDuration = 3.0f; // Duration of the intro animation
    public float cameraDistance = 5.0f; // Distance between the camera and the enemy
    public Vector3 introPositionOffset = new Vector3(0, 2, -2); // Offset from the enemy's position during intro

    private Transform initialCameraTransform;
    private float introTimer = 0.0f;
    private bool introCompleted = false;
    private bool introStarted = false;
    [SerializeField] private IdleState idleState;

    public override void EnterState()
    {
        // Checks if the attack state is null
        if (idleState == null)
        {
            // Gets the connected attack state
            idleState = GetComponent<IdleState>();
        }

        initialCameraTransform = Camera.main.transform;

        CameraFollow.global.bossCam = true;

        stateMachine.BossAnimator.SetBool("isDiving", true);

        if (!introStarted)
        {
            StartCoroutine(Intro());
        }
    }

    public override void ExitState()
    {
        stateMachine.BossAnimator.SetBool("isDiving", false);
        CameraFollow.global.bossCam = false;
    }

    public override void UpdateState()
    {
     

        if (!introCompleted)
        {
            introTimer += Time.deltaTime;

            // Calculate the interpolation factor
            float introProgress = Mathf.Clamp01(introTimer / introDuration);

            // Perform the intro animation
            Vector3 targetPosition = targetEnemy.position + introPositionOffset;
            Vector3 cameraPosition = Vector3.Lerp(initialCameraTransform.position, targetPosition - initialCameraTransform.forward, introProgress);
            Camera.main.transform.position = targetPosition;

            // Look at the target enemy
            Camera.main.transform.LookAt(targetEnemy);

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
        
        yield return new WaitForSeconds(0.2f);
        stateMachine.BossAnimator.speed = 0;
        yield return new WaitForSeconds(1);
        stateMachine.BossAnimator.speed = 1;
    }
}
