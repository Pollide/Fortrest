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

    public Camera SceneCamera;

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

    public GameObject ActiveBuildingGameObject;
    public Transform DirectionalLightTransform;
    public Material LanternGlowingMaterial;
    public Material LanternOffMaterial;
    public SkinnedMeshRenderer LanternSkinnedRenderer;
    public GameObject NightLightGameObject;
    public float DaylightTimer;
    public int day = 0;

    public float daySpeed = 2;
    public float GoblinTimer;
    float GoblinThreshold;
    public GameObject GoblinGameObject;
    public List<Building> NaturalBuildingList = new List<Building>();
    private float gatherCooldown = 0.75f;
    private float nextGather;

    public VisualEffect VFXSparks;
    public VisualEffect VFXPebble;
    public VisualEffect VFXSmokePuff;
    public VisualEffect VFXWoodChip;

    public TMP_Text DayTMP_Text;
    public TMP_Text RemaningTMP_Text;
    public TMP_Text SurvivedTMP_Text;
    private void Awake()
    {
        global = this;
        DaylightTimer = DirectionalLightTransform.eulerAngles.x;
        GameManager.ChangeAnimationLayers(GetComponent<Animation>());

        if (!GameManager.global)
        {

            PlayerPrefs.SetInt("Quick Load", SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(0);
        }
    }

    private void Start()
    {
        VFXSparks.Stop();
        VFXPebble.Stop();
        VFXSmokePuff.Stop();
        VFXWoodChip.Stop();
    }


    private void Update()
    {
        DirectionalLightTransform.Rotate(new Vector3(1, 0, 0), daySpeed * Time.deltaTime);
        DaylightTimer += daySpeed * Time.deltaTime;
        GoblinTimer += Time.deltaTime;



        if (DaylightTimer > 360)
        {
            DaylightTimer = 0;
            day++;
            GameManager.PlayAnimation(GetComponent<Animation>(), "New Day");

            if (day == 1)
            {
                DayTMP_Text.text = "DAY TWO";
                RemaningTMP_Text.text = "THREE DAYS LEFT";
            }
            if (day == 2)
            {
                DayTMP_Text.text = "DAY THREE";
                RemaningTMP_Text.text = "TWO DAYS LEFT";
            }
            if (day == 3)
            {
                DayTMP_Text.text = "DAY FOUR";
                RemaningTMP_Text.text = "ONE DAY LEFT";
            }
            if (day == 4)
            {
                DayTMP_Text.text = "DAY FIVE";
                RemaningTMP_Text.text = "FINAL DAY";
            }

            if (day == 5)
            {
                DayTMP_Text.text = "YOU SURVIVED";
                RemaningTMP_Text.text = "How much longer can you survive?";

            }

            if (day > 5)
            {
                DayTMP_Text.text = "DAY " + (day + 1).ToString();
                RemaningTMP_Text.text = "Highscore: " + (PlayerPrefs.GetInt("Number of Days") + 1);

                if (day > PlayerPrefs.GetInt("Number of Days"))
                {
                    RemaningTMP_Text.text = "Highscore Beaten!";
                    PlayerPrefs.SetInt("Number of Days", day);
                }
            }

        }

        bool nightTimeBool = DaylightTimer > 180;

        if (GoblinTimer >= GoblinThreshold && nightTimeBool)
        {
            GoblinThreshold = Random.Range(15, 20) - (day * 3.5f);
            if (GoblinThreshold < 0.2f)
            {
                GoblinThreshold = 0.2f;
            }
            GoblinTimer = 0;

            Vector3 spawn = PlayerController.global.transform.position;


            spawn.x += Random.Range(10, 20) * (Random.Range(0, 2) == 0 ? -1 : 1);

            spawn.z += Random.Range(10, 20) * (Random.Range(0, 2) == 0 ? -1 : 1);

            GameObject goblin = Instantiate(GoblinGameObject, spawn, Quaternion.identity);

        }

        NightLightGameObject.SetActive(nightTimeBool);
        LanternSkinnedRenderer.material = nightTimeBool ? LanternGlowingMaterial : LanternOffMaterial;


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

                    PlayerController.global.ApplyEnergyDamage(NaturalBuildingList[i].energyConsumptionPerClick);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.global.NextScene(0);
            enabled = false;
            return;
        }

        if (PlayerController.global.transform.position.y < -3)
        {
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
