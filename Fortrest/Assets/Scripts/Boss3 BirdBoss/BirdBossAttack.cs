using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdBossAttack : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private bool sliding;
    private float timer;
    private bool newTarget;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        birdScript = animator.GetComponent<BirdBoss>();                
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        if (!birdScript.diving && !sliding)
        {
            birdScript.targetPosition = birdScript.playerTransform.position;
            birdScript.targetDirection = (new Vector3(birdScript.targetPosition.x, 0f, birdScript.targetPosition.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
        }
        else if (birdScript.diving)
        {
            if (!newTarget)
            {
                birdScript.targetPosition = (new Vector3(birdScript.playerTransform.position.x, 0f, birdScript.playerTransform.position.z) + new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)) / 2;
                birdScript.targetDirection = (new Vector3(birdScript.targetPosition.x, 0f, birdScript.targetPosition.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;               
                newTarget = true;
            }
            if (birdScript.transform.position == birdScript.targetPosition)
            {              
                sliding = true;
                birdScript.diving = false;              
            }
        }
        else if (sliding && birdScript.flyAnimOver)
        {
            birdScript.targetPosition = birdScript.targetPosition + (birdScript.targetDirection * 12.5f);

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
        birdScript.MoveToTarget(birdScript.targetPosition, birdScript.targetDirection);        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        birdScript.vulnerable = false;
        birdScript.flying = true;
        birdScript.normalAttack = false;
        sliding = false;
        newTarget = false;
    }    
}