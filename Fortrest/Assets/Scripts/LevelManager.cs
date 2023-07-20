using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.VFX;
using TMPro;

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

    Vector3 lastPanPosition;
    bool OnceDetection;

    // public List<Transform> EnemyList = new List<Transform>();
    public List<Transform> BuildingList = new List<Transform>();
    public Transform ResourceHolderTransform;
    public GameObject ActiveBuildingGameObject;
    public Transform DirectionalLightTransform;
    public Material LanternGlowingMaterial;
    public Material LanternOffMaterial;
    SkinnedMeshRenderer LanternSkinnedRenderer;
    private GameObject NightLightGameObject;
    public float DaylightTimer;
    public int day = 0;
    public List<GameObject> enemyList = new List<GameObject>();

    public float daySpeed = 1;
    public float GoblinTimer;
    float GoblinThreshold;

    public GameObject GoblinGameObject;
    public GameObject OgreGameObject;
    public GameObject SpiderGameObject;
    public List<Building> NaturalBuildingList = new List<Building>();
    private float gatherCooldown = 0.75f;
    private float nextGather;

    public VisualEffect VFXSparks;
    public VisualEffect VFXPebble;
    public VisualEffect VFXSmokePuff;
    public VisualEffect VFXWoodChip;

    [HideInInspector]
    public bool newDay = false;
    bool NightTimeMusic;
    public Gradient textGradient;

    public List<Transform> TerrainList = new List<Transform>();

    private enum SPAWNDIRECTION
    {
        North = 1,
        South,
        West,
        East
    };

    private SPAWNDIRECTION spawnDir;
    private float direction;
    private bool directionEstablished = false;
    private Transform houseTransform;
    private Vector3 enemySpawnPosition;
    bool housePosObtained = false;
    private float spawnDistance = 39.0f;

    private void Awake()
    {
        global = this;
        DaylightTimer = DirectionalLightTransform.eulerAngles.x;

        if (!GameManager.global)
        {
            // if()
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
        VFXSparks.Stop();
        VFXPebble.Stop();
        VFXSmokePuff.Stop();
        VFXWoodChip.Stop();
        newDay = true;

        PlayerController playerController = PlayerController.global;

        if (SpawnPosition)
        {
            playerController.transform.position = SpawnPosition.position;
        }

        LanternSkinnedRenderer = playerController.transform.Find("Dwarf_main_chracter_Updated").Find("Dwarf_Player_character_updated").GetComponent<SkinnedMeshRenderer>();
        NightLightGameObject = playerController.transform.Find("Spot Light").gameObject;

        /*
        DayTMP_Text = PlayerController.global..GetComponent<TMP_Text>();
        RemaningTMP_Text = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("New Day").Find("Remaining Text").GetComponent<TMP_Text>();
        SurvivedTMP_Text = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("Game Over").Find("Remaining Text").GetComponent<TMP_Text>();
        enemyNumberText = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("EnemiesText").GetComponent<TMP_Text>();
        enemyNumberText2 = GameObject.Find("Player_Holder").transform.Find("Player Canvas").Find("EnemyAmount").GetComponent<TMP_Text>();
        */       
    }

    private void GetHousePosition()
    {
        for (int i = 0; i < BuildingList.Count; i++)
        {
            if (BuildingList[i].GetComponent<Building>().resourceObject == Building.BuildingType.HouseNode)
            {
                houseTransform = BuildingList[i].transform.parent.transform;
                housePosObtained = true;
                break;
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
        /*
        if (TerrainList != null)
        {
            for (int i = 0; i < TerrainList.Count; i++)
            {
                if (TerrainList[i])
                {
                    bool enableBool = Vector3.Distance(TerrainList[i].position, PlayerController.global.transform.position) < 250;
                    // Debug.Log(TerrainList[i] + " + " + Vector3.Distance(TerrainList[i].position, PlayerController.global.transform.position) + "  < " + 450);
                    TerrainList[i].gameObject.SetActive(enableBool);
                }
            }
        }
        */

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

        daySpeed = 15.0f; // FOR TESTING
        //daySpeed = ReturnNight() ? 2 : 1;       

        DirectionalLightTransform.Rotate(new Vector3(1, 0, 0), daySpeed * Time.deltaTime);


        DaylightTimer += daySpeed * Time.deltaTime;
        GoblinTimer += Time.deltaTime;

        if (DaylightTimer > 360)
        {
            directionEstablished = false;
            DaylightTimer = 0;
            day++;
            GameManager.PlayAnimation(PlayerController.global.UIAnimation, "New Day");
            GameManager.global.SoundManager.PlaySound(GameManager.global.NewDaySound);
            PlayerController.global.NewDay();
        }

        //  Light light = DirectionalLightTransform.GetComponent<Light>();

        //   light.intensity = Mathf.Lerp(light.intensity, ReturnNight() ? 0 : 0.4f, Time.deltaTime);

        if (ReturnNight())
        {          
            if (!directionEstablished)
            {
                //direction = Random.Range(1, 4);
                direction = 4;
                switch (direction)
                {
                    case 1:
                        spawnDir = SPAWNDIRECTION.North;
                        break;
                    case 2:
                        spawnDir = SPAWNDIRECTION.South;
                        break;
                    case 3:
                        spawnDir = SPAWNDIRECTION.West;
                        break;
                    case 4:
                        spawnDir = SPAWNDIRECTION.East;
                        break;
                    default:
                        break;
                }               

                enemySpawnPosition = houseTransform.position;

                switch (spawnDir)
                {
                    case SPAWNDIRECTION.North:
                        enemySpawnPosition += new Vector3(spawnDistance, 0.0f, spawnDistance);
                        break;
                    case SPAWNDIRECTION.South:
                        enemySpawnPosition += new Vector3(-spawnDistance, 0.0f, -spawnDistance);
                        break;
                    case SPAWNDIRECTION.West:
                        enemySpawnPosition += new Vector3(-spawnDistance, 0.0f, spawnDistance);
                        break;
                    case SPAWNDIRECTION.East:
                        enemySpawnPosition += new Vector3(spawnDistance, 0.0f, -spawnDistance);
                        break;
                    default:
                        break;
                }

                directionEstablished = true;
            }
            
            if (GoblinTimer >= GoblinThreshold)
            {
                GoblinThreshold = 1.0f; // FOR TESTING
                //GoblinThreshold = Random.Range(15, 20) - (day * 2.5f);
                if (GoblinThreshold < 0.5f)
                {
                    GoblinThreshold = 0.5f;
                }
                GoblinTimer = 0;

                enemySpawnPosition.x += Random.Range(1, 5) * (Random.Range(0, 2) == 0 ? -1 : 1);
                enemySpawnPosition.z += Random.Range(1, 5) * (Random.Range(0, 2) == 0 ? -1 : 1);                

                GameObject prefab = GoblinGameObject;

                enemySpawnPosition.y = 0.0f;

                enemySpawnPosition.y = Terrain.activeTerrain.SampleHeight(enemySpawnPosition) - 16.0f; // 16 is the magic number for this to work          

                if (day > 1 && Random.Range(0, 3) == 0)
                {
                    prefab = SpiderGameObject;
                }

                if (day > 3 && Random.Range(0, 7) == 0)
                {
                    prefab = OgreGameObject;
                }

                GameObject enemy = Instantiate(prefab, enemySpawnPosition, Quaternion.identity);

                enemyList.Add(enemy);
            }
        }

        if (NightLightGameObject != null)
        {
            NightLightGameObject.SetActive(ReturnNight());
        }

        //   Debug.Log(LanternSkinnedRenderer.materials[2] + " " + (LanternSkinnedRenderer.materials[2] == (ReturnNight() ? LanternGlowingMaterial : LanternOffMaterial)));

        Material[] mats = LanternSkinnedRenderer.materials;

        mats[2] = ReturnNight() ? LanternGlowingMaterial : LanternOffMaterial;

        LanternSkinnedRenderer.materials = mats;

        for (int i = 0; i < NaturalBuildingList.Count; i++)
        {
            if (NaturalBuildingList[i])
            {
                float minDistanceFloat = 4;

                float distanceFloat = Vector3.Distance(PlayerController.global.transform.position, NaturalBuildingList[i].transform.position);
                if (distanceFloat < minDistanceFloat && Input.GetMouseButton(0) && PlayerModeHandler.global.playerModes == PlayerModes.ResourceMode && Time.time > nextGather)
                {
                    bool isStoneBool = NaturalBuildingList[i].resourceObject == Building.BuildingType.Stone;
                    PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = !isStoneBool, PicaxeBool = isStoneBool });
                    nextGather = Time.time + gatherCooldown;

                    if (NaturalBuildingList[i].health > 1)
                    {
                        if (isStoneBool)
                        {
                            VFXSparks.transform.position = PlayerController.global.PicaxeGameObject.transform.position;
                            VFXSparks.Play();
                            VFXPebble.transform.position = PlayerController.global.PicaxeGameObject.transform.position;
                            VFXPebble.Play();
                            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.Pickaxe2Sound : GameManager.global.Pickaxe3Sound);
                        }
                        else if (NaturalBuildingList[i].resourceObject == Building.BuildingType.Wood)
                        {
                            VFXWoodChip.transform.position = PlayerController.global.AxeGameObject.transform.position;
                            VFXWoodChip.Play();
                            GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.TreeChop1Sound : GameManager.global.TreeChop2Sound);
                        }
                        else if (NaturalBuildingList[i].resourceObject == Building.BuildingType.Food)
                        {
                            GameManager.global.SoundManager.PlaySound(GameManager.global.BushSound);
                        }

                        NaturalBuildingList[i].TakeDamage(1);
                    }
                    else
                    {
                        NaturalBuildingList[i].GiveResources();
                        NaturalBuildingList[i].DestroyBuilding();
                    }

                    NaturalBuildingList[i].healthBarImage.fillAmount = Mathf.Clamp(NaturalBuildingList[i].health / NaturalBuildingList[i].maxHealth, 0, 1f);

                    PlayerController.global.ApplyEnergyDamage(NaturalBuildingList[i].energyConsumptionPerClick, false);
                }
            }
        }

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
            if (Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                // locked = true;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                //   locked = true;
            }
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
}
