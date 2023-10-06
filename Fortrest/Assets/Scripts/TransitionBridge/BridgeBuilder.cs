using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BridgeBuilder : MonoBehaviour
{

    public int BridgeTypeInt;
    public bool isBuilt;
    bool triggered;
    public Animation FloatingTextAnimation;
    public GameObject WalkAccrossCollider;
    public Animator bridgeAnimator;

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

        LevelManager.global.bridgeList.Add(this);
    }
    void ShowResources(bool show)
    {
        /*
       // PlayerController.global.OpenResourceHolder(show);
       // PlayerController.global.needInteraction = show;
        //PlayerController.global.bridgeInteract = show;
        LevelManager.FloatingTextChange(FloatingTextAnimation.gameObject, show);

        if (show)
            PlayerController.global.UpdateResourceHolder(BridgeTypeInt);
*/
    }

    private void Update()
    {
        if (PlayerController.global.pausedBool || PlayerController.global.mapBool)
        {
            return;
        }

        if (!isBuilt)
        {
            bool open = Vector3.Distance(transform.position, PlayerController.global.transform.position) < 20;

            if (triggered != open)
            {
                triggered = open;
                LevelManager.FloatingTextChange(FloatingTextAnimation.gameObject, open);

            }

            /*
            if (triggered && (Input.GetKeyDown(KeyCode.E) || PlayerController.global.interactCTRL))
            {
                PlayerController.global.interactCTRL = false;
                PlayerController.global.evading = false;
                if (PlayerController.global.CheckSufficientResources(true))
                {
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltNoiseSound);
                    GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltSound);
                    isBuilt = true;
                    LevelManager.FloatingTextChange(FloatingTextAnimation.gameObject, false);

                    GameManager.PlayAnimator(bridgeAnimator, "Armature_BridgeSelfBuild");
                    WalkAccrossCollider.SetActive(true);
                }
                else
                {
                    PlayerController.global.ShakeResourceHolder();
                }
            }
            */
        }
    }

    public void Build()
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltNoiseSound);
        GameManager.global.SoundManager.PlaySound(GameManager.global.BridgeBuiltSound);
        isBuilt = true;
        LevelManager.FloatingTextChange(FloatingTextAnimation.gameObject, false);

        GameManager.PlayAnimator(bridgeAnimator, "Armature_BridgeSelfBuild");
        WalkAccrossCollider.SetActive(true);
    }
}
