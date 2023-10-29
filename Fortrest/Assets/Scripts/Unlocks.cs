using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unlocks : MonoBehaviour
{
    public static Unlocks global;

    public bool mountUnlocked;
    public bool bowUnlocked;
    public bool miniTurretUnlocked;
    public bool extraApplesUnlocked;
    public bool upgradedMeleeUnlocked;
    public bool upgradedBowUnlocked;

    public Sprite mountSprite;
    public Sprite bowSprite;
    public Sprite miniTurretSprite;
    public Sprite extraApplesSprite;
    public Sprite upgradedMeleeSprite;
    public Sprite upgradedBowSprite;
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

        RefreshUnlocks();
    }

    void BossUnlockAnimation(Sprite sprite, string title, string description)
    {
        GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Unlock");
        PlayerController.global.unlockImage.sprite = sprite;
        PlayerController.global.unlockTitleText.text = title;
        PlayerController.global.unlockDescriptionText.text = description;
    }

    public void RefreshUnlocks(BossSpawner defeated = null)
    {
        LevelManager.ProcessBossList((boss) =>
        {
            if (boss.health <= 0)
            {
                switch (boss.bossType)
                {
                    case BossSpawner.TYPE.Basilisk: //UNUSED?

                        extraApplesUnlocked = true;

                        if (defeated == boss)
                            BossUnlockAnimation(extraApplesSprite, "Extra Apples", "Can now hold more apples");

                        break;
                    case BossSpawner.TYPE.Chieftain:
                        bowUnlocked = true;

                        if (defeated == boss)
                        {
                            string top = "While in combat mode, hold " + (GameManager.global.KeyboardBool ? "right click" : "the left trigger") + " to aim.\n";
                            string bottom = "Then " + (GameManager.global.KeyboardBool ? "left click" : "use right trigger") + " to shoot.";
                            BossUnlockAnimation(bowSprite, "Bow Unlocked", top + bottom);
                        }
                        break;
                    case BossSpawner.TYPE.Hrafn:


                        mountUnlocked = true;

                        if (defeated == boss)
                            BossUnlockAnimation(mountSprite, "Mount Unlocked", "Return home to find your ride");
                        break;
                    case BossSpawner.TYPE.Lycan:
                        upgradedMeleeUnlocked = true;

                        if (defeated == boss)
                            BossUnlockAnimation(upgradedMeleeSprite, "Upgraded Sword", "Your sword is now more powerful");
                        break;
                    case BossSpawner.TYPE.SpiderQueen:

                        miniTurretUnlocked = true;

                        if (defeated == boss)
                            BossUnlockAnimation(miniTurretSprite, "Mini Turret Unlocked", "Press " + (GameManager.global.KeyboardBool ? "T" : "the right bumper") + "to place");
                        break;
                    case BossSpawner.TYPE.IsleMaker:
                        upgradedBowUnlocked = true;

                        if (defeated == boss)
                            BossUnlockAnimation(upgradedMeleeSprite, "Upgraded Bow", "Your bow is now more powerful");
                        break;
                    default:
                        break;
                }
            }
        });
    }

    private void Update()
    {
        if (!PlayerModeHandler.global.inTheFortress)
            PlayerController.global.MiniTurretUI.SetActive(miniTurretUnlocked);
    }
}