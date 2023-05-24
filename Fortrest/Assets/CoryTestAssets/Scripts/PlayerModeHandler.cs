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


public class PlayerModeHandler : MonoBehaviour
{
    public static PlayerModeHandler global;
    public PlayerModes playerModes;
    public GameObject turretPrefabPlaced;
    public GameObject turretBlueprint;
    Transform[] parts;
    public Material turretBlueprintRed;
    public Material turretBlueprintBlue;
    public int woodConstructionCostTurret = 15;
    public int stoneConstructionCostTurret = 5;
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
            DragBuildingBlueprint();
            SpawnBuilding();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.ModeChangeClickSound);
            switch (playerModes)
            {
                case PlayerModes.BuildMode:
                    SwitchToCombatMode();
                    break;
                case PlayerModes.CombatMode:
                    SwitchToGatherMode();
                    break;
                case PlayerModes.ResourceMode:
                    SwitchToBuildMode();
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
        SwitchToGatherMode();

    }

    public void SwitchToBuildMode()
    {
        playerModes = PlayerModes.BuildMode;

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

        buildingMode.enabled = false;
        resourceMode.enabled = false;
        combatMode.enabled = true;
    }
    bool runOnce;

    private void SpawnBuilding()
    {
        if (Input.GetMouseButtonDown(0) && InventoryManager.global.GetItemQuantity("Wood") >= woodConstructionCostTurret && InventoryManager.global.GetItemQuantity("Stone") >= stoneConstructionCostTurret && !MouseOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource"))
            {
                Vector3 worldPos = hitData.point;

                if (worldPos.x <= PlayerController.global.transform.position.x + distanceAwayFromPlayer && worldPos.x >= PlayerController.global.transform.position.x - distanceAwayFromPlayer &&
                    worldPos.z <= PlayerController.global.transform.position.z + distanceAwayFromPlayer && worldPos.z >= PlayerController.global.transform.position.z - distanceAwayFromPlayer)
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TurretPlaceSound);
                    GameObject newTurret = Instantiate(turretPrefabPlaced, worldPos, Quaternion.identity);
                    InventoryManager.global.RemoveItem("Wood", woodConstructionCostTurret);
                    InventoryManager.global.RemoveItem("Stone", stoneConstructionCostTurret);

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
                    if (InventoryManager.global.GetItemQuantity("Wood") < woodConstructionCostTurret || InventoryManager.global.GetItemQuantity("Stone") < stoneConstructionCostTurret)
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

    private void DragBuildingBlueprint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !hitData.transform.CompareTag("Resource") && !MouseOverUI())
        {
            turretBlueprint.SetActive(true);

            Vector3 worldPos = hitData.point;

            turretBlueprint.transform.position = worldPos;

            if (worldPos.x <= PlayerController.global.transform.position.x + distanceAwayFromPlayer && worldPos.x >= PlayerController.global.transform.position.x - distanceAwayFromPlayer &&
                worldPos.z <= PlayerController.global.transform.position.z + distanceAwayFromPlayer && worldPos.z >= PlayerController.global.transform.position.z - distanceAwayFromPlayer &&
                InventoryManager.global.GetItemQuantity("Wood") >= woodConstructionCostTurret && InventoryManager.global.GetItemQuantity("Stone") >= stoneConstructionCostTurret)
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
}