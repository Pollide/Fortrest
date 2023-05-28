using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public void Slow()
    {
        PlayerModeHandler.global.SwitchBuildTypeSlow();
        PlayerModeHandler.global.SwitchToBuildMode();
        InventoryManager.global.CloseInventory(false, false);
    }

    public void Turret()
    {
        PlayerModeHandler.global.SwitchToBuildMode();
        PlayerModeHandler.global.SwitchBuildTypeTurret();
        InventoryManager.global.CloseInventory(false, false);
    }

    public void Cannon()
    {
        PlayerModeHandler.global.SwitchToBuildMode();
        PlayerModeHandler.global.SwitchBuildTypeCannon();
        InventoryManager.global.CloseInventory(false, false);
    }
}
