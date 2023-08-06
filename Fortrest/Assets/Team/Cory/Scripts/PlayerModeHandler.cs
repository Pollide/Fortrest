using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        if (playerModes == PlayerModes.BuildMode)
        {
            BuildMode();

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                SwitchToBuildRepairMode();
            }
        }
        else if (playerModes == PlayerModes.RepairMode)
        {
            RepairMode();

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                SwitchToUpgradeMode();
            }
        }
        else if (playerModes == PlayerModes.UpgradeMenu)
        {
            UpgradeMode();

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
                SwitchToBuildMode();
            }
        }

        ScrollSwitchTurret();

        if ((Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL) && !Boar.global.mounted)
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
                case PlayerModes.BuildMode:
                    SwitchToUpgradeMode();
                    break;
                case PlayerModes.RepairMode:
                    SwitchToBuildMode();
                    break;
                case PlayerModes.UpgradeMenu:
                    SwitchToBuildRepairMode();
                    break;
            }
        }

        if ((Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && House.GetComponent<Building>().playerinRange)
        {
            PlayerController.global.interactCTRL = false;
            if (playerModes != PlayerModes.BuildMode && playerModes != PlayerModes.RepairMode)
            {

                if (House.GetComponent<Building>().textDisplayed)
                {
                    LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, false);
                    House.GetComponent<Building>().textDisplayed = false;
                }
                SwitchToBuildMode();
            }
            else
            {
                if (!House.GetComponent<Building>().textDisplayed)
                {
                    LevelManager.FloatingTextChange(House.GetComponent<Building>().interactText.gameObject, true);
                    House.GetComponent<Building>().textDisplayed = true;
                }
                SwitchToResourceMode();
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

    private void LeaveHouse()
    {
        if (playerModes == PlayerModes.BuildMode || playerModes == PlayerModes.RepairMode)
        {
            if (Boar.global.mounted)
            {
                Boar.global.transform.position = PlayerController.global.houseSpawnPoint.transform.position;
                Boar.global.GetComponent<BoxCollider>().enabled = true;
                Boar.global.cc.enabled = true;
            }
            else
            {

                PlayerController.global.transform.position = PlayerController.global.houseSpawnPoint.transform.position;
                PlayerController.global.playerCC.enabled = true;
            }

            SwitchToBuildMode(false);
            StartCoroutine(PlayerAwake());
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

                if (Input.GetMouseButtonDown(0) && hitData.transform.CompareTag("Turret"))
                {

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
        PlayerController.global.MapResourceHolder.gameObject.SetActive(active);

        if (active)
        {
            ModeSwitchText.global.ResetText();
            ClearSelectionGrid();

            if (Boar.global.mounted)
            {
                Boar.global.canMove = false;
                Boar.global.GetComponent<BoxCollider>().enabled = false;
                Boar.global.cc.enabled = false;
                Boar.global.transform.position = House.transform.position;
                Boar.global.animator.SetBool("Moving", false);
            }
            else
            {
                PlayerController.global.playerCC.enabled = false;
                PlayerController.global.transform.position = House.transform.position;
                PlayerController.global.CharacterAnimator.SetBool("Moving", false);
                PlayerController.global.playerCanMove = false;
            }

            playerModes = PlayerModes.BuildMode;

            SetMouseActive(true);
            PlayerController.global.UpdateResourceHolder();
            PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });

            HUD.BuildModeHUD();

            Debug.Log("Build");
        }
    }




    public void SwitchToBuildRepairMode()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();

        playerModes = PlayerModes.RepairMode;

        HUD.RepairModeHUD();

        Debug.Log("Repair");
    }

    public void SwitchToResourceMode()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        LeaveHouse();

        ClearBlueprint();

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
        playerModes = PlayerModes.ResourceMode;
        SetMouseActive(false);

        HUD.ResourceModeHUD();
        Debug.Log("Resource");
    }

    public void SwitchToCombatMode()
    {
        ModeSwitchText.global.ResetText();
        LeaveHouse();

        ClearBlueprint();

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;
        SetMouseActive(false);

        HUD.CombatModeHUD();
        Debug.Log("Combat");
    }

    public void SwitchToUpgradeMode()
    {
        ModeSwitchText.global.ResetText();
        ClearSelectionGrid();
        ClearBlueprint();

        playerModes = PlayerModes.UpgradeMenu;

        HUD.UpgradeModeHUD();
        Debug.Log("Upgrade");
    }

    bool runOnce;

    private void SpawnBuilding(GameObject _prefab)
    {
        if (Input.GetMouseButtonDown(0) && PlayerController.global.CheckSufficientResources() && !MouseOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
            {
                Vector3 worldPos = hitData.point;

                Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

                worldPos = new Vector3(gridPos.x, worldPos.y, gridPos.z);

                if (IsInRange(worldPos))
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
            if (Input.GetMouseButtonDown(0))
            {
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
            if (!Input.GetMouseButton(0))
            {
                runOnce = false;
            }
        }
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
            // && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource") && !MouseOverUI()

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

                TurretShooting turretShooting = turretBlueprint.GetComponent<TurretShooting>();
                turretShooting.enabled = false;

                if (turretShooting)
                    Destroy(turretShooting);

                BluePrintSet(turretBlueprintBlue);


            }

            Vector3 worldPos = hitData.point;
            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

            worldPos = new Vector3(gridPos.x, worldPos.y, gridPos.z);

            turretBlueprint.transform.position = worldPos;

            if (IsInRange(worldPos) && PlayerController.global.CheckSufficientResources() && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
            {
                BluePrintSet(turretBlueprintBlue);
            }
            else
            {
                BluePrintSet(turretBlueprintRed);
            }
        }
        else if (Physics.Raycast(ray, out hitData, 1000) && (hitData.transform.CompareTag("Player") || hitData.transform.CompareTag("Building") || MouseOverUI()))
        {
            //  Cory FYI I disabled this because there is an issue where it will stay disabled for some reason and its kind of game breaking so lets just leave it active
            //  turretBlueprint.SetActive(!MouseOverUI());
        }
    }

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
        if (isActive == true)
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