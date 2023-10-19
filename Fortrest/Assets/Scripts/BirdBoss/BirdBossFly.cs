using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossFly : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private float timer;
    private float attackCD = 5.0f;
    private bool playerReached = false;
    private bool targetReached = true;
    private bool targetSet;
    private Vector3 destination;
    float x, z;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript = animator.GetComponent<BirdBoss>();
        timer = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!playerReached && targetReached && birdScript.distanceToPlayerNoY > birdScript.stoppingDistance)
        {
            birdScript.MoveToTarget(birdScript.playerTransform.position, birdScript.directionToPlayerNoY);
        }
        else
        {
            playerReached = true;
            targetReached = false;           
        }

        if (!targetReached && playerReached && !birdScript.outOfScreen)
        {
            if (!targetSet)
            {
                x = Random.Range(0, 2) == 0 ? Random.Range(30f, 150f) : Random.Range(-30f, -150f);
                z = Random.Range(0, 2) == 0 ? Random.Range(30f, 150f) : Random.Range(-30f, -150f);
                targetSet = true;
            }
            destination = birdScript.transform.position + new Vector3(x, 0f, z);
            Vector3 directionToTarget = (new Vector3(destination.x, 0f, destination.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
            birdScript.MoveToTarget(destination, directionToTarget);
        }
        else
        {
            targetReached = true;
            playerReached = false;
            targetSet = false;
        }
        
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