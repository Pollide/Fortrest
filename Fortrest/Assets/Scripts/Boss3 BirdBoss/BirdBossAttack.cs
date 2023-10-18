using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossAttack : StateMachineBehaviour
{
    private BirdBoss birdScript;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        birdScript = animator.GetComponent<BirdBoss>();
        birdScript.vulnerable = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.MoveToTarget(birdScript.playerTransform.position, birdScript.directionToPlayerNoY);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.playerReached = true;
        birdScript.targetReached = false;
        birdScript.vulnerable = false;
    }
}