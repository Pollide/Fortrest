using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignTrigger : MonoBehaviour
{
    public int BridgeTypeInt;
    public GameObject DamagedGameObject;
    public GameObject RepairedGameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.global.MapResourceHolder.gameObject.SetActive(true);
            PlayerController.global.UpdateResourceHolder(BridgeTypeInt);
            PlayerController.global.needInteraction = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.global.MapResourceHolder.gameObject.SetActive(false);
            PlayerController.global.needInteraction = false;
        }
    }

    private void Update()
    {
        if (PlayerController.global.MapResourceHolder.gameObject.activeSelf && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL))
        {
            PlayerController.global.interactCTRL = false;

            if (PlayerController.global.CheckSufficientResources(true))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltNoiseSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltSound);
                GetComponentInParent<BridgeBuilder>().isBuilt = true;
            }
            else
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
            }
        }
    }

    public void BuildBridge()
    {

    }
}
