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
    Paused,
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
    public GameObject turretBlueprint;
    Transform[] parts;
    public Material turretBlueprintRed;
    public Material turretBlueprintBlue;
    public float distanceAwayFromPlayer = 30;
    public LayerMask buildingLayer;
    public Image buildingMode;
    public Image resourceMode;
    public Image combatMode;

    private void Awake()
    {
        global = this;
        parts = turretBlueprint.GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        if (playerModes == PlayerModes.BuildMode)
        {
            if (buildType == BuildType.Turret)
            {
                Building building = turretPrefabs[0].GetComponentInChildren<Building>();
                DragBuildingBlueprint(building.constructionCostWood, building.constructionCostStone);
                SpawnBuilding(turretPrefabs[0], building.constructionCostWood, building.constructionCostStone);
                
            }
            else if (buildType == BuildType.Slow)
            {
                Building building = turretPrefabs[1].GetComponentInChildren<Building>();
                DragBuildingBlueprint(building.constructionCostWood, building.constructionCostStone);
                SpawnBuilding(turretPrefabs[1], building.constructionCostWood, building.constructionCostStone);
            }
            else if (buildType == BuildType.Cannon)
            {
                Building building = turretPrefabs[2].GetComponentInChildren<Building>();
                DragBuildingBlueprint(building.constructionCostWood, building.constructionCostStone);
                SpawnBuilding(turretPrefabs[2], building.constructionCostWood, building.constructionCostStone);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
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
        SwitchToGatherMode();
    }

    public void SwitchToBuildMode()
    {
        playerModes = PlayerModes.BuildMode;

        SetMouseActive(true);

        buildingMode.enabled = true;
        resourceMode.enabled = false;
        combatMode.enabled = false;
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { HammerBool = true });
    }

    public void SwitchToGatherMode()
    {
        if (turretBlueprint.activeInHierarchy)
        {
            turretBlueprint.SetActive(false);
        }
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { AxeBool = true });
        playerModes = PlayerModes.ResourceMode;
        SetMouseActive(false);
        buildingMode.enabled = false;
        resourceMode.enabled = true;
        combatMode.enabled = false;
    }

    public void SwitchToCombatMode()
    {
        if (turretBlueprint.activeInHierarchy)
        {
            turretBlueprint.SetActive(false);
        }
        PlayerController.global.ChangeTool(new PlayerController.ToolData() { SwordBool = true });
        playerModes = PlayerModes.CombatMode;
        SetMouseActive(false);
        buildingMode.enabled = false;
        resourceMode.enabled = false;
        combatMode.enabled = true;
    }
    bool runOnce;

    private void SpawnBuilding(GameObject _prefab, int _constructionCostWood, int _constructionCostStone)
    {
        if (Input.GetMouseButtonDown(0) && InventoryManager.global.GetItemQuantity("Wood") >= _constructionCostWood && InventoryManager.global.GetItemQuantity("Stone") >= _constructionCostStone && !MouseOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
            {
                Vector3 worldPos = hitData.point;

                if (IsInRange(worldPos))
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TurretPlaceSound);
                    GameObject newTurret = Instantiate(_prefab, worldPos, Quaternion.identity);
                    InventoryManager.global.RemoveItem("Wood", _constructionCostWood);
                    InventoryManager.global.RemoveItem("Stone", _constructionCostStone);

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
                    if (InventoryManager.global.GetItemQuantity("Wood") < _constructionCostWood || InventoryManager.global.GetItemQuantity("Stone") < _constructionCostStone)
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

    private void DragBuildingBlueprint(int _constructionCostWood, int _constructionCostStone)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource") && !MouseOverUI())
        {
            turretBlueprint.SetActive(true);

            Vector3 worldPos = hitData.point;

            turretBlueprint.transform.position = worldPos;

            if (IsInRange(worldPos) &&
                InventoryManager.global.GetItemQuantity("Wood") >= _constructionCostWood && 
                InventoryManager.global.GetItemQuantity("Stone") >= _constructionCostStone)
            {
                foreach (Transform child in parts)
                {
                    if (child.GetComponent<MeshRenderer>() && child.GetComponent<MeshRenderer>().material != turretBlueprintBlue)
                    {
                        child.GetComponent<MeshRenderer>().material = turretBlueprintBlue;
                    }
                }
            }
            else
            {
                foreach (Transform child in parts)
                {
                    if (child.GetComponent<MeshRenderer>() && child.GetComponent<MeshRenderer>().material != turretBlueprintRed)
                    {
                        child.GetComponent<MeshRenderer>().material = turretBlueprintRed;
                    }
                }
            }
        }
        else if (Physics.Raycast(ray, out hitData, 1000) && (hitData.transform.CompareTag("Player") || hitData.transform.CompareTag("Building") || MouseOverUI()))
        {
            turretBlueprint.SetActive(false);
        }
    }
    public bool IsInRange(Vector2 currentTarget)
    {
        PlayerController player = PlayerController.global;
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
        return distanceAwayFromPlayer >= Vector2.Distance(playerPos, currentTarget);
    }


    private void SetMouseActive(bool isActive)
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