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
    public Image turretSubScatter;

    private PlayerModeHandler playerMode;

    private void Start()
    {
        turretSubHolder.SetActive(false);
        playerMode = PlayerModeHandler.global;
    }

    private void Update()
    {
        turretSubHolder.SetActive(playerMode.playerModes == PlayerModes.BuildMode);

        if (playerMode.buildType == BuildType.Turret)
        {
            turretSubCross.enabled = true;
            turretSubCannon.enabled = false;
            turretSubGlyph.enabled = false;
            turretSubScatter.enabled = false;
        }
        if (playerMode.buildType == BuildType.Cannon)
        {
            turretSubCross.enabled = false;
            turretSubCannon.enabled = true;
            turretSubGlyph.enabled = false;
            turretSubScatter.enabled = false;
        }
        if (playerMode.buildType == BuildType.Slow)
        {
            turretSubCross.enabled = false;
            turretSubCannon.enabled = false;
            turretSubGlyph.enabled = true;
            turretSubScatter.enabled = false;
        }
        if (playerMode.buildType == BuildType.Scatter)
        {
            turretSubCross.enabled = false;
            turretSubCannon.enabled = false;
            turretSubGlyph.enabled = false;
            turretSubScatter.enabled = true;
        }
    }
}
