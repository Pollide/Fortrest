using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossAttack3 : StateMachineBehaviour
{
    private BirdBoss birdScript;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        birdScript = animator.GetComponent<BirdBoss>();
        birdScript.circleAttackIndicator = true;
        GameManager.global.SoundManager.PlaySound(GameManager.global.BirdBossPreAttack3Sound);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}