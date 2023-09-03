using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BossState
{
    private AttackState attackState;

    public override void EnterState()
    {
        if (attackState == null)
        {
            attackState = GetComponent<AttackState>();
        }
    }

    public override void ExitState()
    {
        if (attackState != null)
        {
            attackState = null;
        }
    }

    public override void UpdateState()
    {
        if (PlayerInArena(stateMachine.GetArenaSize()))
        {
            stateMachine.ChangeState(attackState);
        }
    }
}
