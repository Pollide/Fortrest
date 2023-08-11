using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignTrigger : MonoBehaviour
{
    [SerializeField] private GameObject activateText;
    [SerializeField] private bool inRange = false;
    [SerializeField] private bool hasRun = false;
    [SerializeField] private BridgeBuilder bridgeBuilder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activateText.SetActive(true);
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activateText.SetActive(false);
            inRange = false;
        }
    }

    private void Update()
    {
        if (inRange)
        {
            PlayerController.global.needInteraction = true;
        }
        else
        {
            PlayerController.global.needInteraction = false;
        }

        if (inRange && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL) && !hasRun)
        {
            PlayerController.global.interactCTRL = false;
            if (bridgeBuilder.resourceCostList.Keys.Count < bridgeBuilder.resourceCostList.Values.Count)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                Debug.Log("Not enough resources to match values");
            }
            else if (bridgeBuilder.resourceCostList.Keys.Count > bridgeBuilder.resourceCostList.Values.Count)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                Debug.Log("Not enough values to match resources");
            }
            else if (bridgeBuilder.resourceCostList.Keys.Count > 0 && bridgeBuilder.resourceCostList.Values.Count > 0 && bridgeBuilder.resourceCostList.Keys.Count == bridgeBuilder.resourceCostList.Values.Count && HasAllResources() == false)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
                Debug.Log("Not enough resources");
            }
            else if (bridgeBuilder.resourceCostList.Keys.Count > 0 && bridgeBuilder.resourceCostList.Values.Count > 0 && bridgeBuilder.resourceCostList.Keys.Count == bridgeBuilder.resourceCostList.Values.Count && HasAllResources() == true)
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltNoiseSound);
                GetComponentInParent<BridgeBuilder>().isBuilt = true;
                hasRun = true;
            }
        }
    }

    private bool HasAllResources()
    {
        InventoryManager inventoryManager = InventoryManager.global;
        for (int i = 0; i < bridgeBuilder.resourceCostList.Count; i++)
        {
            if (inventoryManager.GetItemQuantity(bridgeBuilder.resourceCostList.Keys[i]) < bridgeBuilder.resourceCostList.Values[i])
            {
                return false;
            }
        }
        return true;
    }
}
