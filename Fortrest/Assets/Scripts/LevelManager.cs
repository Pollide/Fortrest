using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager global;
    public Transform SpawnPosition;
    public Camera SceneCamera;
    public GameObject PlayerPrefab;
    public float PanSpeed = 20f;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = 0.5f;

    public float[] BoundsX = new float[] { -10f, 5f };
    public float[] BoundsZ = new float[] { -18f, -4f };
    public float[] ZoomBounds = new float[] { 10f, 85f };

    public List<TierData> WoodTierList = new List<TierData>();
    public List<TierData> StoneTierList = new List<TierData>();

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

    // public List<Transform> EnemyList = new List<Transform>();
    private List<Transform> BuildingList = new List<Transform>(); //This list is private as now you use ProcessBuildingList((building) => . Do not reference this list any other way. dm to ask how to use

    public Transform ResourceHolderTransform;
    public GameObject ActiveBuildingGameObject;
    public Transform DirectionalLightTransform;
    public Material LanternGlowingMaterial;
    public Material LanternOffMaterial;

    public float DaylightTimer;
    public int day = 0;
    public List<EnemyController> EnemyList = new List<EnemyController>();
    public List<GameObject> InventoryItemList = new List<GameObject>();
    public List<BridgeBuilder> BridgeList = new List<BridgeBuilder>();
    public float daySpeed = 1;
    public float enemyTimer;

    [HideInInspector]
    public float enemyThreshold;

    public GameObject GoblinGameObject;
    public GameObject OgreGameObject;
    public GameObject SpiderGameObject;

    public VisualEffect VFXSmokePuff;

    [HideInInspector]
    public bool newDay = false;
    bool NightTimeMusic;
    public Gradient textGradient;

    public List<Transform> TerrainList = new List<Transform>();

    public Image clockHand;

    public enum SPAWNLANE
    {
        Left = 1,
        Right,
        Middle,
    };

    public SPAWNLANE lane;

    private int campsCount;
    public int enemiesCount;
    public bool spawnEnemies;
    private bool nightAttack;
    public float randomAttackTrigger;
    private bool randomSet;
    private bool countSet;
    private bool ogreSpawned;
    private bool attackHappening;
    private int groupSpawnAmount;
    private int laneInt;
    private Transform houseTransform;
    private Vector3 enemySpawnPosition;
    bool housePosObtained = false;
    private float spawnDistance = 39.0f;

    public bool messageDisplayed;
    private bool runOnce;

    private void Awake()
    {
        global = this;
        DaylightTimer = DirectionalLightTransform.eulerAngles.x;
        clockHand.transform.rotation = Quaternion.Euler(clockHand.transform.rotation.eulerAngles.x, clockHand.transform.rotation.eulerAngles.y, -DaylightTimer);

        if (!GameManager.global)
        {
            PlayerPrefs.SetInt("Quick Load", SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(0);
        }
        else if (!PlayerController.global && SceneManager.GetActiveScene().buildIndex != 1)
        {
            Instantiate(PlayerPrefab, SpawnPosition);
        }


    }

    public AudioClip ActiveBiomeMusic;

    private void Start()
    {
        ActiveBiomeMusic = GameManager.global.GameMusic;
        newDay = true;

        PlayerController playerController = PlayerController.global;

        if (SpawnPosition)
        {
            playerController.transform.position = SpawnPosition.position;
        }

        //LanternSkinnedRenderer = playerController.transform.Find("Dwarf_main_chracter_Updated").Find("Dwarf_Player_character_updated").GetComponent<SkinnedMeshRenderer>();
        //NightLightGameObject = playerController.transform.Find("Spot Light").gameObject;

        VFXSmokePuff.Stop();
        /*
        DayTMP_Text = PlayerController.global..GetComponent<TMP_Text>();
        RemaningTMP_Text = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("New Day").Find("Remaining Text").GetComponent<TMP_Text>();
        SurvivedTMP_Text = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("Game Over").Find("Remaining Text").GetComponent<TMP_Text>();
        enemyNumberText = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("EnemiesText").GetComponent<TMP_Text>();
        enemyNumberText2 = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("EnemyAmount").GetComponent<TMP_Text>();
        */

        enemyThreshold = 0.0f;
    }



    private void GetHousePosition()
    {
        ProcessBuildingList((building) =>
        {
            if (building.GetComponent<Building>().resourceObject == Building.BuildingType.HouseNode)
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

    public static void ProcessEnemyList(System.Action<EnemyController> processAction)
    {
        for (int i = 0; i < LevelManager.global.EnemyList.Count; i++)
        {
            if (LevelManager.global.EnemyList[i])
            {
                processAction(LevelManager.global.EnemyList[i]);
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
        return DaylightTimer > 180;
    }

    private void Update()
    {
        if (!housePosObtained)
        {
            GetHousePosition();
        }

        LockCursor();

        PlayerController.global.EnemiesTextControl();

        if (!NightTimeMusic && ReturnNight())
        {
            NightTimeMusic = true;
            GameManager.global.MusicManager.PlayMusic(GameManager.global.NightMusic);
        }

        if (NightTimeMusic && !ReturnNight())
        {
            NightTimeMusic = false;
            GameManager.global.MusicManager.PlayMusic(ActiveBiomeMusic);
        }

        daySpeed = ReturnNight() ? 2 : 1;

        //daySpeed = 7.0f; // FOR TESTING

        //  DirectionalLightTransform.Rotate(new Vector3(1, 0, 0), daySpeed * Time.deltaTime);
        DirectionalLightTransform.eulerAngles = new Vector3(DaylightTimer, 0, 0);
        clockHand.transform.rotation = Quaternion.Euler(clockHand.transform.rotation.eulerAngles.x, clockHand.transform.rotation.eulerAngles.y, -DaylightTimer);
        DaylightTimer += daySpeed * Time.deltaTime;

        if (DaylightTimer > 360)
        {
            attackHappening = false;
            randomAttackTrigger = 0f;
            randomSet = false;
            DaylightTimer = 0;
            day++;
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "New Day");
            GameManager.global.SoundManager.PlaySound(GameManager.global.NewDaySound);
            PlayerController.global.NewDay();
            GameManager.global.DataSetVoid(false);
        }

        EnemyWaves();

        //  Light light = DirectionalLightTransform.GetComponent<Light>();

        //   light.intensity = Mathf.Lerp(light.intensity, ReturnNight() ? 0 : 0.4f, Time.deltaTime);



        if (PlayerController.global.LanternLighted != ReturnNight())
        {
            PlayerController.global.LanternLighted = ReturnNight();
            GameManager.PlayAnimation(PlayerController.global.GetComponent<Animation>(), "Lantern Light", PlayerController.global.LanternLighted);
        }

        //   Debug.Log(LanternSkinnedRenderer.materials[2] + " " + (LanternSkinnedRenderer.materials[2] == (ReturnNight() ? LanternGlowingMaterial : LanternOffMaterial)));

        Material[] mats = PlayerController.global.LanternSkinnedRenderer.materials;

        mats[2] = ReturnNight() ? LanternGlowingMaterial : LanternOffMaterial;

        PlayerController.global.LanternSkinnedRenderer.materials = mats;

        if (PlayerController.global.transform.position.y < -3)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.WaterSound);
            GameManager.global.NextScene(1);
            enabled = false;
            return;
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


    void LockCursor()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            PlayerModeHandler.SetMouseActive(!Cursor.visible);
        }
    }

    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.Minus))
        {
            scroll = -0.1f;
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            scroll = 0.1f;
        }

        if (scroll != 0)
        {
            SceneCamera.orthographicSize = Mathf.Clamp(SceneCamera.orthographicSize - (scroll * ZoomSpeedMouse), ZoomBounds[0], ZoomBounds[1]);
        }

        PanSpeed = SceneCamera.orthographicSize / 2.5f;
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

    private void EnemyWaves()
    {
        // Day Attack
        if (day > 0 && !ReturnNight() && !randomSet)
        {
            float randomChance = Random.Range(0.0f, 1.0f);

            switch (campsCount)
            {
                case 0: // No camps = no day attack
                    attackHappening = false;
                    break;
                case 1: // 1 camp = 20% chance
                    if (randomChance > 0.8f)
                    {
                        attackHappening = true;
                    }
                    break;
                case 2: // 2 camps = 40% chance
                    if (randomChance > 0.6f)
                    {
                        attackHappening = true;
                    }
                    break;
                case 3: // 3 camps = 60% chance
                    if (randomChance > 0.4f)
                    {
                        attackHappening = true;
                    }
                    break;
                case 4: // 4 camps = 80% chance
                    if (randomChance > 0.2f)
                    {
                        attackHappening = true;
                    }
                    break;
                default:
                    break;
            }

            if (campsCount >= 5) // 5+ camps = 100% chance
            {
                attackHappening = true;
            }

            if (attackHappening)
            {
                randomAttackTrigger = Random.Range(60.0f, 120.0f); // Attack starts at a random time during the day
                nightAttack = false; // It is not a night attack
            }
            randomSet = true; // Regardless of the outcome, we are not running this again until the next day                     
        }

        // Night attack
        if (DaylightTimer >= 150.0f && DaylightTimer <= 151.0f)
        {
            randomAttackTrigger = 180.0f;
            nightAttack = true;
        }

        if ((DaylightTimer >= randomAttackTrigger - 30.0f && DaylightTimer <= randomAttackTrigger - 30.0f + 1.0f) && randomAttackTrigger != 0f && !messageDisplayed)
        {
            PlayerController.global.DisplayEnemiesComingText(); // Display enemies are coming a bit before an attack            
            messageDisplayed = true;
            runOnce = false;
        }

        if ((DaylightTimer >= randomAttackTrigger && DaylightTimer <= randomAttackTrigger + 1.0f) && randomAttackTrigger != 0f && !runOnce)
        {
            spawnEnemies = true; // Attack starts when the time is reached
            countSet = false;
            runOnce = true;
        }

        if (spawnEnemies)
        {
            // Set the amount of enemies at the start of the attack
            if (!countSet)
            {
                if (nightAttack)
                {
                    enemiesCount += 5 * (day + 1) + (campsCount * 3);
                }
                else
                {
                    enemiesCount += 3 * (day + 1) + (campsCount * 1);
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

                        GameObject prefab = GoblinGameObject;

                        if (day > 1 && Random.Range(0, 3) == 0)
                        {
                            prefab = SpiderGameObject;
                        }

                        if (day > 2 && Random.Range(0, 7) == 0 && !ogreSpawned)
                        {
                            prefab = OgreGameObject;
                            ogreSpawned = true;
                        }

                        enemySpawnPosition.y = Terrain.activeTerrain.SampleHeight(enemySpawnPosition) - 16.0f; // 16 is the magic number for this to work                         

                        GameObject enemy = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);
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

                    GameObject prefab = GoblinGameObject;

                    if (day > 1 && Random.Range(0, 3) == 0)
                    {
                        prefab = SpiderGameObject;
                    }

                    if (day > 2 && Random.Range(0, 7) == 0 && !ogreSpawned)
                    {
                        prefab = OgreGameObject;
                        ogreSpawned = true;
                    }

                    enemySpawnPosition.y = Terrain.activeTerrain.SampleHeight(enemySpawnPosition) - 16.0f; // 16 is the magic number for this to work                         

                    GameObject enemy = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);

                    enemiesCount--;
                    enemyTimer = 0;
                }
            }
            // Reset everything once enemies have spawned. Day attacks variable are also reset when a new day starts
            else if (enemiesCount <= 0)
            {
                spawnEnemies = false;
                groupSpawnAmount = 0;
                ogreSpawned = false;
                enemiesCount = 0;
            }
        }
    }
}