using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventChief : MonoBehaviour
{
    [SerializeField] private ChargeStateChief state;
    [SerializeField] private BossStateMachine stateMachine;
    [SerializeField] private BoxCollider bossCollider;

    void SetAttackFalse()
    {
        GetComponentInParent<Animator>().SetBool("attacking", false);
    }

    void EndCharge()
    {
        StartCoroutine(state.StopCharging());
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
