using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{

    public enum ItemType
    {
        Wood,
        Stone,
        Food
    }

    public ItemType type;


    private void OnTriggerEnter(Collider other)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.CollectSound);
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            switch (type)
            {
                case ItemType.Food:

                    PlayerController.global.ApplyEnergyRestore(5);

                    break;

                case ItemType.Wood:

                    InventoryManager.global.AddWood(1);

                    break;

                case ItemType.Stone:

                    InventoryManager.global.AddStone(1);

                    break;
            }
        }
    }
}


