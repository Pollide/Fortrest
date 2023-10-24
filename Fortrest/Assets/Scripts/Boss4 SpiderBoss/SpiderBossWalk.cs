using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderBossWalk : StateMachineBehaviour
{
    private Transform playerTransform;
    private NavMeshAgent agent;
    private SpiderBoss spiderScript;
    private float attackRange;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = PlayerController.global.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        spiderScript = animator.GetComponent<SpiderBoss>();
        attackRange = agent.stoppingDistance + 0.5f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!spiderScript.retreating)
        {
            if (Vector3.Distance(playerTransform.position, agent.transform.position) <= attackRange && !spiderScript.specialAttackReady)
            {
                spiderScript.Attack();
            }
            else
            {
                agent.SetDestination(playerTransform.position);
            }
        }
        else
        {
            agent.SetDestination(spiderScript.startPosition);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}