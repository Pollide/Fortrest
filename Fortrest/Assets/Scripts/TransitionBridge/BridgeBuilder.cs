using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BridgeBuilder : MonoBehaviour
{
    public int BridgeTypeInt;
    public bool isBuilt;
    bool triggered;
    public Animation FloatingTextAnimation;
    public Animation FloatingTextCanBuildAnimation;
    public GameObject WalkAccrossCollider;
    public Animator bridgeAnimator;
    bool canBuild;

    public bool debugforcebuild;

    private void Start()
    {
        if (BridgeTypeInt == 1)
        {
            Indicator.global.AddIndicator(transform, Color.magenta, "Marsh", false, Indicator.global.BridgeSprite);
        }

        if (BridgeTypeInt == 2)
        {
            Indicator.global.AddIndicator(transform, new Color(1.0f, 0.6f, 0.0f, 1.0f), "Tussocks", false, Indicator.global.BridgeSprite);
        }

        if (BridgeTypeInt == 3)
        {
            Indicator.global.AddIndicator(transform, Color.blue, "Taiga", false, Indicator.global.BridgeSprite);
        }

        if (BridgeTypeInt == 4)
        {
            Indicator.global.AddIndicator(transform, Color.yellow, "Coast", false, Indicator.global.BridgeSprite);
        }

        if (BridgeTypeInt == 5)
        {
            Indicator.global.AddIndicator(transform, Color.gray, "Volcanic Flats", false, Indicator.global.BridgeSprite);
        }

        LevelManager.global.bridgeList.Add(this);
    }
    void ShowPrompt(bool show)
    {
        PlayerController.global.needInteraction = show;
        PlayerController.global.bridgeInteract = show;

        canBuild = BridgeTypeInt == 2 || BridgeTypeInt == 4;

        for (int i = 0; i < LevelManager.global.bossList.Count; i++)
        {
            if (LevelManager.global.bossList[i].health <= 0)
            {
                if (BridgeTypeInt == 1 && LevelManager.global.bossList[i].bossType == BossSpawner.TYPE.Chieftain)
                {
                    canBuild = true;
                    break;
                }

                if (BridgeTypeInt == 3 && LevelManager.global.bossList[i].bossType == BossSpawner.TYPE.Basilisk)
                {
                    canBuild = true;
                    break;
                }

                if (BridgeTypeInt == 5 && LevelManager.global.bossList[i].bossType == BossSpawner.TYPE.SpiderQueen)
                {
                    canBuild = true;
                    break;
                }
            }
        }

        PlayerController.global.OpenResourceHolder(show);
        PlayerController.global.needInteraction = show;
        PlayerController.global.bridgeInteract = show;
        LevelManager.FloatingTextChange(canBuild ? FloatingTextCanBuildAnimation.gameObject : FloatingTextAnimation.gameObject, show);

        if (show)
            PlayerController.global.UpdateResourceHolder(BridgeTypeInt);
    }

    private void Update()
    {
        if (PlayerController.global.pausedBool || PlayerController.global.mapBool)
        {
            return;
        }

        if (!isBuilt)
        {
            bool open = Vector3.Distance(transform.position, PlayerController.global.transform.position) < 20;

            if (triggered != open)
            {
                triggered = open;
                ShowPrompt(open);

            }

            if ((debugforcebuild || canBuild) && triggered && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL))
            {
                debugforcebuild = false;
                PlayerController.global.interactCTRL = false;
                PlayerController.global.evading = false;

                if (PlayerController.global.CheckSufficientResources(true))
                {
                    canBuild = false;
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltNoiseSound);
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltSound);
                    isBuilt = true;
                    ShowPrompt(false);

                    GameManager.PlayAnimator(bridgeAnimator, "Armature_BridgeSelfBuild");
                    WalkAccrossCollider.SetActive(true);

                    if (BridgeTypeInt == 1)
                    {
                        LevelManager.global.goblinSpawnable = true;
                    }
                    if (BridgeTypeInt == 2)
                    {
                        LevelManager.global.snakeSpawnable = true;
                    }
                    if (BridgeTypeInt == 3)
                    {
                        LevelManager.global.wolfSpawnable = true;
                    }
                    if (BridgeTypeInt == 4)
                    {
                        LevelManager.global.spiderSpawnable = true;
                    }
                    if (BridgeTypeInt == 5)
                    {
                        LevelManager.global.lavaSpawnable = true;
                    }
                }
                else
                {
                    PlayerController.global.ShakeResourceHolder();
                }

            }

        }
    }
}
