using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableEvents : MonoBehaviour
{

    public void DeleteItem()
    {
        DragableItem drag = GetComponent<DragableItem>();
        GameManager.global.SoundManager.PlaySound(GameManager.global.SpeedButtonClickSound);
        InventoryManager inv = InventoryManager.global;

        inv.RemoveItem(drag.name, inv.GetItemQuantity(drag.name));
    }

    public void EatFood()
    {
        InventoryManager.global.RemoveItem("Apple");
        GameManager.global.SoundManager.PlaySound(Random.Range(0, 2) == 0 ? GameManager.global.PlayerEatSound : GameManager.global.EatingSound);
        PlayerController.global.ApplyEnergyRestore(5);
    }
}
