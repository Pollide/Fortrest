using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    [SerializeField] private FirstAttackState attackState1;
    [SerializeField] private SecondAttackState attackState2;
    [SerializeField] private BossStateMachine stateMachine;
    [SerializeField] private BoxCollider bossCollider;


    void PlaySlashOne()
    {
        attackState1.PlaySlash(0);
    }

    void PlaySlashTwo()
    {
        attackState1.PlaySlash(1);
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
