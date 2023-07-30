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
    public Vector2 distanceAwayFromPlayer;
    public LayerMask buildingLayer;
    public Image buildingMode;
    public Image resourceMode;
    public Image combatMode;
    public Image repairMode;
    public Image resourceModeSub;
    public Image combatModeSub;
    private Grid buildGrid;

    private void Awake()
    {
        if (global)
        {
            //destroys the duplicate
            Destroy(gameObject);
            GrassComputeScript.global.interactors.Remove(GetComponentInChildren<ShaderInteractor>());
        }
        else
        {
            //itself doesnt exist so set it
            global = this;
        }

        buildGrid = GameObject.Find("BuildingGrid").GetComponent<Grid>();
    }

    private void Update()
    {
        if (playerModes == PlayerModes.BuildMode)
        {
            BuildMode();
        }

        ScrollSwitchTurret();

        if (Input.GetKeyDown(KeyCode.Q) || PlayerController.global.swapCTRL)
        {
            PlayerController.global.swapCTRL = false;
            GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
            switch (playerModes)
            {
                case PlayerModes.CombatMode:
                    SwitchToGatherMode();
                    break;
                case PlayerModes.ResourceMode:
                    SwitchToCombatMode();
                    break;
                case PlayerModes.BuildMode:
                    SwitchToGatherMode();
                    break;
                case PlayerModes.RepairMode:
                    SwitchToGatherMode();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && GameObject.Find("House") != null && GameObject.Find("House").GetComponent<Building>().playerinRange)
        {
            SwitchToBuildMode();
        }
    }

    public bool MouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerModes = PlayerModes.ResourceMode;
        buildType = BuildType.Slow;
        SwitchToGatherMode();
        buildGrid.gameObject.SetActive(false);
    }

    private void BuildMode()
    {
        if (buildType == BuildType.Turret)
        {
            Building building = turretPrefabs[0].GetComponentInChildren<Building>();
            DragBuildingBlueprint("Wood", "Stone", building.constructionCostWood, building.constructionCostStone);
            SpawnBuilding(turretPrefabs[0], "Wood", "Stone", building.constructionCostWood, building.constructionCostStone);

        }
        else if (buildType == BuildType.Slow)
        {
            Building building = turretPrefabs[1].GetComponentInChildren<Building>();
            DragBuildingBlueprint("HardWood", "MossyStone", building.constructionCostWood, building.constructionCostStone);
            SpawnBuilding(turretPrefabs[1], "HardWood", "MossyStone", building.constructionCostWood, building.constructionCostStone);
        }
        else if (buildType == BuildType.Cannon)
        {
            Building building = turretPrefabs[2].GetComponentInChildren<Building>();
            DragBuildingBlueprint("CoarseWood", "Slate", building.constructionCostWood, building.constructionCostStone);
            SpawnBuilding(turretPrefabs[2], "CoarseWood", "Slate", building.constructionCostWood, building.constructionCostStone);
        }
    }

    private void ScrollSwitchTurret()
    {
        if (Input.mouseScrollDelta.y > 0f && playerModes == PlayerModes.BuildMode)
        {
            if (buildType == BuildType.Turret)
            {
                buildType = BuildType.Cannon;
            }
            else if (buildType == BuildType.Cannon)
            {
                buildType = BuildType.Slow;
            }
            else if (buildType == BuildType.Slow)
            {
                buildType = BuildType.Turret;
            }
        }
        if (Input.mouseScrollDelta.y < 0f && playerModes == PlayerModes.BuildMode)
        {
            if (buildType == BuildType.Turret)
            {
                buildType = BuildType.Slow;
            }
            else if (buildType == BuildType.Cannon)
            {
                buildType = BuildType.Turret;
            }
            else if (buildType == BuildType.Slow)
            {
                buildType = BuildType.Cannon;
            }
        }
    }

    private void LeaveHouse()
    {
        if (playerModes == PlayerModes.BuildMode || playerModes == PlayerModes.RepairMode)
        {
            Vector3 housePos = GameObject.Find("House").transform.position;
            Vector3 leavePosition = new(housePos.x, housePos.y, housePos.z - 100);
            PlayerController.global.transform.position = leavePosition;
            PlayerController.global.playerCanMove = true;
            buildGrid.gameObject.SetActive(false);
        }
    }

    public void RepairMode()
    {

    }

    public void SwitchToBuildMode()
    {

        PlayerController.global.playerCanMove = false;
        PlayerController.global.transform.position = GameObject.Find("House").transform.position;
        buildGrid.gameObject.SetActive(true);


        playerModes = PlayerModes.BuildMode;

        SetMouseActive(true);

        buildingMode.enabled = true;
        resourceMode.enabled = false;
        combatMode.enabled = false;
        repairMode.transform.GetChild(0).gameObject.SetActive(false);
        repairMode.enabled = false;

        resourceModeSub.enabled = true;
        combatModeSub.enabled = false;
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });
    }

    public void SwitchToBuildRepairMode()
    {
        ClearBlueprint();

        playerModes = PlayerModes.RepairMode;

        SetMouseActive(false);

        buildingMode.enabled = false;
        resourceMode.enabled = false;
        combatMode.enabled = false;
        repairMode.transform.GetChild(0).gameObject.SetActive(true);
        repairMode.enabled = true;

        resourceModeSub.enabled = true;
        combatModeSub.enabled = false;
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = true });
    }

    void ClearBlueprint()
    {
        if (turretBlueprint)
        {
            Destroy(turretBlueprint);
        }
    }
    public void SwitchToGatherMode()
    {
        LeaveHouse();

        ClearBlueprint();

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = true });
        playerModes = PlayerModes.ResourceMode;
        SetMouseActive(false);
        buildingMode.enabled = false;
        resourceMode.enabled = true;
        combatMode.enabled = false;
        repairMode.transform.GetChild(0).gameObject.SetActive(false);
        repairMode.enabled = false;

        resourceModeSub.enabled = false;
        combatModeSub.enabled = true;
    }

    public void SwitchToCombatMode()
    {
        LeaveHouse();

        ClearBlueprint();

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;
        SetMouseActive(false);
        buildingMode.enabled = false;
        resourceMode.enabled = false;
        combatMode.enabled = true;
        repairMode.transform.GetChild(0).gameObject.SetActive(false);
        repairMode.enabled = false;

        resourceModeSub.enabled = true;
        combatModeSub.enabled = false;
    }
    bool runOnce;

    private void SpawnBuilding(GameObject _prefab, string _resource1, string _resource2, int _resource1Cost, int _resource2Cost)
    {
        if (GameManager.global.CheatInfiniteBuilding)
        {
            _resource1Cost = -1;
            _resource2Cost = -1;

        }
        if (Input.GetMouseButtonDown(0) && InventoryManager.global.GetItemQuantity(_resource1) >= _resource1Cost && InventoryManager.global.GetItemQuantity(_resource2) >= _resource2Cost && !MouseOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
            {
                Vector3 worldPos = hitData.point;

                Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

                worldPos = new Vector3(gridPos.x, worldPos.y, gridPos.z);

                if (IsInRange(worldPos))
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TurretPlaceSound);
                    GameObject newTurret = Instantiate(_prefab, worldPos, Quaternion.identity);
                    InventoryManager.global.RemoveItem(_resource1, _resource1Cost);
                    InventoryManager.global.RemoveItem(_resource2, _resource2Cost);

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
                    if (InventoryManager.global.GetItemQuantity("Wood") < _resource1Cost || InventoryManager.global.GetItemQuantity("Stone") < _resource2Cost)
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

    private void DragBuildingBlueprint(string _resource1, string _resource2, int _resource1Cost, int _resource2Cost)
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
                else
                {
                    turretBlueprint = Instantiate(turretPrefabs[0]);
                }

                turretBlueprint.GetComponent<Building>().resourceObject = Building.BuildingType.DefenseBP;

                TurretShooting turretShooting = turretBlueprint.GetComponent<TurretShooting>();

                if (turretShooting)
                    Destroy(turretShooting);

                BluePrintSet(turretBlueprintBlue);


            }

            Vector3 worldPos = hitData.point;
            Vector3 gridPos = buildGrid.GetCellCenterWorld(buildGrid.WorldToCell(worldPos));

            worldPos = new Vector3(gridPos.x, worldPos.y, gridPos.z);

            turretBlueprint.transform.position = worldPos;

            if (IsInRange(worldPos) &&
                InventoryManager.global.GetItemQuantity(_resource1) >= _resource1Cost &&
                InventoryManager.global.GetItemQuantity(_resource2) >= _resource2Cost && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
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

        float top = playerPos.x + distanceAwayFromPlayer.x;
        float bot = playerPos.x - distanceAwayFromPlayer.x;
        float right = playerPos.z + distanceAwayFromPlayer.y;
        float left = playerPos.z - distanceAwayFromPlayer.y;

        if (currentTarget.x < top && currentTarget.x > bot && currentTarget.z < right && currentTarget.z > left)
        {
            return true;
        }

        return false;
    }

    public void SetMouseActive(bool isActive)
    {
        if (isActive == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SwitchBuildTypeSlow()
    {
        buildType = BuildType.Slow;
    }

    public void SwitchBuildTypeCannon()
    {
        buildType = BuildType.Cannon;
    }

    public void SwitchBuildTypeTurret()
    {
        buildType = BuildType.Turret;
    }

}