using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroToPhaseTwo : BossState
{
    [SerializeField] private BossState nextState;

    public override void EnterState()
    {
        Debug.Log("INTRO 2");
        stateMachine.bossSpawner.BossIntro();
    }

    public override void ExitState()
    {
        //not in use anymore, found in boss spawner
    }

    public override void UpdateState()
    {
        //not in use anymore, found in boss spawner
    }
}
