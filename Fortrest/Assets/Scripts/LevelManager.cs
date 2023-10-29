using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.Collections;
public class LevelManager : MonoBehaviour
{
    // VFXs
    public VisualEffect VFXSlash;
    public VisualEffect VFXSlashReversed;
    public VisualEffect VFXSleeping;
    public VisualEffect VFXSparks;
    public VisualEffect VFXPebble;
    public VisualEffect VFXWoodChip;
    public VisualEffect VFXSmokeRing;
    public VisualEffect VFXBuilding;
    public VisualEffect VFXSmoke;
    public VisualEffect VFXBossSlash;
    public VisualEffect VFXBossSlashReversed;

    public static LevelManager global;

    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject objectToSpawn;
        public float spawnPercentage;
    }


    [Header("Enemies to spawn by chance")]
    public List<SpawnEntry> spawnEntries = new List<SpawnEntry>();

    public SpawnEntry goblin;
    public SpawnEntry snake;
    public SpawnEntry wolf;
    public SpawnEntry spider;
    public SpawnEntry lava;
    [Space]

    public Camera SceneCamera;
    public GameObject PlayerPrefab;
    public GameObject mountPrefab;
    public float PanSpeed = 20f;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = 0.5f;

    public float[] BoundsX = new float[] { -10f, 5f };
    public float[] BoundsZ = new float[] { -18f, -4f };
    public float[] ZoomBounds = new float[] { 10f, 85f };

    public List<TierData> WoodTierList = new List<TierData>();
    public List<TierData> StoneTierList = new List<TierData>();

    public Gradient SunriseGradient;
    public Gradient SkyboxGradient;
    public Gradient AmbientGradient;

    [System.Serializable]
    public class TierData
    {
        public string ResourceName = "Wood";
        public Sprite ResourceIcon;
        public int ResourceAmount;
        public int ResourceCost;

        public bool SufficientResource()
        {
            return ResourceAmount + ResourceCost >= 0;
        }

    }

    Vector3 lastPanPosition;
    bool OnceDetection;


    public Transform ResourceHolderTransform;
    public GameObject ActiveBuildingGameObject;
    public Transform DirectionalLightTransform;
    public Material LanternGlowingMaterial;
    public Material LanternOffMaterial;

    public float daylightTimer;
    public int day = 0;

    [Header("Lists below should be left alone, the game will fill them automatically")]
    public List<EnemyController> enemyList = new List<EnemyController>();
    public List<GameObject> ItemDropList = new List<GameObject>();
    public List<BridgeBuilder> bridgeList = new List<BridgeBuilder>();
    public List<BossSpawner> bossList = new List<BossSpawner>();
    public List<Chest> chestList = new List<Chest>();
    public List<Transform> BuildingList = new List<Transform>();
    public List<Camp> campList = new List<Camp>();

    public float daySpeed = 1;
    public float enemyTimer;

    [HideInInspector]
    public float enemyThreshold;
    [Header("Ogre Prefab")]
    [Space]
    public GameObject ogrePrefab;
    [Space]
    public GameObject HUD;

    [HideInInspector]
    public bool newDay = false;
    public Gradient textGradient;
    [HideInInspector]
    public TerrainData currentTerrainData;
    public List<TerrainData> terrainDataList = new List<TerrainData>();
    public Image clockHand;
    public Image clockSun;
    public Image clockMoon;
    public bool waveEnd;

    [System.Serializable]
    public class TerrainData
    {
        public Terrain terrain;
        public Sprite welcomeSprite;
        public AudioClip music;
        public Material rabbitMaterial;

        public string indictorName;
        public Color indicatorColor;
    }

    public enum SPAWNLANE
    {
        Left = 1,
        Right,
        Middle,
    };

    public SPAWNLANE lane;

    public int campsCount;
    public int enemiesCount;
    public int ogreDayCount = 3;
    private int ogreSpawnCounterMax = 1;
    private int ogresSpawnCounter = 0;
    public bool spawnEnemies;
    private bool nightAttack;
    public float randomAttackTrigger;
    private bool countSet;
    private bool ogreSpawned;
    public bool dayPaused;
    private int groupSpawnAmount;
    private int laneInt;
    public Transform houseTransform;
    private Vector3 enemySpawnPosition;
    bool housePosObtained = false;
    private float spawnDistance = 39.0f;
    AnimationState enemyIncomingState;
    [HideInInspector]
    public bool messageDisplayed;

    [Header("Enemies Spawnable")]
    [Space]
    public bool goblinSpawnable;
    public bool snakeSpawnable;
    public bool wolfSpawnable;
    public bool spiderSpawnable;
    public bool lavaSpawnable;

    public BossSpawner activeBossSpawner;
    public List<Transform> cutsceneCameraLocations = new List<Transform>();

    private void Awake()
    {
        global = this;
        currentTerrainData = terrainDataList[0];
        clockHand.transform.rotation = Quaternion.Euler(clockHand.transform.rotation.eulerAngles.x, clockHand.transform.rotation.eulerAngles.y, -daylightTimer + 90);

        if (!GameManager.global)
        {
            PlayerPrefs.SetInt("Quick Load", SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(0);
        }

    }

    private void Start()
    {
        RenderSettings.skybox = new Material(RenderSettings.skybox); //stops it changing in git

        newDay = true;
        VFXSlash.Stop();
        VFXSlashReversed.Stop();
        VFXSleeping.Stop();
        VFXSparks.Stop();
        VFXPebble.Stop();
        VFXWoodChip.Stop();
        VFXSmokeRing.Stop();
        VFXBuilding.Stop();
        VFXSmoke.Stop();

        //LanternSkinnedRenderer = playerController.transform.Find("Dwarf_main_chracter_Updated").Find("Dwarf_Player_character_updated").GetComponent<SkinnedMeshRenderer>();
        //NightLightGameObject = playerController.transform.Find("Spot Light").gameObject;
        //terrainList = GameObject.FindObjectsOfType<Terrain>().ToList();

        /*
        DayTMP_Text = PlayerController.global..GetComponent<TMP_Text>();
        RemaningTMP_Text = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("New Day").Find("Remaining Text").GetComponent<TMP_Text>();
        SurvivedTMP_Text = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("Game Over").Find("Remaining Text").GetComponent<TMP_Text>();
        enemyNumberText = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("EnemiesText").GetComponent<TMP_Text>();
        enemyNumberText2 = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("EnemyAmount").GetComponent<TMP_Text>();
        */

        enemyThreshold = 0.0f;

        if (PlayerPrefs.GetInt("Prologue") == 0)
        {
            StartCoroutine(PrologueCutscene());
        }
    }

    IEnumerator PrologueCutscene()
    {
        PlayerController.global.gameObject.SetActive(false);
        PlayerController.global.UIAnimation.gameObject.SetActive(false);
        Indicator.global.GetComponent<Canvas>().enabled = false;

        SceneCamera.orthographic = false;
        SceneCamera.fieldOfView = 60;

        CameraFollow.global.bossCam = true;
        SFXManager.SFXData sFXData = GameManager.global.SoundManager.PlaySound(GameManager.global.CutsceneChattingOutside, 1, false);

        CutsceneSet(SceneCamera.transform, 0);

        while (Vector3.Distance(SceneCamera.transform.position, cutsceneCameraLocations[1].position) > 0.5f)
        {
            SceneCamera.transform.position = Vector3.Slerp(SceneCamera.transform.position, cutsceneCameraLocations[1].position, 0.6f * Time.deltaTime);
            yield return 0;
        }

        float timer = 0;

        yield return new WaitForSeconds(2);

        ScreenShake.global.ShakeScreen(0.5f);

        GameManager.global.SoundManager.StopSelectedSound(sFXData.Audio.clip);

        yield return new WaitForSeconds(1);

        while (timer < 2)
        {
            SceneCamera.transform.rotation = Quaternion.Slerp(SceneCamera.transform.rotation, cutsceneCameraLocations[2].transform.rotation, 0.5f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return 0;
        }

        ScreenShake.global.ShakeScreen(1);


        for (int i = 0; i < bossList.Count; i++)
        {
            if (bossList[i].bossType == BossSpawner.TYPE.IsleMaker)
            {
                CutsceneSet(bossList[i].transform, 3);

                while (Vector3.Distance(bossList[i].transform.position, cutsceneCameraLocations[4].position) > 1)
                {
                    activeBossSpawner = bossList[i]; //so the music changes
                    bossList[i].transform.position = Vector3.Slerp(bossList[i].transform.position, cutsceneCameraLocations[4].position, 2 * Time.deltaTime);
                    yield return 0;
                }

                break;
            }
        }
        GameManager.global.SoundManager.PlaySound(GameManager.global.BossRoarSound, 1, false);

        yield return new WaitForSeconds(1);

        for (int i = 0; i < bossList.Count; i++)
        {
            if (bossList[i].gameObject.activeInHierarchy && bossList[i].bossType != BossSpawner.TYPE.IsleMaker)
            {
                if (bossList[i].GetComponent<UnityEngine.AI.NavMeshAgent>())
                    bossList[i].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                bossList[i].cutsceneMode = true;
                timer = 0;
                if (bossList[i].bossType == BossSpawner.TYPE.Hrafn)
                {
                    bossList[i].GetComponent<BirdBoss>().enabled = false;
                }

                for (int j = 0; j < cutsceneCameraLocations.Count; j++)
                {
                    if (cutsceneCameraLocations[j].name.Contains(bossList[i].bossType.ToString()))
                    {
                        CutsceneSet(bossList[i].transform, j);
                        bossList[i].transform.position -= new Vector3(0, 3, 0);

                        while (timer < 2)
                        {
                            bossList[i].transform.position = Vector3.Slerp(bossList[i].transform.position, cutsceneCameraLocations[j].position, 2 * Time.deltaTime);
                            timer += Time.deltaTime;
                            yield return 0;
                        }

                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.5f);

        PlayerPrefs.SetInt("Prologue", 1);
        GameManager.global.NextScene(1);
    }

    void CutsceneSet(Transform move, int number)
    {
        move.position = cutsceneCameraLocations[number].position;
        move.rotation = cutsceneCameraLocations[number].rotation;
    }

    private void GetHousePosition()
    {
        ProcessBuildingList((building) =>
        {
            if (building.GetComponent<Building>().buildingObject == Building.BuildingType.HouseNode)
            {
                houseTransform = building.parent.transform;
                housePosObtained = true;
                return;
            }
        });
    }


    public void AddBuildingVoid(Transform addTransform)
    {
        BuildingList.Add(addTransform);
    }

    public void RemoveBuildingVoid(Transform removeTransform)
    {
        BuildingList.Remove(removeTransform);
    }

    public int ReturnIndex(Transform requestedTransform)
    {
        return BuildingList.IndexOf(requestedTransform);
    }

    public static void ProcessBossList(System.Action<BossSpawner> processAction)
    {
        for (int i = 0; i < LevelManager.global.bossList.Count; i++)
        {
            if (LevelManager.global.bossList[i])
            {
                processAction(LevelManager.global.bossList[i]);
            }
        }
    }

    public static void ProcessCampList(System.Action<Camp> processAction)
    {
        for (int i = 0; i < LevelManager.global.campList.Count; i++)
        {
            if (LevelManager.global.campList[i])
            {
                processAction(LevelManager.global.campList[i]);
            }
        }
    }

    public static void ProcessEnemyList(System.Action<EnemyController> processAction)
    {
        for (int i = 0; i < LevelManager.global.enemyList.Count; i++)
        {
            if (LevelManager.global.enemyList[i])
            {
                processAction(LevelManager.global.enemyList[i]);
            }
        }
    }

    public static void ProcessBuildingList(System.Action<Transform> processAction, bool naturalBool = false)
    {
        for (int i = 0; i < LevelManager.global.BuildingList.Count; i++)
        {
            if (LevelManager.global.BuildingList[i])
            {
                Building building = global.BuildingList[i].GetComponent<Building>();

                if (building)
                {
                    if (building.NaturalBool == naturalBool)
                        processAction(LevelManager.global.BuildingList[i]);
                }
                else
                {
                    Debug.LogWarning("No building script found!");
                }
            }
            else
            {
                Debug.LogWarning("CANNOT BE NULL. Dont destory buildings because then cant be saved!");
            }
        }
    }

    public static void FloatingTextChange(GameObject floatingText, bool enable)
    {
        floatingText.gameObject.SetActive(true);

        GameManager.PlayAnimation(floatingText.GetComponent<Animation>(), "BobbingText Appear", enable);
    }

    public bool ReturnNight()
    {
        return daylightTimer > 180;
    }

    AudioClip activeMusic;
    public void SetGameMusic()
    {
        TerrainData terrainData = currentTerrainData;

        Physics.Raycast(PlayerController.global.transform.position, -Vector3.up, out RaycastHit raycastHit, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Terrain" }));

        for (int i = 0; i < terrainDataList.Count; i++)
        {
            if (terrainDataList[i].terrain.transform == raycastHit.transform)
            {
                terrainData = terrainDataList[i];
                break;
            }
        }

        AudioClip music = terrainData.music;

        if (activeBossSpawner)
            music = GameManager.global.BossMusic;

        if (ReturnNight())
            music = GameManager.global.NightMusic;

        if (Time.timeScale == 0)
            music = GameManager.global.PauseMusic;


        if (activeMusic != music)
        {
            activeMusic = music;

            GameManager.global.MusicManager.PlayMusic(music);
        }

        if (currentTerrainData.welcomeSprite != terrainData.welcomeSprite)
        {
            PlayerController.global.biomeNameImage.color = terrainData.indicatorColor;
            PlayerController.global.biomeNameImage.sprite = terrainData.welcomeSprite;
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Biome Name Appear");
            //GameManager.global.SoundManager.PlaySound(GameManager.global.NewDaySound);
        }


        currentTerrainData = terrainData;

    }

    private void CalculateCamps()
    {
        campsCount = 0;
        ProcessCampList((camp) =>
        {
            campsCount++;
        });
    }

    private void Update()
    {
        if (!housePosObtained)
        {
            GetHousePosition();
        }

        if (ReturnNight())
        {
            clockMoon.enabled = true;
            clockSun.enabled = false;
        }
        else
        {
            clockMoon.enabled = false;
            clockSun.enabled = true;
        }

        PlayerController.global.EnemiesTextControl();

        SetGameMusic();

        if (dayPaused)
        {
            daySpeed = 0;
        }
        else
        {
            daySpeed = ReturnNight() ? 2 : 1;
        }


#if UNITY_EDITOR
        // daySpeed = 7.0f; // FOR TESTING
#endif
        //  DirectionalLightTransform.Rotate(new Vector3(1, 0, 0), daySpeed * Time.deltaTime);
        DirectionalLightTransform.eulerAngles = new Vector3(45, 0, 0);
        //        DirectionalLightTransform.eulerAngles = new Vector3(daylightTimer, 0, 0);
        clockHand.transform.rotation = Quaternion.Euler(clockHand.transform.rotation.eulerAngles.x, clockHand.transform.rotation.eulerAngles.y, -daylightTimer + 90);
        daylightTimer += daySpeed * Time.deltaTime;

        Light light = DirectionalLightTransform.GetComponent<Light>();
        int cycle = 360;

        float evaluate = daylightTimer / cycle;

        light.color = SunriseGradient.Evaluate(evaluate);
        SceneCamera.backgroundColor = SkyboxGradient.Evaluate(evaluate);
        RenderSettings.ambientLight = AmbientGradient.Evaluate(evaluate);
        RenderSettings.skybox.SetColor("_Tint", SkyboxGradient.Evaluate(evaluate));

        if (daylightTimer > cycle)
        {
            randomAttackTrigger = 0f;
            messageDisplayed = false;
            daylightTimer = 0;
            day++;
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "New Day");
            GameManager.global.SoundManager.PlaySound(GameManager.global.NewDaySound);
            PlayerController.global.NewDay();
            GameManager.global.DataSetVoid(false);
        }

        if (Unlocks.global.mountUnlocked && !Boar.global)
        {
            Instantiate(mountPrefab, houseTransform.position - houseTransform.forward * 5 - houseTransform.right * 5, Quaternion.identity);
        }

        CalculateCamps();

        EnemyWaves();

        //  Light light = DirectionalLightTransform.GetComponent<Light>();

        light.intensity = Mathf.Lerp(light.intensity, 1 - (ReturnNight() ? 0 : Weather.global.DecreaseDayLightIntensity), 0.4f * Time.deltaTime);



        if (PlayerController.global.LanternLighted != ReturnNight())
        {
            PlayerController.global.LanternLighted = ReturnNight();
            GameManager.PlayAnimation(PlayerController.global.GetComponent<Animation>(), "Lantern Light", PlayerController.global.LanternLighted);
        }

        //   Debug.Log(LanternSkinnedRenderer.materials[2] + " " + (LanternSkinnedRenderer.materials[2] == (ReturnNight() ? LanternGlowingMaterial : LanternOffMaterial)));

        Material[] mats = PlayerController.global.LanternSkinnedRenderer.materials;

        mats[2] = ReturnNight() ? LanternGlowingMaterial : LanternOffMaterial;

        PlayerController.global.LanternSkinnedRenderer.materials = mats;

        if (!PlayerModeHandler.global.inTheFortress && PlayerController.global.transform.position.y < -3 && PlayerController.global.playerHealth > 0)
        {
            //   GameManager.global.SoundManager.PlaySound(GameManager.global.WaterSound);
            // GameManager.global.NextScene(1);
            if (Boar.global.mounted)
            {
                Boar.global.Mount();
                Boar.global.transform.position = Boar.global.respawnLocation;
            }
            PlayerController.global.TakeDamage(PlayerController.global.playerHealth);
            // enabled = false;
            // return;
        }


        //if (Physics.Raycast(SceneCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        //{
        //    Debug.Log(hit.transform);
        //    if (hit.transform.GetComponent<Building>())
        //    {
        //        hit.transform.GetComponent<Building>().MouseOverVoid();
        //    }
        //}


        if (ActiveBuildingGameObject)
            return;

        //HandleMouse();
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera

        Vector3 offset = SceneCamera.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);

        if (!OnceDetection)
        {
            OnceDetection = true;
            float MaxMovement = 0.5f;
            float DefaultMovement = 0.1f;
            if (move.x > MaxMovement)
            {
                move.x = DefaultMovement;
            }
            if (move.z > MaxMovement)
            {
                move.z = DefaultMovement;
            }

            if (move.x < -MaxMovement)
            {
                move.x = -DefaultMovement;
            }
            if (move.z < -MaxMovement)
            {
                move.z = -DefaultMovement;
            }
        }

        SceneCamera.transform.Translate(move, Space.World);

        //Debug.Log("POS: " + move + " |  SceneCamera.transform: " +  SceneCamera.transform.position);

        // Ensure the camera remains within bounds.
        Vector3 pos = SceneCamera.transform.position;
        pos.x = Mathf.Clamp(SceneCamera.transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(SceneCamera.transform.position.z, BoundsZ[0], BoundsZ[1]);

        SceneCamera.transform.position = pos;
        // Cache the position
        lastPanPosition = newPanPosition;
    }

    private GameObject SpawnRandomEnemy()
    {
        float totalPercentage = 0f;

        foreach (SpawnEntry entry in spawnEntries)
        {
            totalPercentage += entry.spawnPercentage;
        }

        float randomValue = Random.Range(0f, totalPercentage);
        float cumulativePercentage = 0f;

        foreach (SpawnEntry entry in spawnEntries)
        {
            cumulativePercentage += entry.spawnPercentage;
            if (randomValue <= cumulativePercentage)
            {
                return entry.objectToSpawn;
            }
        }

        return null;
    }

    private void EnemyWaves()
    {
        //// Day Attack
        //if (day > 0 && !ReturnNight() && !randomSet)
        //{
        //    float randomChance = Random.Range(0.0f, 1.0f);

        //    switch (campsCount)
        //    {
        //        case 0: // No camps = no day attack
        //            attackHappening = false;
        //            break;
        //        case 1: // 1 camp = 20% chance
        //            if (randomChance > 0.8f)
        //            {
        //                attackHappening = true;
        //            }
        //            break;
        //        case 2: // 2 camps = 40% chance
        //            if (randomChance > 0.6f)
        //            {
        //                attackHappening = true;
        //            }
        //            break;
        //        case 3: // 3 camps = 60% chance
        //            if (randomChance > 0.4f)
        //            {
        //                attackHappening = true;
        //            }
        //            break;
        //        case 4: // 4 camps = 80% chance
        //            if (randomChance > 0.2f)
        //            {
        //                attackHappening = true;
        //            }
        //            break;
        //        default:
        //            break;
        //    }

        //    if (campsCount >= 5) // 5+ camps = 100% chance
        //    {
        //        attackHappening = true;
        //    }

        //    if (attackHappening)
        //    {
        //        randomAttackTrigger = Random.Range(60.0f, 120.0f); // Attack starts at a random time during the day
        //        nightAttack = false; // It is not a night attack
        //    }
        //    randomSet = true; // Regardless of the outcome, we are not running this again until the next day                     
        //}

        // Night attack
        if (daylightTimer >= 150.0f && daylightTimer <= 151.0f)
        {
            randomAttackTrigger = 180.0f;
            nightAttack = true;
        }

        // Message and countdown bar appear 30f before the attack

        float enemiesIncoming = randomAttackTrigger - PlayerController.global.UIAnimation["Enemies Incoming"].length;

        //Debug.Log(daylightTimer + " >= " + noon);

        if (daylightTimer >= enemiesIncoming && randomAttackTrigger != 0f && !messageDisplayed)
        {
            enemyIncomingState = GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Enemies Incoming"); // Display enemies are coming a bit before an attack
            messageDisplayed = true;
            GameManager.global.SoundManager.PlaySound(GameManager.global.ClockSound);
        }


        // Enemies start spawning after enemy incoming animation is finished
        if (!spawnEnemies && messageDisplayed && enemyIncomingState && !enemyIncomingState.enabled)
        {
            enemyIncomingState = null;
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "Enemies Appear");
            spawnEnemies = true; // Attack starts when the time is reached
            countSet = false;
        }

        if (spawnEnemies)
        {
            // Set the amount of enemies at the start of the attack
            if (!countSet)
            {
                if (nightAttack)
                {
                    enemiesCount += 10 * (day + 1) + (campsCount * 3);
                }
                else
                {
                    enemiesCount += 6 * (day + 1) + (campsCount * 2);
                }
                countSet = true;
            }

            // North position
            enemySpawnPosition = houseTransform.position + new Vector3(spawnDistance, 0.0f, spawnDistance);

            enemyTimer += Time.deltaTime;

            // Spawn delay for enemies. Happens till the count reaches 0
            if (enemyTimer >= enemyThreshold && enemiesCount > 0)
            {
                // Random position out of 3
                laneInt = Random.Range(1, 4);
                switch (laneInt)
                {
                    case 1:
                        lane = SPAWNLANE.Left;
                        enemySpawnPosition += new Vector3(-10.0f, 0.0f, 10.0f);
                        break;
                    case 2:
                        lane = SPAWNLANE.Middle;
                        break;
                    case 3:
                        lane = SPAWNLANE.Right;
                        enemySpawnPosition += new Vector3(10.0f, 0.0f, -10.0f);
                        break;
                    default:
                        break;
                }

                // Delay till the next enemy spawns
                enemyThreshold = Random.Range(5.0f, 7.5f) - (day * 1.0f);

                // Minimum delay
                if (enemyThreshold < 1.0f)
                {
                    enemyThreshold = 1.0f;
                }

                // Chance to spawn a group of enemies.
                int randomInt = Random.Range(0, 4 + groupSpawnAmount);

                // Group of enemies
                if (randomInt == 1 && enemiesCount > 3)
                {
                    int randomRange = Random.Range(2, 5);

                    for (int i = 0; i < randomRange; i++)
                    {
                        enemySpawnPosition.x += Random.Range(2, 6) * (Random.Range(0, 2) == 0 ? -1 : 1) + (i * Random.Range(0, 2) == 0 ? -2 : 2);
                        enemySpawnPosition.z += Random.Range(2, 6) * (Random.Range(0, 2) == 0 ? -1 : 1) + (i * Random.Range(0, 2) == 0 ? -2 : 2);

                        GameObject prefab = SpawnRandomEnemy();

                        if (day % ogreDayCount == 0 && Random.Range(0, 3) == 0 && !ogreSpawned)
                        {
                            prefab = ogrePrefab;

                            ogresSpawnCounter++;

                            if (ogresSpawnCounter == ogreSpawnCounterMax)
                            {
                                ogresSpawnCounter = 0;
                                ogreSpawnCounterMax++;
                                ogreSpawned = true;
                            }
                        }

                        enemySpawnPosition.y = 0; //everything is at ground zero                   

                        if (prefab != null)
                        {
                            GameObject enemy = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);
                        }
                    }
                    enemiesCount -= randomRange;
                    enemyTimer = 0;
                    groupSpawnAmount++;
                }
                // Single enemy
                else if (randomInt != 1)
                {
                    enemySpawnPosition.x += Random.Range(2, 6) * (Random.Range(0, 2) == 0 ? -1 : 1);
                    enemySpawnPosition.z += Random.Range(2, 6) * (Random.Range(0, 2) == 0 ? -1 : 1);
                    enemySpawnPosition.y = 0; //everything is at ground zero        

                    GameObject prefab = SpawnRandomEnemy();

                    if (day % ogreDayCount == 0 && Random.Range(0, 3) == 0 && !ogreSpawned)
                    {
                        prefab = ogrePrefab;

                        ogresSpawnCounter++;

                        if (ogresSpawnCounter == ogreSpawnCounterMax)
                        {
                            ogresSpawnCounter = 0;
                            ogreSpawnCounterMax++;
                            ogreSpawned = true;
                        }
                    }

                    if (prefab != null)
                    {
                        GameObject enemy = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);
                    }

                    enemiesCount--;
                    enemyTimer = 0;
                }
            }
            // Reset everything once enemies have spawned. Day attacks variable are also reset when a new day starts
            else if (enemiesCount <= 0)
            {
                waveEnd = true;
                spawnEnemies = false;

                groupSpawnAmount = 0;
                ogreSpawned = false;
                enemiesCount = 0;
            }
        }
    }
}