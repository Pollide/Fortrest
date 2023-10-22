using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossFly : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private float timer;
    private float attackCD = 2.0f;
    private bool targetSet;
    float x, z;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        birdScript = animator.GetComponent<BirdBoss>();
        birdScript.flyAnimOver = false;
        timer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (birdScript.altitudeReached)
        {
            if (!birdScript.playerReached && birdScript.targetReached)
            {
                birdScript.MoveToTarget(birdScript.playerTransform.position, birdScript.directionToPlayer);
                if (birdScript.distanceToPlayer < 30.0f)
                {
                    if (birdScript.normalAttack)
                    {
                        animator.SetTrigger("Attack1");
                    }
                    else
                    {
                        animator.SetBool("Attack2", true);
                        birdScript.normalAttack = true;
                    }
                }
            }    
            if (!birdScript.targetReached && birdScript.playerReached)
            {
                if (!targetSet)
                {
                    x = Random.Range(0, 2) == 0 ? Random.Range(40f, 150f) : Random.Range(-40f, -150f);
                    z = Random.Range(0, 2) == 0 ? Random.Range(40f, 150f) : Random.Range(-40f, -150f);
                    targetSet = true;
                }
                birdScript.targetPosition = birdScript.transform.position + new Vector3(x, 0f, z);
                birdScript.targetDirection = (new Vector3(birdScript.targetPosition.x, 0f, birdScript.targetPosition.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
                birdScript.MoveToTarget(birdScript.targetPosition, birdScript.targetDirection);
                if (birdScript.IsOutOfScreen())
                {
                    timer += Time.deltaTime;
                    if (timer > attackCD)
                    {
                        birdScript.targetReached = true;
                        birdScript.playerReached = false;
                        targetSet = false;
                        timer = 0f;
                    }
                }
            }
        }                
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack1");
        birdScript.playerReached = true;
        birdScript.targetReached = false;
        birdScript.flyAnimOver = true;
    }
}