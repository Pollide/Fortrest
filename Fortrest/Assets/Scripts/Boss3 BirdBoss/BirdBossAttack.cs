using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdBossAttack : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private Vector3 target, target2;
    private Vector3 directionToTarget;   
    private bool sliding;
    private float timer;
    private bool newTarget;
    //private GameObject telegraph;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        birdScript = animator.GetComponent<BirdBoss>();             
        //birdScript.normalAttackIndicator = true;       
        //telegraph = Instantiate(birdScript.telegraphedRectangle, new Vector3(birdScript.playerTransform.position.x, 0f, birdScript.playerTransform.position.z), birdScript.transform.rotation);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        if (!birdScript.diving && !sliding)
        {
            target = birdScript.playerTransform.position + (birdScript.directionToPlayerNoY.normalized * 20.0f);
            directionToTarget = (new Vector3(target.x, 0f, target.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
        }
        else if (birdScript.diving)
        {
            if (!newTarget)
            {
                target = new Vector3(birdScript.playerTransform.position.x, 0f, birdScript.playerTransform.position.z) - (directionToTarget * 7.5f);
                directionToTarget = (new Vector3(target.x, 0f, target.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
                target2 = target + (directionToTarget * 12.5f);
                newTarget = true;
            }
            if (birdScript.transform.position == target)
            {
                sliding = true;
                birdScript.diving = false;
            }
        }
        else if (sliding)
        {
            target = target2;

            timer += Time.deltaTime;
            if (timer > 0.06f)
            {
                timer = 0f;
                GameObject SmokeVFXRight = Instantiate(LevelManager.global.VFXSmoke.gameObject, birdScript.transform.position, Quaternion.identity);
                float randomeFloat = Random.Range(0.2f, 0.4f);
                SmokeVFXRight.transform.localScale = new Vector3(randomeFloat * 1.5f, randomeFloat, randomeFloat * 1.5f);
                SmokeVFXRight.GetComponent<VisualEffect>().Play();
                Destroy(SmokeVFXRight, 1.5f);
            }
        }
        birdScript.MoveToTarget(target, directionToTarget);        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        birdScript.vulnerable = false;
        birdScript.flying = true;
        birdScript.normalAttack = false;
        sliding = false;
        newTarget = false;
        //Destroy(telegraph);
    }    
}