using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BirdBossFly : StateMachineBehaviour
{
    private Transform playerTransform;
    private NavMeshAgent agent;
    private BirdBoss birdScript;
    private float timer;
    private float attackCD = 5.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = PlayerController.global.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        birdScript = animator.GetComponent<BirdBoss>();
        timer = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(playerTransform.position);
        timer += Time.deltaTime;
        if (timer > attackCD)
        {
            //birdScript.Attack();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}