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
    [Header("Food Restore Amount")]
    public float restoreAmount = 5f;
    [Header("Resource Amount (Stone and Wood)")]
    public int resourceAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundPlayed = false;
            Destroy(gameObject);

            switch (type)
            {
                case ItemType.Food:
                    
                    if(PlayerController.global.playerEnergy < PlayerController.global.maxPlayerEnergy)
                    {
                        PlayerController.global.ApplyEnergyRestore(restoreAmount);
                    }
         
                    break;

                case ItemType.Wood:

                    InventoryManager.global.AddWood(resourceAmount);

                    break;

                case ItemType.Stone:

                    InventoryManager.global.AddStone(resourceAmount);

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


