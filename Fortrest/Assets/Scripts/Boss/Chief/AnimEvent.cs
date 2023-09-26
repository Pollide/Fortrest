using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    [SerializeField] private FirstAttackState attackState1;
    [SerializeField] private SecondAttackState attackState2;
    [SerializeField] private ThirdAttackState attackState3;
    [SerializeField] private BossStateMachine stateMachine;
    [SerializeField] private BoxCollider bossCollider;
    [SerializeField] private AudioClip slamAudio;


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

    void SetHasJumped()
    {
        attackState3.HasJumped = true;
    }

    void SetAttackFalse()
    {
        GetComponentInParent<Animator>().SetBool("attacking", false);
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
