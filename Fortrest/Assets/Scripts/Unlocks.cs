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

#if UNITY_EDITOR
        mountUnlocked = true;
        bowUnlocked = true;
        miniTurretUnlocked = true;
#endif
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
                    case BossSpawner.TYPE.Hrafn:
                        extraApplesUnlocked = true;
                        break;
                    case BossSpawner.TYPE.Lycan:
                        upgradedMeleeUnlocked = true;
                        break;
                    case BossSpawner.TYPE.Squid:
                        upgradedBowUnlocked = true;
                        break;
                    default:
                        break;
                }
            }
        });

        PlayerController.global.MiniTurretUI.SetActive(miniTurretUnlocked);
    }
}