using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FrenzyMode : BossState
{
    [SerializeField] private BossState idleState;
    //  [SerializeField] private float nextAttackTime = 0f;
    [SerializeField] private float jumpDistance = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    public float timeBetweenJump = 2f;
    public bool attacking = false;
    public bool stopRotation = false;
    public bool telegraph;
    public float rotationSpeed = 4;

    public override void EnterState()
    {
        stateMachine.CurrentPhase = BossStateMachine.BossPhase.Three;
        stateMachine.BossAnimator.ResetTrigger("Frenzy");
        stateMachine.BossAnimator.SetTrigger("Frenzy");
        stateMachine.BossAnimator.SetBool("inFrenzy", true);
        GetComponent<NavMeshAgent>().speed = jumpSpeed;
        GetComponent<NavMeshAgent>().acceleration = jumpSpeed * 10;
        stopRotation = true;
    }

    public override void ExitState()
    {
        GetComponent<IdleState>().lastState = this;
    }

    public override void UpdateState()
    {

        // Switch to Idle if player is outside of arena
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (attacking)
        {
            GetComponent<NavMeshAgent>().isStopped = false;
            WalkTo(transform.position + transform.forward * jumpDistance, 1);
        }
        else
        {
            GetComponent<NavMeshAgent>().isStopped = true;
            if (!stopRotation)
            {
                // Calculate the direction to the target
                Vector3 targetDirection = playerTransform.position - transform.position;

                // Calculate the rotation needed to face the target
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                // Smoothly interpolate the current rotation to the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
