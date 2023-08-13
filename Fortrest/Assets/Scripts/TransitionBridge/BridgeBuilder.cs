using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BridgeBuilder : MonoBehaviour
{

    public int BridgeTypeInt;
    public bool isBuilt;
    bool triggered;
    public GameObject DamagedGameObject;
    public GameObject RepairedGameObject;

    private void Start()
    {
        if (BridgeTypeInt == 1)
        {
            Indicator.global.AddIndicator(transform, Color.magenta, "Marsh", false);
        }

        if (BridgeTypeInt == 2)
        {
            Indicator.global.AddIndicator(transform, new Color(1.0f, 0.6f, 0.0f, 1.0f), "Tussocks", false);
        }

        LevelManager.global.BridgeList.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBuilt && other.CompareTag("Player"))
        {
            triggered = true;
            ShowResources(true);
            PlayerController.global.UpdateResourceHolder(BridgeTypeInt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isBuilt && other.CompareTag("Player"))
        {
            triggered = false;
            ShowResources(false);
        }
    }

    void ShowResources(bool show)
    {
        PlayerController.global.MapResourceHolder.gameObject.SetActive(show);
        PlayerController.global.needInteraction = show;
    }

    private void Update()
    {
        if (!isBuilt && triggered && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL))
        {
            PlayerController.global.interactCTRL = false;

            if (PlayerController.global.CheckSufficientResources(true))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltNoiseSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltSound);
                isBuilt = true;
                ShowResources(false);
                BuildBridge();
            }
            else
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.CantPlaceSound);
            }
        }
    }

    public void BuildBridge()
    {
        DamagedGameObject.SetActive(false);
        RepairedGameObject.SetActive(true);
    }
}
