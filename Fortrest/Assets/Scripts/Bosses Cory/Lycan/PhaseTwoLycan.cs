using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoLycan : BossState
{
    [SerializeField] private Transform[] spawnLocation;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<GameObject> enemyList;
    [SerializeField] private BossState nextState;
    [SerializeField] private BossState idleState;
    [SerializeField] private bool introRan;

    public override void EnterState()
    {
        if (enemyList.Count > 0)
        {
            for (int i = 0; i < spawnLocation.Length; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnLocation[i]);

                enemy.GetComponent<EnemyController>().bossScriptTwo = this;
                enemy.GetComponent<EnemyController>().isMob = true;

                enemyList.Add(enemy);

            }
        }

        introRan = false;
    }

    public override void ExitState()
    {
        GetComponent<IdleState>().lastState = this;
    }

    public override void UpdateState()
    {
        // Switch to Idle if player is outside of arena
        if (!PlayerInArena(stateMachine.ArenaSize))
        {
            stateMachine.ChangeState(idleState);
        }

        if (enemyList.Count == 0 && PlayerInArena(stateMachine.ArenaSize))
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
    }

    public void DestroyEnemies()
    {
        if (enemyList.Count > 0)
        {
            foreach (var enemy in enemyList)
            {
                enemy.GetComponent<EnemyController>().Death();
            }

            enemyList.Clear();
        }
    }

    public List<GameObject> EnemyList
    {
        get { return enemyList; }
    }
}
