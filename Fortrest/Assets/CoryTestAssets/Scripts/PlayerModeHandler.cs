using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerModes
{
    BuildMode,
    ResourceMode,
    CombatMode
}

public class PlayerModeHandler : MonoBehaviour
{
    public PlayerModes playerModes;

    public Image buildModeImage;
    public Image resourceModeImage;
    public Image combatModeImage;

    // Start is called before the first frame update
    void Start()
    {
        playerModes = PlayerModes.ResourceMode;
        resourceModeImage.enabled = true;
        buildModeImage.enabled = false;
        combatModeImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (playerModes)
            {
                case PlayerModes.BuildMode:
                    playerModes = PlayerModes.ResourceMode;
                    buildModeImage.enabled = false;
                    resourceModeImage.enabled = true;
                    combatModeImage.enabled = false;
                    break;
                case PlayerModes.CombatMode:
                    playerModes = PlayerModes.BuildMode;
                    buildModeImage.enabled = true;
                    resourceModeImage.enabled = false;
                    combatModeImage.enabled = false;
                    break;
                case PlayerModes.ResourceMode:
                    playerModes = PlayerModes.CombatMode;
                    buildModeImage.enabled = false;
                    resourceModeImage.enabled = false;
                    combatModeImage.enabled = true;
                    break;
            }
        }
    }
}
