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
            if (boss.health <= 0)
            {
                switch (boss.bossType)
                {
                    case BossSpawner.TYPE.Chieftain:
                        mountUnlocked = true;
                        break;
                    case BossSpawner.TYPE.Basilisk:
                        bowUnlocked = true;
                        break;
                    case BossSpawner.TYPE.SpiderQueen:
                        miniTurretUnlocked = true;
                        break;
                    case BossSpawner.TYPE.Bird:
                        extraApplesUnlocked = true;
                        break;
                    case BossSpawner.TYPE.Werewolf:
                        upgradedMeleeUnlocked = true;
                        break;
                    case BossSpawner.TYPE.Fire:
                        upgradedBowUnlocked = true;
                        break;
                    default:
                        break;
                }
            }
        });
    }
}