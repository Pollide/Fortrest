using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        CheckIndicators();
        LevelManager.global.bridgeList.Add(this);
    }

    public void CheckIndicators()
    {
        bool before = canBuild;


        canBuild = BridgeTypeInt == 1;

        //remember the spaced out text below is where the E key goes ingame
        FloatingTextCanBuildAnimation.GetComponent<TMP_Text>().text = "Press   to build\nthe " + LevelManager.global.terrainDataList[BridgeTypeInt].indictorName + " bridge";
        FloatingTextCanBuildAnimation.GetComponent<TMP_Text>().color = LevelManager.global.terrainDataList[BridgeTypeInt].indicatorColor;

        FloatingTextAnimation.GetComponent<TMP_Text>().text = "Defeat the " + LevelManager.global.terrainDataList[BridgeTypeInt].indictorName + "\nboss to continue";
        FloatingTextAnimation.GetComponent<TMP_Text>().color = LevelManager.global.terrainDataList[BridgeTypeInt].indicatorColor;

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

        if (!before && canBuild)
        {
            Indicator.global.AddIndicator(transform, LevelManager.global.terrainDataList[BridgeTypeInt].indicatorColor, LevelManager.global.terrainDataList[BridgeTypeInt].indictorName, false, Indicator.global.BridgeSprite);
        }
    }

    void ShowPrompt(bool show)
    {
        PlayerController.global.needInteraction = show;
        PlayerController.global.bridgeInteract = show;
        CheckIndicators();

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
