using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsLycan : MonoBehaviour
{
    public FrenzyMode frenzy;

    void isAttacking()
    {
        frenzy.attacking = true;
    }
    void isNotAttacking()
    {
        frenzy.attacking = false;
    }

    void StopRotation()
    {
        frenzy.stopRotation = true;
    }

    void DontStopRotation()
    {
        frenzy.stopRotation = false;
    }

    void TelegraphTrue()
    {
        frenzy.telegraph = true;
    }

    void TelegraphFalse()
    {
        frenzy.telegraph = false;
    }

    void StopAnim()
    {
        StartCoroutine(StopAndStartAnim());
    }

    IEnumerator StopAndStartAnim()
    {
        BossStateMachine stateMachine = GetComponentInParent<BossStateMachine>();

        stateMachine.BossAnimator.speed = 0.05f;

        yield return new WaitForSeconds(frenzy.timeBetweenJump);

        stateMachine.BossAnimator.speed = 1f;
    }
}