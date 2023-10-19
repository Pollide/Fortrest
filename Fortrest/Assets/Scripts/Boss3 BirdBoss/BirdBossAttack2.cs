using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossAttack2 : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private Vector3 target;
    private Vector3 directionToTarget;
    private float timer;
    private float attackCD = 2.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript = animator.GetComponent<BirdBoss>();
        target = birdScript.playerTransform.position + (birdScript.directionToPlayerNoY.normalized * 50.0f);
        directionToTarget = (new Vector3(target.x, 0f, target.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.MoveToTarget(target, directionToTarget);
        if (birdScript.outOfScreen)
        {          
            timer += Time.deltaTime;
            if (timer > attackCD)
            {
                birdScript.targetReached = true;
                birdScript.playerReached = false;
                animator.SetBool("Attack2", false);
                timer = 0f;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.normalAttack = true;
    }
}