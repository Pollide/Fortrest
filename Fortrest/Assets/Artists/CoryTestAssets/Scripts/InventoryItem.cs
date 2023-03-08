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

    bool soundPlayed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundPlayed = false;
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

        if (!soundPlayed)
        {
            soundPlayed = true;
            GameManager.global.SoundManager.PlaySound(GameManager.global.CollectSound);
        }
    }
}


