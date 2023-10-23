using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BossState
{
    // Holds next state
    [SerializeField] private BossState nextStateOne;
    [SerializeField] private BossState nextStateIntro;
    public BossState lastState;
    private float resetTimer = 0f;
    [SerializeField] private int idleRuns = 0;
    [SerializeField] private float resetTimerDuration;
    [SerializeField] private float stoppingDistance = 3f;

    public override void EnterState()
    {
        idleRuns++;

        resetTimer = resetTimerDuration;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        // checks if player is in the arena 
        if (PlayerInArena(stateMachine.ArenaSize))
        {
            // changes to attack state
            if (idleRuns <= 1 && stateMachine.BossType == BossSpawner.TYPE.Lycan)
            {
                stateMachine.ChangeState(nextStateOne);
            }
            else if (idleRuns > 1 && stateMachine.BossType == BossSpawner.TYPE.Lycan)
            {
                if (resetTimer <= 0)
                {
                    stateMachine.ChangeState(nextStateIntro);
                }
                else
                {
                    stateMachine.ChangeState(lastState);
                }

            }
            else if (stateMachine.BossType != BossSpawner.TYPE.Lycan)
            {
                stateMachine.ChangeState(nextStateOne);
            }
        }
        else
        {
            if (resetTimer <= 0f)
            {
                stateMachine.bossSpawner.Awake();
                StateMachine.bossSpawner.BossEncountered(false);
                if (GetComponent<PhaseOneLycan>())
                {
                    GetComponent<PhaseOneLycan>().DestroyEnemies();
                }
                if (GetComponent<PhaseTwoLycan>())
                {
                    GetComponent<PhaseTwoLycan>().DestroyEnemies();
                }
            }

            if (stateMachine.BossType == BossSpawner.TYPE.Chieftain)
            {
                WalkTo(initialSpawn, stoppingDistance);
            }
            else if (stateMachine.BossType == BossSpawner.TYPE.Lycan)
            {
                WalkTo(initialSpawn, stoppingDistance);
            }

            resetTimer -= Time.deltaTime;
        }
    }

    public int IdleRuns
    {
        get { return idleRuns; }
    }
}
