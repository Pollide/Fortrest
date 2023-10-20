using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossCrash : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private float timer;
    private float recoveryTime = 5.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript = animator.GetComponent<BirdBoss>();
        birdScript.hitReceived = 0;
        timer = 0f;
        birdScript.crashed = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > recoveryTime)
        {
            animator.SetTrigger("Recover");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Recover");
        birdScript.crashed = false;
    }
}