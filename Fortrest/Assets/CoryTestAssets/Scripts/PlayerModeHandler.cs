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
        parts = turretBlueprint.GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        if (playerModes == PlayerModes.BuildMode)
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

    private void SpawnBuilding(GameObject _prefab, string _resource1, string _resource2, int _resource1Cost, int _resource2Cost)
    {
        if (Input.GetMouseButtonDown(0) && InventoryManager.global.GetItemQuantity(_resource1) >= _resource1Cost && InventoryManager.global.GetItemQuantity(_resource2) >= _resource2Cost && !MouseOverUI())
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

    private void DragBuildingBlueprint(string _resource1, string _resource2, int _resource1Cost, int _resource2Cost)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource") && !MouseOverUI())
        {
            turretBlueprint.SetActive(true);

            Vector3 worldPos = hitData.point;

            turretBlueprint.transform.position = worldPos;

            if (IsInRange(worldPos) &&
                InventoryManager.global.GetItemQuantity(_resource1) >= _resource1Cost &&
                InventoryManager.global.GetItemQuantity(_resource2) >= _resource2Cost)
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
            turretBlueprint.SetActive(!MouseOverUI());
        }
    }
    public bool IsInRange(Vector3 currentTarget)
    {
        PlayerController player = PlayerController.global;

        return distanceAwayFromPlayer >= Vector3.Distance(player.transform.position, currentTarget);
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