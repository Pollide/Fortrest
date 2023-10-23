using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossAttack2 : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private float timer1, timer2;
    private float rockCD = 0.5f;
    private float attackCD = 2.0f;
    private bool throwRock;
    private int rocks;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        birdScript = animator.GetComponent<BirdBoss>();
        birdScript.targetPosition = birdScript.playerTransform.position + (birdScript.directionToPlayer.normalized * 150.0f);
        birdScript.targetDirection = (new Vector3(birdScript.targetPosition.x, 0f, birdScript.targetPosition.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
        rocks = 0;
        throwRock = false;
        birdScript.displayedRock.SetActive(true);
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossPreAttack2Sound);
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossHoverSound, 1f, true, 0, false, birdScript.transform);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript.MoveToTarget(birdScript.targetPosition, birdScript.targetDirection);

        if (birdScript.flyAnimOver)
        {
            if (rocks < 7)
            {
                if (!throwRock)
                {                   
                    rocks++;
                    GameObject rockObject = Instantiate(birdScript.rockObject, birdScript.transform.position, Quaternion.identity);
                    GameObject telegraph = Instantiate(birdScript.telegraphedCircle, new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z), Quaternion.identity);
                    telegraph.GetComponentInChildren<TelegraphedAttack>().getRockObject(rockObject);
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
        }

        if (birdScript.IsOutOfScreen())
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
        birdScript.lastWasNormal = false;
        birdScript.displayedRock.SetActive(false);
        GameManager.global.SoundManager.StopSelectedSound(GameManager.global.BirdBossHoverSound);
    }
}