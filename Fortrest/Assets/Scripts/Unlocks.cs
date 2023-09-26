using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
    public static Unlocks global;

    public bool mountUnlocked;
    public bool bowUnlocked;
    public bool miniTurretUnlocked;
    public bool extraApplesUnlocked;
    public bool upgradedMeleeUnlocked;
    public bool upgradedBowUnlocked;

    private void Awake()
    {
        global = this;
    }

    void Start()
    {
        mountUnlocked = true;
        bowUnlocked = true;
        miniTurretUnlocked = true;
        extraApplesUnlocked = false;
        upgradedMeleeUnlocked = false;
        upgradedBowUnlocked = false;
    }

    void Update()
    {
        LevelManager.ProcessBossList((boss) =>
        {           
            if (boss.IsDead)
            {
                switch (boss.BossType)
                {
                    case BossStateMachine.TYPE.Chieftain:
                        mountUnlocked = true;
                        break;
                    case BossStateMachine.TYPE.Basilisk:
                        bowUnlocked = true;
                        break;
                    case BossStateMachine.TYPE.SpiderQueen:
                        miniTurretUnlocked = true;
                        break;
                    case BossStateMachine.TYPE.Bird:
                        extraApplesUnlocked = true;
                        break;
                    case BossStateMachine.TYPE.Werewolf:
                        upgradedMeleeUnlocked = true;
                        break;
                    case BossStateMachine.TYPE.Fire:
                        upgradedBowUnlocked = true;
                        break;
                    default:
                        break;
                }
            }           
        });
    }
}