using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BridgeBuilder : MonoBehaviour
{

    public int BridgeTypeInt;
    public bool isBuilt;
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
        if (enabled && other.CompareTag("Player"))
        {
            ShowResources(true);
            PlayerController.global.UpdateResourceHolder(BridgeTypeInt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
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
        if (PlayerController.global.MapResourceHolder.gameObject.activeSelf && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL))
        {
            PlayerController.global.interactCTRL = false;

            if (PlayerController.global.CheckSufficientResources(true))
            {
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltNoiseSound);
                GameManager.global.SoundManager.PlaySound(GameManager.global.HouseBuiltSound);
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
        enabled = false;
        DamagedGameObject.SetActive(false);
        RepairedGameObject.SetActive(true);
    }
}
