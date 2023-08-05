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
    public Vector2 distanceAwayFromPlayerX;
    public Vector2 distanceAwayFromPlayerZ;
    public LayerMask buildingLayer;
    public Grid buildGrid;

    public GameObject House;

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
                    SwitchToResourceMode();
                    break;
                case PlayerModes.RepairMode:
                    SwitchToResourceMode();
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
        playerModes = PlayerModes.ResourceMode;
        buildType = BuildType.Slow;
        SwitchToResourceMode();
        buildGrid.gameObject.SetActive(false);
        HUD = HUDHandler.global;
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

            buildGrid.gameObject.SetActive(false);
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
    }

    public void SwitchToBuildMode()
    {
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

        buildGrid.gameObject.SetActive(true);
        SetMouseActive(true);

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });

        HUD.BuildModeHUD();
    }

    public void SwitchToBuildRepairMode()
    {
        ClearBlueprint();

        playerModes = PlayerModes.RepairMode;

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });

        HUD.RepairModeHUD();
    }

    public void SwitchToResourceMode()
    {
        LeaveHouse();

        ClearBlueprint();

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = PlayerController.global.lastWasAxe, PickaxeBool = !PlayerController.global.lastWasAxe });
        playerModes = PlayerModes.ResourceMode;
        SetMouseActive(false);

        HUD.ResourceModeHUD();
    }

    public void SwitchToCombatMode()
    {
        LeaveHouse();

        ClearBlueprint();

        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;
        SetMouseActive(false);

        HUD.CombatModeHUD();
    }

    public void SwitchToUpgradeMode()
    {
        ClearBlueprint();

        playerModes = PlayerModes.UpgradeMenu;

        HUD.UpgradeModeHUD();
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