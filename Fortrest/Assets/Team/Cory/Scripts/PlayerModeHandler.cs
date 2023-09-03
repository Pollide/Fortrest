using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum PlayerModes
{
    BuildMode,
    ResourceMode,
    CombatMode,
    Paused,
}

public enum BuildType
{
    Turret,
    Cannon,
    Slow,
    Scatter,
    None,
}

public class PlayerModeHandler : MonoBehaviour
{
    public static PlayerModeHandler global;
    public PlayerModes playerModes;
    private PlayerModes lastMode;
    public BuildType buildType;
    public GameObject[] turretPrefabs;
    GameObject turretBlueprint;
    Transform KeyHint;
    public Material turretBlueprintRed;
    public Material turretBlueprintBlue;
    public Vector2 distanceAwayFromPlayerX;
    public Vector2 distanceAwayFromPlayerZ;
    public LayerMask buildingLayer;
    public Grid buildGrid;

    public GameObject House;
    public GameObject selectionGrid;
    GameObject newSelectionGrid;
    public GameObject KeyBlueprintHintPrefab;
    private HUDHandler HUD;

    public bool inTheFortress;
    private bool centerMouse;
    Vector2 cursorPosition;
    Vector3 entryPosition;

    public bool canInteractWithHouse;

    public float nimDistanceBetweenTurrts = 3;

    bool runOnce;

    public Vector3 HintOffset;


    public Building SelectedTurret;
    private void Awake()
    {
        if (global)
        {
            //destroys the duplicate
            Destroy(gameObject);
            //  GrassComputeScript.global.interactors.Remove(GetComponentInChildren<ShaderInteractor>());
        }
        else
        {
            //itself doesnt exist so set it
            global = this;
        }
    }

    private void Update()
    {
        if (PlayerController.global.pausedBool || PlayerController.global.mapBool)
        {
            return;
        }

        inTheFortress = playerModes == PlayerModes.BuildMode;

        if (inTheFortress)
        {
            if (Weather.global.gameObject.activeSelf)
            {
                Weather.global.gameObject.SetActive(false);
            }
            if (!centerMouse)
            {
                if (GameManager.global.KeyboardBool)
                    Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));

                cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);
                centerMouse = true;
            }
            if (GameManager.global.moveCTRL.x != 0 || GameManager.global.moveCTRL.y != 0)
            {
                cursorPosition += GameManager.global.moveCTRL * 1.25f;

                if (GameManager.global.KeyboardBool)
                    Mouse.current.WarpCursorPosition(cursorPosition);
            }
            else if (GameManager.global.KeyboardBool)
            {
                cursorPosition = Input.mousePosition;
            }

            BuildMode();

            ScrollSwitchTurret();
        }
        else
        {
            if (!Weather.global.gameObject.activeSelf)
            {
                Weather.global.gameObject.SetActive(true);
            }
            if ((Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL) && PlayerController.global.playerCanMove)
            {
                PlayerController.global.swapCTRL = false;
                GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                switch (playerModes)
                {
                    case PlayerModes.CombatMode:
                        PlayerController.global.canShoot = false;
                        PlayerController.global.CharacterAnimator.SetBool("Aiming", false);
                        PlayerController.global.lunge = false;
                        SwitchToResourceMode();
                        break;
                    case PlayerModes.ResourceMode:
                        SwitchToCombatMode();
                        break;
                    default:
                        break;
                }
            }
        }

        if (House.GetComponent<Building>().playerinRange && !Boar.global.mounted && (Boar.global.closerToHouse || (!Boar.global.closerToHouse && !Boar.global.inRange)) && !PlayerController.global.playerDead && !PlayerController.global.teleporting && !PlayerController.global.respawning)
        {
            canInteractWithHouse = true;
            PlayerController.global.needInteraction = true;
        }
        else
        {
            canInteractWithHouse = false;
            if (!Boar.global.canInteractWithBoar && !PlayerController.global.canTeleport && PlayerController.global.playerRespawned && !PlayerController.global.bridgeInteract)
            {
                PlayerController.global.needInteraction = false;
            }
        }

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && canInteractWithHouse)
        {
            PlayerController.global.interactCTRL = false;

            if (!inTheFortress)
            {
                PlayerController.global.evading = false;
                lastMode = playerModes;
                entryPosition = PlayerController.global.transform.position;
                if (House.GetComponent<Building>().textDisplayed)
                {
                    LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, false);
                    House.GetComponent<Building>().textDisplayed = false;
                }
                centerMouse = false;
                Boar.global.body.SetActive(false);
                SwitchToBuildMode();
            }
            else
            {
                if (!House.GetComponent<Building>().textDisplayed)
                {
                    LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, true);
                    House.GetComponent<Building>().textDisplayed = true;
                }
                Boar.global.body.SetActive(true);
                ExitHouseCleanUp();
            }
        }
    }

    public bool MouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        buildType = BuildType.Turret;
        HUD = HUDHandler.global;
        SwitchToBuildMode(false);
        SwitchToResourceMode();
        entryPosition = PlayerController.global.transform.position;
    }

    private void BuildMode()
    {
        Ray ray = LevelManager.global.SceneCamera.ScreenPointToRay(cursorPosition);

        if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Terrain" })))
        {


            GameObject turretPrefab = turretPrefabs[0];

            if (buildType == BuildType.Slow)
            {
                turretPrefab = turretPrefabs[1];
            }
            else if (buildType == BuildType.Cannon)
            {
                turretPrefab = turretPrefabs[2];
            }
            else if (buildType == BuildType.Scatter)
            {
                turretPrefab = turretPrefabs[3];
            }

            Vector3 worldPos = hitData.point;
            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

            worldPos = new Vector3(gridPos.x, 0, gridPos.z);

            Collider[] colliders = Physics.OverlapSphere(worldPos, nimDistanceBetweenTurrts, GameManager.ReturnBitShift(new string[] { "Building", "Resource" }));
            PlayerController.global.TurretMenuHolder.gameObject.SetActive(SelectedTurret);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == "Turret")
                {
                    if (!SelectedTurret)
                    {

                        PlayerController.global.TurretMenuTitle.text = buildType.ToString();
                        ClearBlueprint();
                    }

                    PlayerController.global.TurretMenuHolder.position = LevelManager.global.SceneCamera.WorldToScreenPoint(hitData.point);
                    SelectedTurret = colliders[i].GetComponentInParent<Building>();

                    return;
                }
            }
            SelectedTurret = null;

            if (!turretBlueprint)
            {
                turretBlueprint = Instantiate(turretPrefab);

                turretBlueprint.GetComponent<Building>().resourceObject = Building.BuildingType.DefenseBP;
                turretBlueprint.GetComponent<Building>().enabled = false;
                turretBlueprint.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                turretBlueprint.tag = "BuildingBP";
                turretBlueprint.layer = 0;
                //NOT IN USE RIGHT NOW but basically it would hover over the building to say to place
                //KeyHint = Instantiate(KeyBlueprintHintPrefab).transform;

                TurretShooting turretShooting = turretBlueprint.GetComponent<TurretShooting>();

                if (turretShooting)
                {
                    turretShooting.enabled = false;
                    Destroy(turretShooting);
                }

            }

            turretBlueprint.transform.position = worldPos;

            if (KeyHint)
                KeyHint.position = worldPos + HintOffset;

            bool selectBool = (Input.GetMouseButtonDown(0) || PlayerController.global.selectCTRL) && !MouseOverUI();

            if (selectBool)
            {
                PlayerController.global.selectCTRL = false;
            }
            else
            {
                runOnce = false;
            }
            if (IsInRange(worldPos) && PlayerController.global.CheckSufficientResources() && colliders.Length == 0)
            {
                BluePrintSet(turretBlueprintBlue);

                if (selectBool)
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TurretPlaceSound);
                    GameObject newTurret = Instantiate(turretPrefab, worldPos, Quaternion.identity);

                    PlayerController.global.CheckSufficientResources(true);

                    LevelManager.global.VFXSmokePuff.transform.position = newTurret.transform.position + new Vector3(0, .5f, 0);

                    //!hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource")
                    LevelManager.global.VFXSmokePuff.Play();
                }
            }
            else
            {
                BluePrintSet(turretBlueprintRed);


                if (selectBool)
                {
                    if (!runOnce)
                    {
                        runOnce = true;

                        if (!PlayerController.global.CheckSufficientResources())
                        {
                            PlayerController.global.ShakeResourceHolder();

                        }
                    }
                }
            }
        }
    }

    private void ScrollSwitchTurret()
    {
        if ((Input.mouseScrollDelta.y > 0f || PlayerController.global.scrollCTRL) && playerModes == PlayerModes.BuildMode)
        {
            PlayerController.global.scrollCTRL = false;
            if (buildType == BuildType.Turret)
            {
                SwitchBuildTypeCannon();
            }
            else if (buildType == BuildType.Cannon)
            {
                SwitchBuildTypeSlow();
            }
            else if (buildType == BuildType.Slow)
            {
                SwitchBuildTypeScatter();
            }
            else if (buildType == BuildType.Scatter)
            {
                SwitchBuildTypeTurret();
            }
        }
        if ((Input.mouseScrollDelta.y < 0f || PlayerController.global.scrollCTRL) && playerModes == PlayerModes.BuildMode)
        {
            PlayerController.global.scrollCTRL = false;
            if (buildType == BuildType.Turret)
            {
                SwitchBuildTypeScatter();
            }
            else if (buildType == BuildType.Cannon)
            {
                SwitchBuildTypeTurret();
            }
            else if (buildType == BuildType.Slow)
            {
                SwitchBuildTypeCannon();
            }
            else if (buildType == BuildType.Scatter)
            {
                SwitchBuildTypeSlow();
            }
        }
    }

    IEnumerator PlayerAwake()
    {
        yield return new WaitForSeconds(0.1f);
        if (Boar.global.mounted)
        {
            Boar.global.canMove = true;
        }
        else
        {
            PlayerController.global.playerCanMove = true;
        }
    }

    public void RepairMode()
    {
        Ray ray = LevelManager.global.SceneCamera.ScreenPointToRay(cursorPosition);

        if (Physics.Raycast(ray, out RaycastHit hitData, 1000))
        {
            Vector3 worldPos = hitData.point;

            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

            worldPos = new Vector3(gridPos.x, worldPos.y, gridPos.z);

            if (IsInRange(worldPos))
            {

                if ((Input.GetMouseButtonDown(0) || PlayerController.global.selectCTRL) && hitData.transform.CompareTag("Turret"))
                {
                    PlayerController.global.selectCTRL = false;
                }

                if (!newSelectionGrid)
                {
                    newSelectionGrid = Instantiate(selectionGrid, worldPos, Quaternion.Euler(90, 0, 0));
                }

                if (hitData.transform.CompareTag("Turret"))
                {
                    newSelectionGrid.GetComponentInChildren<Image>().color = Color.green;
                }
                else
                {
                    newSelectionGrid.GetComponentInChildren<Image>().color = Color.red;
                }

                newSelectionGrid.transform.position = worldPos;
            }
            else
            {
                ClearSelectionGrid();
            }

        }

    }

    public void UpgradeMode()
    {

    }

    void ClearBlueprint()
    {
        if (turretBlueprint)
        {
            Destroy(turretBlueprint);

            if (KeyHint)
                Destroy(KeyHint.gameObject);
        }
        PlayerController.global.TurretMenuHolder.gameObject.SetActive(false);
        PlayerController.global.UpdateResourceHolder();
    }

    void ClearSelectionGrid()
    {
        if (newSelectionGrid)
        {
            Destroy(newSelectionGrid);
        }
    }

    public void ExitHouseCleanUp()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();
        SwitchToBuildMode(false);
        StartCoroutine(PlayerAwake());
        PlayerController.global.TeleportPlayer(entryPosition);
        if (lastMode == PlayerModes.ResourceMode)
        {
            SwitchToResourceMode();
        }
        else if (lastMode == PlayerModes.CombatMode)
        {
            SwitchToCombatMode();
        }
    }

    public void SwitchToBuildMode(bool active = true)
    {
        buildGrid.gameObject.SetActive(active);
        PlayerController.global.OpenResourceHolder(active);

        PlayerController.global.CharacterAnimator.gameObject.SetActive(!active);
        if (active)
        {
            ModeSwitchText.global.ResetText();
            ClearSelectionGrid();
            PlayerController.global.TeleportPlayer(PlayerController.global.house.transform.position);
            if (Boar.global.mounted)
            {
                Boar.global.canMove = false;
            }
            else
            {
                PlayerController.global.playerCanMove = false;
            }

            playerModes = PlayerModes.BuildMode;
            PlayerController.global.UpdateResourceHolder();
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });

            // Debug.Log("Build");
        }
        else
        {

        }

        CameraFollow.global.Update(); //refreshes it instantly
    }

    public void SwitchToResourceMode()
    {
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
        playerModes = PlayerModes.ResourceMode;
    }

    public void SwitchToCombatMode()
    {
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;

    }


    void BluePrintSet(Material colorMaterial)
    {
        List<Renderer> meshRenderers = GameManager.FindComponent<Renderer>(turretBlueprint.transform);

        for (int i = 0; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].material = colorMaterial;
        }
    }

    //// Check building distance
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    Vector3 house = House.transform.position;

    //    float top = house.x + distanceAwayFromPlayerX.x;
    //    float bot = house.x - distanceAwayFromPlayerX.y;
    //    float right = house.z + distanceAwayFromPlayerZ.x;
    //    float left = house.z - distanceAwayFromPlayerZ.y;

    //    Vector3 houseX1 = new(top, house.y, house.z);
    //    Vector3 houseX2 = new(bot, house.y, house.z);
    //    Vector3 houseZ1 = new(house.x, house.y, right);
    //    Vector3 houseZ2 = new(house.x, house.y, left);
    //    Gizmos.DrawLine(house, houseX1);
    //    Gizmos.DrawLine(house, houseX2);
    //    Gizmos.DrawLine(house, houseZ1);
    //    Gizmos.DrawLine(house, houseZ2);
    //}

    public bool IsInRange(Vector3 currentTarget)
    {
        Vector3 playerPos = PlayerController.global.transform.position;

        float top = playerPos.x + distanceAwayFromPlayerX.x;
        float bot = playerPos.x - distanceAwayFromPlayerX.y;
        float right = playerPos.z + distanceAwayFromPlayerZ.x;
        float left = playerPos.z - distanceAwayFromPlayerZ.y;

        if (currentTarget.x < top && currentTarget.x > bot && currentTarget.z < right && currentTarget.z > left)
        {
            return true;
        }

        return false;
    }


    public static void SetMouseActive(bool isActive)
    {
        //  Debug.Log("Cursor " + isActive);

        if (isActive && GameManager.global.KeyboardBool)
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;

                Cursor.lockState = CursorLockMode.None;
            }

        }
        else
        {
            if (Cursor.visible)
            {

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void SwitchBuildTypeSlow()
    {
        buildType = BuildType.Slow;
        ClearBlueprint();
    }

    public void SwitchBuildTypeCannon()
    {
        buildType = BuildType.Cannon;
        ClearBlueprint();
    }

    public void SwitchBuildTypeTurret()
    {
        buildType = BuildType.Turret;
        ClearBlueprint();
    }

    public void SwitchBuildTypeScatter()
    {
        buildType = BuildType.Scatter;
        ClearBlueprint();
    }
}