using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirdBossAttack : StateMachineBehaviour
{
    private BirdBoss birdScript;
    private Vector3 target;
    private Vector3 directionToTarget;
    private GameObject telegraph;
    private bool sliding;
    private float timer;
    public Vector3 smokePosition;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        birdScript = animator.GetComponent<BirdBoss>();
        target = birdScript.playerTransform.position + (birdScript.directionToPlayerNoY.normalized * 20.0f);
        directionToTarget = (new Vector3(target.x, 0f, target.z) - new Vector3(birdScript.transform.position.x, 0f, birdScript.transform.position.z)).normalized;
        birdScript.normalAttackIndicator = true;
        //telegraph = Instantiate(birdScript.telegraphedRectangle, new Vector3(birdScript.playerTransform.position.x, 0f, birdScript.playerTransform.position.z), birdScript.transform.rotation);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!birdScript.diving && !sliding)
        {
            target = birdScript.playerTransform.position + (directionToTarget * 20.0f);
        }
        else if (birdScript.diving)
        {
            target = new Vector3(birdScript.playerTransform.position.x, 0f, birdScript.playerTransform.position.z) - (directionToTarget * 7.5f);
            if (birdScript.transform.position == target)
            {               
                sliding = true;
                birdScript.diving = false;
            }
        }
        else
        {
            target = new Vector3(birdScript.playerTransform.position.x, 0f, birdScript.playerTransform.position.z) + (directionToTarget * 20.0f);
            timer += Time.deltaTime;
            if (timer > 0.06f)
            {
                timer = 0f;
                GameObject SmokeVFXRight = Instantiate(LevelManager.global.VFXSmoke.gameObject, birdScript.transform.position + smokePosition, Quaternion.identity);
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
        Destroy(telegraph);
    }
}