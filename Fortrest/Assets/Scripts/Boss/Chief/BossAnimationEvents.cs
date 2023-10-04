using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAnimationEvents : MonoBehaviour
{
    [SerializeField] private AttackManagerState attackState1;
    [SerializeField] private PhaseTwoAttack attackState2;
    [SerializeField] private PhaseThreeAttack attackState3;
    [SerializeField] private BossStateMachine stateMachine;
    [SerializeField] private BoxCollider bossCollider;
    [SerializeField] private AudioClip slamAudio;
    [SerializeField] private AudioClip enrageAudio;
    [SerializeField] private GameObject introCard;


    void ActivateIntroCard()
    {
        introCard.SetActive(true);
        stateMachine.BossAnimator.speed = 0f;
    }

    void PlaySlashOne()
    {
        attackState1.PlaySlash(0);
        GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing1Sound);
    }

    void PlaySlashTwo()
    {
        attackState1.PlaySlash(1);
        GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing2Sound);
    }

    void PlaySlashThree()
    {
        attackState1.PlaySlash(2);
        GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing2Sound);
    }

    void PlaySwordRelease()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.SwordSwing3Sound);
    }

    void PlaySlamSound()
    {
        GameManager.global.SoundManager.PlaySound(slamAudio);
    }
    void PlayEnrageSound()
    {
        GameManager.global.SoundManager.PlaySound(enrageAudio);
    }
    void AttackPlusPlus()
    {
        attackState1.attackCounter++;
    }

    void Attack3()
    {
        if (attackState1.attackCounter >= 3)
        {
            stateMachine.BossAnimator.SetBool("isTired", true);
        }
        else
        {
            attackState1.GetComponent<NavMeshAgent>().isStopped = false;
        }
    }

    void DeactivateAgent()
    {
        attackState1.GetComponent<NavMeshAgent>().isStopped = false;
    }

    void DeactivateAttacking()
    {
        attackState1.IsAttacking = false;
    }

    void SetTiredFalse()
    {

        stateMachine.BossAnimator.SetBool("isTired", false);

    }

    void SetTelegraphFalse()
    {
        attackState1.SetTeleFalse();
    }

    void PlayScreenShake()
    {
        ScreenShake.global.shake = true;
    }

    void SetHasJumped()
    {
        attackState3.HasJumped = true;
    }

    void SetAttackFalse()
    {
        GetComponentInParent<Animator>().SetBool("attacking", false);
    }  
    
    void PauseAnimater()
    {
        GetComponentInParent<Animator>().speed = 0f;
    } 
    
    void SetDiveFalse()
    {
        stateMachine.BossAnimator.SetBool("isDiving", false);
    }

    void EndCharge()
    {
        StartCoroutine(attackState2.StopCharging());
    }

    void InDefence()
    {
        stateMachine.InDefence = true;
    }

    void NotInDefence()
    {
        stateMachine.InDefence = false;
    }

    void DisableCollider()
    {
        bossCollider.enabled = false;
    }
    void EnableCollider()
    {
        bossCollider.enabled = true;
    }
}
