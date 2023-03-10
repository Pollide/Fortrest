using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public int constructionCostTurret = 2;
    public LayerMask buildingLayer;

    private void Awake()
    {
        global = this;
    }

    private void Update()
    {
        if (playerModes == PlayerModes.BuildMode)
        {
            DragBuildingBlueprint();
            SpawnBuilding();
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
    }

    public void SwitchToBuildMode()
    {
        playerModes = PlayerModes.BuildMode;
    }

    public void SwitchToGatherMode()
    {
        if (turretBlueprint.activeInHierarchy)
        {
            turretBlueprint.SetActive(false);
        }

        playerModes = PlayerModes.ResourceMode;
    }

    public void SwitchToCombatMode()
    {
        if (turretBlueprint.activeInHierarchy)
        {
            turretBlueprint.SetActive(false);
        }

        playerModes = PlayerModes.CombatMode;
    }

    private void SpawnBuilding()
    {
        if (Input.GetMouseButtonDown(0) && InventoryManager.global.wood >= constructionCostTurret && !MouseOverUI())
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;

            if (Physics.Raycast(ray, out hitData, 1000, ~buildingLayer) && !hitData.transform.CompareTag("Player") && !hitData.transform.CompareTag("Building"))
            {
                Vector3 worldPos = hitData.point;
                worldPos.y = PlayerController.global.transform.position.y;
                Instantiate(turretPrefabPlaced, worldPos, Quaternion.identity);
                InventoryManager.global.wood -= constructionCostTurret;
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
        else if (Input.GetMouseButtonDown(0) && InventoryManager.global.wood < constructionCostTurret)
        {
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

            for (int i = 0; i < turretBlueprint.transform.childCount; i++)
            {
                turretBlueprint.transform.transform.GetChild(i).tag = "BuildingBP";
            }


            Vector3 worldPos = hitData.point;

            turretBlueprint.transform.position = worldPos;
        }
        else if (Physics.Raycast(ray, out hitData, 1000) && (hitData.transform.CompareTag("Player") || hitData.transform.CompareTag("Building") || MouseOverUI()))
        {
            turretBlueprint.SetActive(false);
        }
    }
}
