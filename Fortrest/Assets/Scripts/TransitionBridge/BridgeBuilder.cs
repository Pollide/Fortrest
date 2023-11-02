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
    CampSpawner spawner;
    LevelManager manager;

    public bool debugforcebuild;

    private void Start()
    {
        CheckIndicators();
        LevelManager.global.bridgeList.Add(this);
        spawner = CampSpawner.global;
        manager = LevelManager.global;
    }

    public void CheckIndicators()
    {
        canBuild = BridgeTypeInt == 1;

        //remember the spaced out text below is where the E key goes ingame
        // FloatingTextCanBuildAnimation.gameObject.SetActive(canBuild);
        // FloatingTextAnimation.gameObject.SetActive(!canBuild);

        FloatingTextAnimation.GetComponent<TMP_Text>().text = "Clear the " + LevelManager.global.terrainDataList[BridgeTypeInt - 1].indictorName + "\n to continue"; //-1 for previous

        FloatingTextAnimation.GetComponent<TMP_Text>().color = LevelManager.global.terrainDataList[BridgeTypeInt].indicatorColor;



        FloatingTextCanBuildAnimation.GetComponent<TMP_Text>().text = "Press   to build\nthe " + LevelManager.global.terrainDataList[BridgeTypeInt].indictorName + " bridge";
        FloatingTextCanBuildAnimation.GetComponent<TMP_Text>().color = LevelManager.global.terrainDataList[BridgeTypeInt].indicatorColor;



        for (int i = 0; i < LevelManager.global.bossList.Count; i++)
        {

            if (LevelManager.global.bossList[i].health <= 0)
            {
                if (BridgeTypeInt == 2 && LevelManager.global.bossList[i].bossType == BossSpawner.TYPE.Chieftain)
                {
                    canBuild = true;
                    break;
                }

                if (BridgeTypeInt == 3 && LevelManager.global.bossList[i].bossType == BossSpawner.TYPE.Hrafn)
                {
                    canBuild = true;
                    break;
                }

                if (BridgeTypeInt == 4 && (LevelManager.global.bossList[i].bossType == BossSpawner.TYPE.Lycan))
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

        if (canBuild)
        {
            for (int i = 0; i < Indicator.global.IndicatorList.Count; i++)
            {
                if (Indicator.global.IndicatorList[i].ActiveString == LevelManager.global.terrainDataList[BridgeTypeInt].indictorName)
                    return;
            }
            Indicator.global.AddIndicator(transform, LevelManager.global.terrainDataList[BridgeTypeInt].indicatorColor, LevelManager.global.terrainDataList[BridgeTypeInt].indictorName, false);
        }
    }

    void ShowPrompt(bool show)
    {
        PlayerController.global.needInteraction = show;
        PlayerController.global.bridgeInteract = show;
        CheckIndicators();

        PlayerController.global.needInteraction = show;
        PlayerController.global.bridgeInteract = show;

        LevelManager.FloatingTextChange(canBuild ? FloatingTextCanBuildAnimation.gameObject : FloatingTextAnimation.gameObject, show);

        PlayerController.global.UpdateResourceHolder(new PlayerController.ResourceData() { bridgeTypeInt = BridgeTypeInt }, open: show);
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

                if (PlayerController.global.CheckSufficientResources())
                {
                    canBuild = false;
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltNoiseSound);
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltSound);
                    isBuilt = true;
                    ShowPrompt(false);

                    GameManager.PlayAnimator(bridgeAnimator, "Armature_BridgeSelfBuild");
                    WalkAccrossCollider.SetActive(true);

                    spawner.spawnEnemies = false;

                    if (BridgeTypeInt == 1)
                    {
                        manager.goblinSpawnable = true;
                    }
                    if (BridgeTypeInt == 2)
                    {
                        manager.snakeSpawnable = true;
                        manager.spawnEntries.Add(manager.snake);
                    }
                    if (BridgeTypeInt == 3)
                    {
                        manager.wolfSpawnable = true;
                        manager.spawnEntries.Add(manager.wolf);
                    }
                    if (BridgeTypeInt == 4)
                    {
                        manager.spiderSpawnable = true;
                        manager.spawnEntries.Add(manager.spider);
                    }
                    if (BridgeTypeInt == 5)
                    {
                        manager.lavaSpawnable = true;
                        manager.spawnEntries.Add(manager.lava);
                    }
                }
            }
        }
        else
        {
            if (!spawner.spawnEnemies)
            {
                if (BridgeTypeInt == 1)
                {
                    spawner.SpawnEnemies(manager.terrainDataList[BridgeTypeInt].terrain, manager.goblin.objectToSpawn, spawner.spawnMaxMarsh, ref spawner.spawnCurrentMarsh);
                }
                if (BridgeTypeInt == 2)
                {
                    spawner.SpawnEnemies(manager.terrainDataList[BridgeTypeInt].terrain, manager.snake.objectToSpawn, spawner.spawnMaxTussuck, ref spawner.spawnCurrentTussuck);
                }
                if (BridgeTypeInt == 3)
                {
                    spawner.SpawnEnemies(manager.terrainDataList[BridgeTypeInt].terrain, manager.wolf.objectToSpawn, spawner.spawnMaxCoast, ref spawner.spawnCurrentCoast);
                }
                if (BridgeTypeInt == 4)
                {
                    spawner.SpawnEnemies(manager.terrainDataList[BridgeTypeInt].terrain, manager.spider.objectToSpawn, spawner.spawnMaxTaiga, ref spawner.spawnCurrentTaiga);
                }
                if (BridgeTypeInt == 5)
                {
                    spawner.SpawnEnemies(manager.terrainDataList[BridgeTypeInt].terrain, manager.lava.objectToSpawn, spawner.spawnMaxVolcanic, ref spawner.spawnCurrentVolcanic);
                }
            }

            if (manager.currentTerrainData == manager.terrainDataList[0])
            {
                spawner.spawnEnemies = false;
            }
            else
            {
                spawner.spawnEnemies = true;
            }
        }
    }
}
