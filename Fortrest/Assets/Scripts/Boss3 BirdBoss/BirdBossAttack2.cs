using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossAttack2 : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private Vector3 target;
    private Vector3 directionToTarget;
    private float timer1, timer2;
    private float rockCD = 0.6f;
    private float attackCD = 2.0f;
    private bool throwRock;
    private int rocks;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        birdScript = animator.GetComponent<BirdBoss>();
        target = birdScript.playerTransform.position + (birdScript.directionToPlayerNoY.normalized * 150.0f);
        directionToTarget = (new Vector3(target.x, 0f, target.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
        rocks = 0;
        throwRock = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.MoveToTarget(target, directionToTarget);

        if (rocks < 6)
        {
            if (!throwRock)
            {
                rocks++;
                GameObject telegraph = Instantiate(birdScript.telegraphedCircle, new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z), Quaternion.identity);
                Destroy(telegraph, 5.0f);
                throwRock = true;
            }
            else
            {
                timer1 += Time.deltaTime;
                if (timer1 > rockCD)
                {
                    throwRock = false;
                    timer1 = 0f;
                }
            }           
        }
        if (birdScript.outOfScreen)
        {
            timer2 += Time.deltaTime;
            if (timer2 > attackCD)
            {
                birdScript.targetReached = true;
                birdScript.playerReached = false;
                animator.SetBool("Attack2", false);
                timer2 = 0f;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.normalAttack = true;
    }
}