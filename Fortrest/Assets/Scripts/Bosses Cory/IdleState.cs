using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BossState
{
    // Holds next state
    [SerializeField] private BossState nextState;
    public BossState lastState;
    [SerializeField] private float resetTimer = 0f;
    [SerializeField] private int idleRuns = 0;
    [SerializeField] private float resetTimerDuration;
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private bool introRan;

    public override void EnterState()
    {
        idleRuns++;

        resetTimer = resetTimerDuration;
        introRan = false;
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
            if (stateMachine.BossType == BossSpawner.TYPE.Lycan)
            {
                if (resetTimer <= 0 || resetTimer == resetTimerDuration)
                {
                    if (!introRan)
                    {
                        stateMachine.bossSpawner.BossIntro();
                        introRan = true;
                    }

                    if (stateMachine.bossSpawner.introCompleted)
                    {
                        stateMachine.ChangeState(nextState);
                    }

                }
                else
                {
                    stateMachine.ChangeState(lastState);
                }

            }
            else if (stateMachine.BossType != BossSpawner.TYPE.Lycan)
            {
                stateMachine.ChangeState(nextState);
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
