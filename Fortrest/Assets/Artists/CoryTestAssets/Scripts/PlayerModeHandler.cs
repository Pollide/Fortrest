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
        buildingMode.enabled = false;
        resourceMode.enabled = true;
        combatMode.enabled = false;
    }

    public void SwitchToBuildMode()
    {
        playerModes = PlayerModes.BuildMode;

        buildingMode.enabled = true;
        resourceMode.enabled = false;
        combatMode.enabled = false;
    }

    public void SwitchToGatherMode()
    {
        if (turretBlueprint.activeInHierarchy)
        {
            turretBlueprint.SetActive(false);
        }

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

        playerModes = PlayerModes.CombatMode;

        buildingMode.enabled = false;
        resourceMode.enabled = false;
        combatMode.enabled = true;
    }

    private void SpawnBuilding()
    {
        if (Input.GetMouseButtonDown(0) && InventoryManager.global.wood >= woodConstructionCostTurret && InventoryManager.global.wood >= stoneConstructionCostTurret && !MouseOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building"))
            {
                Vector3 worldPos = hitData.point;

                if (worldPos.x <= PlayerController.global.transform.position.x + distanceAwayFromPlayer && worldPos.x >= PlayerController.global.transform.position.x - distanceAwayFromPlayer &&
                    worldPos.z <= PlayerController.global.transform.position.z + distanceAwayFromPlayer && worldPos.z >= PlayerController.global.transform.position.z - distanceAwayFromPlayer)
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.TurretPlaceSound);
                    Instantiate(turretPrefabPlaced, worldPos, Quaternion.identity);
                    InventoryManager.global.wood -= woodConstructionCostTurret;
                    InventoryManager.global.stone -= stoneConstructionCostTurret;
                    // Debug.Log("working");
                }
            }
            else if (Physics.Raycast(ray, out hitData, 1000) && hitData.transform.CompareTag("Player"))
            {
                Debug.Log("Cannot Place Building Here");
            }
            else if (Physics.Raycast(ray, out hitData, 1000) && hitData.transform.CompareTag("Building"))
            {
                Debug.Log("Building Here");
            }
        }
        else if (Input.GetMouseButtonDown(0) && InventoryManager.global.wood < woodConstructionCostTurret && InventoryManager.global.stone < stoneConstructionCostTurret)
        {
            GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
            Debug.Log("Not Enough Resources");
        }
    }

    private void DragBuildingBlueprint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building") && !MouseOverUI())
        {
            turretBlueprint.SetActive(true);

            Vector3 worldPos = hitData.point;

            turretBlueprint.transform.position = worldPos;

            if (worldPos.x <= PlayerController.global.transform.position.x + distanceAwayFromPlayer && worldPos.x >= PlayerController.global.transform.position.x - distanceAwayFromPlayer &&
                worldPos.z <= PlayerController.global.transform.position.z + distanceAwayFromPlayer && worldPos.z >= PlayerController.global.transform.position.z - distanceAwayFromPlayer &&
                InventoryManager.global.wood >= woodConstructionCostTurret && InventoryManager.global.wood >= stoneConstructionCostTurret)
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