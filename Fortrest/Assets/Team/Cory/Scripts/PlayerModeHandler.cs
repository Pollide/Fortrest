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
    RepairMode,
    Paused,
    UpgradeMenu,
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
    public Material turretBlueprintRed;
    public Material turretBlueprintBlue;
    public Vector2 distanceAwayFromPlayerX;
    public Vector2 distanceAwayFromPlayerZ;
    public LayerMask buildingLayer;
    public Grid buildGrid;

    public GameObject House;
    public GameObject selectionGrid;
    GameObject newSelectionGrid;

    private HUDHandler HUD;

    public bool inTheFortress;
    private bool centerMouse;
    Vector2 cursorPosition;
    Vector3 entryPosition;

    public bool canInteractWithHouse;

    public float nimDistanceBetweenTurrts = 3;

    bool runOnce;
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
        if (PlayerController.global.pausedBool)
        {
            return;
        }

        inTheFortress = playerModes == PlayerModes.BuildMode || playerModes == PlayerModes.RepairMode || playerModes == PlayerModes.UpgradeMenu;

        if (inTheFortress)
        {
            if (Weather.global.gameObject.activeSelf)
            {
                Weather.global.gameObject.SetActive(false);
            }
            if (!centerMouse)
            {
                Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
                cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);
                centerMouse = true;
            }
            if (PlayerController.global.moveCTRL.x != 0 || PlayerController.global.moveCTRL.y != 0)
            {
                cursorPosition += PlayerController.global.moveCTRL * 1.25f;
                Mouse.current.WarpCursorPosition(cursorPosition);
            }
            else
            {
                cursorPosition = Input.mousePosition;
            }

            if (playerModes == PlayerModes.BuildMode)
            {
                BuildMode();

                if (Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL)
                {
                    PlayerController.global.swapCTRL = false;
                    GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                    SwitchToBuildRepairMode();
                }
            }
            else if (playerModes == PlayerModes.RepairMode)
            {
                RepairMode();

                if (Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL)
                {
                    PlayerController.global.swapCTRL = false;
                    GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                    SwitchToUpgradeMode();
                }
            }
            else if (playerModes == PlayerModes.UpgradeMenu)
            {
                UpgradeMode();

                if (Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL)
                {
                    PlayerController.global.swapCTRL = false;
                    GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                    SwitchToBuildMode();
                }
            }
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

        if (House.GetComponent<Building>().playerinRange && !Boar.global.mounted && Boar.global.closerToHouse && !PlayerController.global.playerDead && !PlayerController.global.teleporting && !PlayerController.global.respawning)
        {
            canInteractWithHouse = true;
            PlayerController.global.needInteraction = true;
        }
        else
        {
            canInteractWithHouse = false;
            PlayerController.global.needInteraction = false;
        }

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && canInteractWithHouse)
        {
            PlayerController.global.interactCTRL = false;
            if (playerModes != PlayerModes.BuildMode && playerModes != PlayerModes.RepairMode)
            {
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
        if (buildType == BuildType.Turret)
        {
            SpawnBuilding(turretPrefabs[0]);

        }
        else if (buildType == BuildType.Slow)
        {
            SpawnBuilding(turretPrefabs[1]);
        }
        else if (buildType == BuildType.Cannon)
        {
            SpawnBuilding(turretPrefabs[2]);
        }
        else if (buildType == BuildType.Scatter)
        {
            SpawnBuilding(turretPrefabs[3]);
        }

        DragBuildingBlueprint();
    }

    private void ScrollSwitchTurret()
    {
        if (Input.mouseScrollDelta.y > 0f && playerModes == PlayerModes.BuildMode)
        {
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
        if (Input.mouseScrollDelta.y < 0f && playerModes == PlayerModes.BuildMode)
        {
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
        }

        PlayerController.global.UpdateResourceHolder();
    }

    void ClearSelectionGrid()
    {
        if (newSelectionGrid)
        {
            Destroy(newSelectionGrid);
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

            CameraFollow.global.transform.position = CameraFollow.global.ReturnBuildOffset();

            playerModes = PlayerModes.BuildMode;

            SetMouseActive(true);
            PlayerController.global.UpdateResourceHolder();
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });

            HUD.BuildModeHUD();

            // Debug.Log("Build");
        }
        else
        {

        }

        CameraFollow.global.Update(); //refreshes it instantly
    }

    public void SwitchToBuildRepairMode()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();

        playerModes = PlayerModes.RepairMode;

        HUD.RepairModeHUD();

        //  Debug.Log("Repair");
    }

    public void ExitHouseCleanUp()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();
        SetMouseActive(false);
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

    public void SwitchToResourceMode()
    {
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
        playerModes = PlayerModes.ResourceMode;
        HUD.ResourceModeHUD();
    }

    public void SwitchToCombatMode()
    {
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;
        HUD.CombatModeHUD();
    }

    public void SwitchToUpgradeMode()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();

        playerModes = PlayerModes.UpgradeMenu;

        HUD.UpgradeModeHUD();
        //   Debug.Log("Upgrade");
    }

    private void SpawnBuilding(GameObject _prefab)
    {
        if ((Input.GetMouseButtonDown(0) || PlayerController.global.selectCTRL) && PlayerController.global.CheckSufficientResources() && !MouseOverUI())
        {
            PlayerController.global.selectCTRL = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
            {
                Vector3 worldPos = hitData.point;

                Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

                worldPos = new Vector3(gridPos.x, 0.0f, gridPos.z);

                Collider[] collidershit = Physics.OverlapSphere(worldPos, nimDistanceBetweenTurrts);

                if (IsInRange(worldPos) && !ReturnColiders(collidershit))
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TurretPlaceSound);
                    GameObject newTurret = Instantiate(_prefab, worldPos, Quaternion.identity);

                    PlayerController.global.CheckSufficientResources(true);


                    LevelManager.global.VFXSmokePuff.transform.position = newTurret.transform.position + new Vector3(0, .5f, 0);

                    LevelManager.global.VFXSmokePuff.Play();
                    // Debug.Log("working");
                }
                else
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                }
            }
            else if (Physics.Raycast(ray, out hitData, 1000) && hitData.transform.CompareTag("Player"))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                Debug.Log("Cannot Place Building Here");
            }
            else if (Physics.Raycast(ray, out hitData, 1000) && (hitData.transform.CompareTag("Building") || hitData.transform.CompareTag("Resource")))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                Debug.Log("Building Here");
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) || PlayerController.global.selectCTRL)
            {
                PlayerController.global.selectCTRL = false;
                if (!runOnce)
                {
                    runOnce = true;
                    if (!PlayerController.global.CheckSufficientResources())
                    {
                        GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                        Debug.Log("Not Enough Resources");
                    }
                }
            }
            if (!Input.GetMouseButton(0) || !PlayerController.global.selectCTRL)
            {
                runOnce = false;
            }
        }
    }

    private bool ReturnColiders(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Building") || collider.CompareTag("Resource") || collider.CompareTag("Turret"))
            {
                Debug.Log("true");
                return true;
            }
        }
        Debug.Log("false");
        return false;
    }

    void BluePrintSet(Material colorMaterial)
    {
        List<Renderer> meshRenderers = GameManager.FindComponent<Renderer>(turretBlueprint.transform);

        for (int i = 0; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].material = colorMaterial;
        }
    }

    private void DragBuildingBlueprint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitData, 1000, ~buildingLayer))
        {
            if (!turretBlueprint)
            {
                if (buildType == BuildType.Slow)
                {
                    turretBlueprint = Instantiate(turretPrefabs[1]);
                }
                else if (buildType == BuildType.Cannon)
                {
                    turretBlueprint = Instantiate(turretPrefabs[2]);
                }
                else if (buildType == BuildType.Scatter)
                {
                    turretBlueprint = Instantiate(turretPrefabs[3]);
                }
                else
                {
                    turretBlueprint = Instantiate(turretPrefabs[0]);
                }

                turretBlueprint.GetComponent<Building>().resourceObject = Building.BuildingType.DefenseBP;
                turretBlueprint.GetComponent<Building>().enabled = false;
                turretBlueprint.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                turretBlueprint.tag = "BuildingBP";

                TurretShooting turretShooting = turretBlueprint.GetComponent<TurretShooting>();

                if (turretShooting)
                {
                    turretShooting.enabled = false;
                    Destroy(turretShooting);
                }
            }

            Vector3 worldPos = hitData.point;
            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

            worldPos = new Vector3(gridPos.x, worldPos.y, gridPos.z);
            turretBlueprint.transform.position = worldPos;

            Collider[] collidershit = Physics.OverlapSphere(new Vector3(worldPos.x, 0f, worldPos.z), nimDistanceBetweenTurrts);

            if (IsInRange(worldPos) && PlayerController.global.CheckSufficientResources() && !ReturnColiders(collidershit))
            {
                BluePrintSet(turretBlueprintBlue);
            }
            else
            {
                BluePrintSet(turretBlueprintRed);
            }
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


        if (isActive && GameManager.global.KeyboardBool)
        {
            // Debug.Log(isActive + " && " + ReturnKeyboard() + " " + Cursor.visible);

            //  Debug.Log(1);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

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