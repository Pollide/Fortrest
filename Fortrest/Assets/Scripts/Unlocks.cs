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
        mountUnlocked = false;
        bowUnlocked = false;
        miniTurretUnlocked = false;
        extraApplesUnlocked = false;
        upgradedMeleeUnlocked = false;
        upgradedBowUnlocked = false;
    }

    void Update()
    {
        LevelManager.ProcessBossList((boss) =>
        {           
            if (boss.dead)
            {
                switch (boss.bossType)
                {
                    case BossHandler.TYPE.Chieftain:
                        mountUnlocked = true;
                        break;
                    case BossHandler.TYPE.Basilisk:
                        bowUnlocked = true;
                        break;
                    case BossHandler.TYPE.SpiderQueen:
                        miniTurretUnlocked = true;
                        break;
                    case BossHandler.TYPE.Tier4:
                        extraApplesUnlocked = true;
                        break;
                    case BossHandler.TYPE.Tier5:
                        upgradedMeleeUnlocked = true;
                        break;
                    case BossHandler.TYPE.Tier6:
                        upgradedBowUnlocked = true;
                        break;
                    default:
                        break;
                }
            }           
        });
    }
}