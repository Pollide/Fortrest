using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchImageHUD : MonoBehaviour
{
    public GameObject turretSubHolder;
    public Image turretSubCross;
    public Image turretSubCannon;
    public Image turretSubGlyph;

    private PlayerModeHandler playerMode;

    private void Start()
    {
        turretSubHolder.SetActive(false);
        playerMode = PlayerModeHandler.global;
    }

    private void Update()
    {
        if (playerMode.playerModes == PlayerModes.BuildMode)
        {
            turretSubHolder.SetActive(true);
        }
        if (playerMode.playerModes != PlayerModes.BuildMode)
        {
            turretSubHolder.SetActive(false);
        }

        if (playerMode.buildType == BuildType.Turret)
        {
            turretSubCross.enabled = true;
            turretSubCannon.enabled = false;
            turretSubGlyph.enabled = false;
        }
        if (playerMode.buildType == BuildType.Cannon)
        {
            turretSubCross.enabled = false;
            turretSubCannon.enabled = true;
            turretSubGlyph.enabled = false;
        }
        if (playerMode.buildType == BuildType.Slow)
        {
            turretSubCross.enabled = false;
            turretSubCannon.enabled = false;
            turretSubGlyph.enabled = true;
        }
    }
}
