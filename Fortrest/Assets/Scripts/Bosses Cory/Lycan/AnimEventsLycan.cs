using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsLycan : MonoBehaviour
{
    public FrenzyMode frenzy;
    [SerializeField] private AudioClip swipeAudio;
    [SerializeField] private AudioClip enrageAudio;
    [SerializeField] private AudioClip jumpAudio;

    void PlayEnrageSound()
    {
        ScreenShake.global.ShakeScreen(1);
        GameManager.global.SoundManager.PlaySound(enrageAudio);
    }
    void PlaySwipeSound()
    {
        GameManager.global.SoundManager.PlaySound(swipeAudio);
    }

    void PlayJumpSound()
    {
        GameManager.global.SoundManager.PlaySound(jumpAudio);
    }


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

    void AttackCounter()
    {
        frenzy.attackCounter++;
    }

    void SetTiredFalse()
    {
        frenzy.StateMachine.BossAnimator.SetBool("isTired", false);
        frenzy.StateMachine.BossAnimator.SetBool("inFrenzy", true);
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