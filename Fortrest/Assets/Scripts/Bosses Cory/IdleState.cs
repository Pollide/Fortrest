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
        stateMachine.inIdle = true;
        resetTimer = resetTimerDuration;
        introRan = false;
    }

    public override void ExitState()
    {
        stateMachine.inIdle = false;
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
                    if (!introRan && idleRuns > 1)
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

                stateMachine.bossSpawner.UpdateHealth(stateMachine.bossSpawner.maxHealth - stateMachine.bossSpawner.health);

                stateMachine.PhaseTwoRan = false;
                stateMachine.PhaseThreeRan = false;

                stateMachine.CurrentPhase = BossStateMachine.BossPhase.One;

                if (GetComponent<PhaseOneLycan>())
                {
                    GetComponent<PhaseOneLycan>().DestroyEnemies();
                }
                if (GetComponent<PhaseTwoLycan>())
                {
                    GetComponent<PhaseTwoLycan>().DestroyEnemies();
                }
                
            }
            else
            {
                resetTimer -= Time.deltaTime;
            }

            if (stateMachine.BossType == BossSpawner.TYPE.Chieftain)
            {
                WalkTo(initialSpawn, stoppingDistance);
            }
            else if (stateMachine.BossType == BossSpawner.TYPE.Lycan)
            {
                WalkTo(initialSpawn, stoppingDistance);
            }
        }
    }

    public int IdleRuns
    {
        get { return idleRuns; }
    }
}
