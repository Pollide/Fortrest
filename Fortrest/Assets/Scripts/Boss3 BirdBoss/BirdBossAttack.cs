using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossAttack : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private Vector3 target, target2;
    private Vector3 directionToTarget;
    GameObject telegraph;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        birdScript = animator.GetComponent<BirdBoss>();
        birdScript.vulnerable = true;
        target = birdScript.playerTransform.position + (birdScript.directionToPlayerNoY.normalized * 50.0f);
        directionToTarget = (new Vector3(target.x, 0f, target.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
        birdScript.normalAttackIndicator = true;
        telegraph = Instantiate(birdScript.telegraphedRectangle, birdScript.playerTransform.position, birdScript.transform.rotation);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.MoveToTarget(target, directionToTarget);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        birdScript.vulnerable = false;
        birdScript.flying = true;
        birdScript.normalAttack = false;
        Destroy(telegraph);
    }
}