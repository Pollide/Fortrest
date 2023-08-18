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
    public Animation FloatingTextAnimation;
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

        if (BridgeTypeInt == 3)
        {
            Indicator.global.AddIndicator(transform, Color.yellow, "Coast", false);
        }

        if (BridgeTypeInt == 4)
        {
            Indicator.global.AddIndicator(transform, Color.blue, "Taiga", false);
        }


        LevelManager.global.BridgeList.Add(this);
    }
    void ShowResources(bool show)
    {
        PlayerController.global.OpenResourceHolder(show);
        PlayerController.global.needInteraction = show;

        LevelManager.FloatingTextChange(FloatingTextAnimation.gameObject, show);

        if (show)
            PlayerController.global.UpdateResourceHolder(BridgeTypeInt);
    }

    private void Update()
    {
        bool open = Vector3.Distance(transform.position, PlayerController.global.transform.position) < 15;

        if (triggered != open)
        {
            triggered = open;
            ShowResources(open);

        }

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
